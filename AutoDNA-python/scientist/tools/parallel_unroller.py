"""Deterministically unroll the marked parallelization loop in Python source."""

from __future__ import annotations

import argparse
import ast
import copy
import json
import sys
from pathlib import Path
from typing import Iterable, Sequence


DEFAULT_MARKER = "# Consider parallelizing the following loop"


class UnrollError(ValueError):
    """Raised when source code does not match the supported deterministic shape."""


def unroll_code(source: str, marker: str = DEFAULT_MARKER) -> str:
    copies = unroll_code_copies(source, marker=marker)
    return "\n\n".join(
        f"=== Code {index} ===\n{code}"
        for index, code in enumerate(copies, start=1)
    )


def unroll_code_copies(source: str, marker: str = DEFAULT_MARKER) -> list[str]:
    """Return one complete Python source copy per marked-loop entry."""
    source = _strip_single_python_code_fence(source)
    tree = ast.parse(source)
    lines = source.splitlines(keepends=True)
    marker_lineno = _find_marker_lineno(lines, marker)
    loop = _find_marked_loop(tree, lines, marker_lineno)
    loop_parent_block = _find_parent_statement_block(tree, loop)

    _validate_last_statement_in_block(loop_parent_block, loop)
    _validate_single_call_body(loop)

    symbolic_entries = _resolve_symbolic_list_comprehension_loop(
        loop.iter,
        tree,
        before_lineno=loop.lineno,
        scope_block=loop_parent_block,
    )
    if symbolic_entries is None:
        entry_sources = [
            _literal_to_source(entry)
            for entry in _resolve_iterable(
                loop.iter,
                tree,
                before_lineno=loop.lineno,
                scope_block=loop_parent_block,
            )
        ]
        removed_statement = None
    else:
        entry_sources, removed_statement = symbolic_entries

    prefix = _source_before_loop(lines, loop, removed_statement)
    suffix = "".join(lines[loop.end_lineno :])

    target_source = _get_expression_source(source, loop.target)
    call_source = _get_statement_source(source, loop.body[0])
    loop_indent = _line_indent(lines[loop.lineno - 1])

    copies = []
    for entry_source in entry_sources:
        assignment = f"{loop_indent}{target_source} = {entry_source}\n"
        call = _indent_block(call_source, loop_indent)
        replacement = assignment + _ensure_trailing_newline(call)
        code = prefix + replacement + suffix
        copies.append(code.rstrip())

    return copies


def _strip_single_python_code_fence(source: str) -> str:
    lines = source.splitlines(keepends=True)
    if not lines:
        return source

    nonblank_indexes = [index for index, line in enumerate(lines) if line.strip()]
    if not nonblank_indexes:
        return source

    first_index = nonblank_indexes[0]
    last_index = nonblank_indexes[-1]
    opening = lines[first_index].strip()
    closing = lines[last_index].strip()
    if not opening.startswith("```") or not closing.startswith("```"):
        return source

    language = opening[3:].strip().lower()
    if language and language not in {"python", "py"}:
        return source

    return "".join(lines[first_index + 1 : last_index])


def _find_marker_lineno(lines: Sequence[str], marker: str) -> int:
    matches = [
        index
        for index, line in enumerate(lines, start=1)
        if line.lstrip().startswith(marker)
    ]
    if not matches:
        raise UnrollError(f"Marker not found: {marker}")
    if len(matches) > 1:
        raise UnrollError(f"Expected exactly one marker, found {len(matches)}")
    return matches[0]


def _find_marked_loop(
    tree: ast.Module, lines: Sequence[str], marker_lineno: int
) -> ast.For:
    loops = [
        node
        for node in ast.walk(tree)
        if isinstance(node, ast.For) and node.lineno > marker_lineno
    ]
    if len(loops) != 1:
        raise UnrollError(
            f"Expected exactly one for loop after the marker, found {len(loops)}"
        )

    loop = loops[0]
    first_code_lineno = _first_code_lineno_after(lines, marker_lineno)
    if first_code_lineno != loop.lineno:
        raise UnrollError("The first executable statement after the marker must be the loop")
    return loop


def _first_code_lineno_after(lines: Sequence[str], marker_lineno: int) -> int:
    for index in range(marker_lineno, len(lines)):
        stripped = lines[index].strip()
        if stripped and not stripped.startswith("#"):
            return index + 1
    raise UnrollError("No executable statement found after the marker")


def _find_parent_statement_block(node: ast.AST, loop: ast.For) -> list[ast.stmt]:
    for block in _iter_child_statement_blocks(node):
        if any(statement is loop for statement in block):
            return block

        for statement in block:
            try:
                return _find_parent_statement_block(statement, loop)
            except UnrollError:
                pass

    raise UnrollError("Could not locate the marked loop parent block")


def _iter_child_statement_blocks(node: ast.AST):
    for _field_name, value in ast.iter_fields(node):
        if (
            isinstance(value, list)
            and value
            and all(isinstance(item, ast.stmt) for item in value)
        ):
            yield value


def _validate_last_statement_in_block(block: list[ast.stmt], loop: ast.For) -> None:
    for index, statement in enumerate(block):
        if statement is loop:
            if block[index + 1 :]:
                raise UnrollError("Executable code exists after the marked loop")
            return
    raise UnrollError("Could not locate the marked loop in its parent block")


def _validate_single_call_body(loop: ast.For) -> None:
    if len(loop.body) != 1:
        raise UnrollError("The marked loop body must contain a single function call")

    statement = loop.body[0]
    if not isinstance(statement, ast.Expr) or not isinstance(statement.value, ast.Call):
        raise UnrollError("The marked loop body must contain a single function call")


def _source_before_loop(
    lines: Sequence[str],
    loop: ast.For,
    removed_statement: ast.stmt | None,
) -> str:
    prefix_lines = list(lines[: loop.lineno - 1])
    if removed_statement is not None:
        start_index = removed_statement.lineno - 1
        end_index = removed_statement.end_lineno or removed_statement.lineno
        for index in range(start_index, min(end_index, len(prefix_lines))):
            prefix_lines[index] = ""
    return "".join(prefix_lines)


def _resolve_symbolic_list_comprehension_loop(
    node: ast.AST,
    tree: ast.Module,
    before_lineno: int,
    scope_block: list[ast.stmt] | None = None,
) -> tuple[list[str], ast.stmt] | None:
    if not isinstance(node, ast.Name):
        return None

    assignment = _find_prior_assignment_statement(
        tree,
        node.id,
        before_lineno,
        scope_block=scope_block,
    )
    if assignment is None or not isinstance(assignment.value, ast.ListComp):
        return None

    list_comp = assignment.value
    if len(list_comp.generators) != 1:
        raise UnrollError("List comprehension iterable is not statically resolvable")

    generator = list_comp.generators[0]
    if generator.is_async or generator.ifs:
        raise UnrollError("List comprehension iterable is not statically resolvable")
    if not isinstance(generator.target, ast.Name):
        raise UnrollError("List comprehension target is not statically resolvable")

    values = _resolve_iterable(
        generator.iter,
        tree,
        before_lineno,
        scope_block=scope_block,
    )
    entry_sources = [
        _inline_comprehension_value(list_comp.elt, generator.target.id, value)
        for value in values
    ]
    return entry_sources, assignment


def _inline_comprehension_value(template: ast.AST, name: str, value: object) -> str:
    expression = copy.deepcopy(template)
    expression = _NameLiteralSubstituter(name, value).visit(expression)
    ast.fix_missing_locations(expression)
    return ast.unparse(expression)


class _NameLiteralSubstituter(ast.NodeTransformer):
    def __init__(self, name: str, value: object):
        self.name = name
        self.value = value

    def visit_Name(self, node: ast.Name):
        if node.id == self.name and isinstance(node.ctx, ast.Load):
            return ast.copy_location(_literal_to_ast_node(self.value), node)
        return node


def _literal_to_ast_node(value: object) -> ast.AST:
    return ast.parse(_literal_to_source(value), mode="eval").body


def _resolve_iterable(
    node: ast.AST,
    tree: ast.Module,
    before_lineno: int,
    scope_block: list[ast.stmt] | None = None,
) -> list[object]:
    if isinstance(node, ast.Name):
        assignment = _find_prior_assignment(
            tree,
            node.id,
            before_lineno,
            scope_block=scope_block,
        )
        if assignment is None:
            raise UnrollError(
                f"Loop iterable '{node.id}' is not statically resolvable"
            )
        return _resolve_iterable(
            assignment,
            tree,
            before_lineno,
            scope_block=scope_block,
        )

    if isinstance(node, (ast.List, ast.Tuple)):
        return list(ast.literal_eval(node))

    if isinstance(node, ast.Call):
        return _resolve_call_iterable(
            node,
            tree,
            before_lineno,
            scope_block=scope_block,
        )

    try:
        value = ast.literal_eval(node)
    except (TypeError, ValueError) as exc:
        raise UnrollError("Loop iterable is not statically resolvable") from exc

    if not isinstance(value, Iterable) or isinstance(value, (str, bytes)):
        raise UnrollError("Loop iterable is not statically resolvable")
    return list(value)


def _find_prior_assignment(
    tree: ast.Module,
    name: str,
    before_lineno: int,
    scope_block: list[ast.stmt] | None = None,
) -> ast.AST | None:
    return _find_prior_assignment_with_scope(
        tree,
        name,
        before_lineno,
        scope_block=scope_block,
    )


def _find_prior_assignment_with_scope(
    tree: ast.Module,
    name: str,
    before_lineno: int,
    scope_block: list[ast.stmt] | None = None,
) -> ast.AST | None:
    blocks = []
    if scope_block is not None:
        blocks.append(scope_block)
    if not any(block is tree.body for block in blocks):
        blocks.append(tree.body)

    for block in blocks:
        found = _find_prior_assignment_in_block(block, name, before_lineno)
        if found is not None:
            return found
    return None


def _find_prior_assignment_statement(
    tree: ast.Module,
    name: str,
    before_lineno: int,
    scope_block: list[ast.stmt] | None = None,
) -> ast.Assign | ast.AnnAssign | None:
    blocks = []
    if scope_block is not None:
        blocks.append(scope_block)
    if not any(block is tree.body for block in blocks):
        blocks.append(tree.body)

    for block in blocks:
        found = _find_prior_assignment_statement_in_block(block, name, before_lineno)
        if found is not None:
            return found
    return None


def _find_prior_assignment_statement_in_block(
    block: list[ast.stmt],
    name: str,
    before_lineno: int,
) -> ast.Assign | ast.AnnAssign | None:
    found = None
    for statement in block:
        if statement.lineno >= before_lineno:
            break

        if isinstance(statement, ast.Assign):
            for target in statement.targets:
                if isinstance(target, ast.Name) and target.id == name:
                    found = statement
        elif isinstance(statement, ast.AnnAssign):
            if isinstance(statement.target, ast.Name) and statement.target.id == name:
                found = statement

    return found


def _find_prior_assignment_in_block(
    block: list[ast.stmt],
    name: str,
    before_lineno: int,
) -> ast.AST | None:
    found = None
    for statement in block:
        if statement.lineno >= before_lineno:
            break

        if isinstance(statement, ast.Assign):
            for target in statement.targets:
                if isinstance(target, ast.Name) and target.id == name:
                    found = statement.value

        if isinstance(statement, ast.AnnAssign):
            if isinstance(statement.target, ast.Name) and statement.target.id == name:
                found = statement.value

    return found


def _resolve_call_iterable(
    node: ast.Call,
    tree: ast.Module,
    before_lineno: int,
    scope_block: list[ast.stmt] | None = None,
) -> list[object]:
    if isinstance(node.func, ast.Name) and node.func.id == "range":
        if node.keywords:
            raise UnrollError("range(...) with keywords is not statically resolvable")
        args = [_literal_int(arg) for arg in node.args]
        if not 1 <= len(args) <= 3:
            raise UnrollError("range(...) must have one to three integer arguments")
        return list(range(*args))

    if isinstance(node.func, ast.Name) and node.func.id in {"list", "tuple"}:
        if len(node.args) != 1 or node.keywords:
            raise UnrollError(f"{node.func.id}(...) is not statically resolvable")
        return list(
            _resolve_iterable(
                node.args[0],
                tree,
                before_lineno,
                scope_block=scope_block,
            )
        )

    if isinstance(node.func, ast.Name) and node.func.id == "enumerate":
        return list(_resolve_enumerate_call(node, tree, before_lineno, scope_block))

    raise UnrollError("Loop iterable is not statically resolvable")


def _resolve_enumerate_call(
    node: ast.Call,
    tree: ast.Module,
    before_lineno: int,
    scope_block: list[ast.stmt] | None,
):
    if not 1 <= len(node.args) <= 2:
        raise UnrollError("enumerate(...) must have one iterable argument")

    start = 0
    has_positional_start = len(node.args) == 2
    if has_positional_start:
        start = _literal_int(node.args[1])

    for keyword in node.keywords:
        if keyword.arg != "start":
            raise UnrollError("enumerate(...) keyword is not statically resolvable")
        if has_positional_start:
            raise UnrollError("enumerate(...) start was provided twice")
        start = _literal_int(keyword.value)

    iterable = _resolve_iterable(
        node.args[0],
        tree,
        before_lineno,
        scope_block=scope_block,
    )
    return enumerate(iterable, start=start)


def _literal_int(node: ast.AST) -> int:
    try:
        value = ast.literal_eval(node)
    except (TypeError, ValueError) as exc:
        raise UnrollError("range(...) arguments must be literal integers") from exc
    if not isinstance(value, int):
        raise UnrollError("range(...) arguments must be literal integers")
    return value


def _get_statement_source(source: str, statement: ast.stmt) -> str:
    segment = ast.get_source_segment(source, statement)
    if segment is None:
        segment = ast.unparse(statement)
    return segment.strip()


def _get_expression_source(source: str, expression: ast.expr) -> str:
    segment = ast.get_source_segment(source, expression)
    if segment is None:
        segment = ast.unparse(expression)
    return segment.strip()


def _line_indent(line: str) -> str:
    return line[: len(line) - len(line.lstrip())]


def _indent_block(text: str, indent: str) -> str:
    return "\n".join(
        f"{indent}{line}" if line else line for line in text.splitlines()
    )


def _ensure_trailing_newline(text: str) -> str:
    if text.endswith("\n"):
        return text
    return f"{text}\n"


def _literal_to_source(value: object) -> str:
    if isinstance(value, str):
        return json.dumps(value, ensure_ascii=False)
    return repr(value)


def main(argv: Sequence[str] | None = None) -> int:
    parser = argparse.ArgumentParser(
        description=(
            "Unroll the loop following '# Consider parallelizing the following loop' "
            "into complete code copies."
        )
    )
    parser.add_argument(
        "input",
        nargs="?",
        default="-",
        help="Python input file. Use '-' or omit to read from stdin.",
    )
    parser.add_argument(
        "-o",
        "--output",
        help="Output file. If omitted, writes to stdout.",
    )
    parser.add_argument(
        "--marker",
        default=DEFAULT_MARKER,
        help="Comment marker that immediately precedes the loop.",
    )
    args = parser.parse_args(argv)

    if args.input == "-":
        source = sys.stdin.read()
    else:
        source = Path(args.input).read_text(encoding="utf-8")

    try:
        output = unroll_code(source, marker=args.marker)
    except UnrollError as exc:
        print(f"unroll_parallel_loop: {exc}", file=sys.stderr)
        return 2

    if args.output:
        Path(args.output).write_text(output, encoding="utf-8")
    else:
        sys.stdout.write(output)
        if output and not output.endswith("\n"):
            sys.stdout.write("\n")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
