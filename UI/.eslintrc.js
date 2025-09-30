const config = require('@lobehub/lint').eslint;

config.extends.push('plugin:@next/next/recommended');

config.rules['unicorn/no-negated-condition'] = 0;
config.rules['unicorn/prefer-type-error'] = 0;
config.rules['unicorn/prefer-logical-operator-over-ternary'] = 0;
config.rules['unicorn/no-null'] = 0;
config.rules['unicorn/no-typeof-undefined'] = 0;
config.rules['unicorn/explicit-length-check'] = 0;
config.rules['unicorn/prefer-code-point'] = 0;
config.rules['no-extra-boolean-cast'] = 0;
config.rules['unicorn/no-useless-undefined'] = 0;
config.rules['react/no-unknown-property'] = 0;
config.rules['unicorn/prefer-ternary'] = 0;
config.rules['unicorn/prefer-spread'] = 0;
config.rules['unicorn/catch-error-name'] = 0;
config.rules['unicorn/no-array-for-each'] = 0;
config.rules['unicorn/prefer-number-properties'] = 0;


config.rules['unused-imports/no-unused-imports'] = 0;
config.rules['@typescript-eslint/no-unused-vars'] = 0;

// 2. 禁用导入/模块相关规则
config.rules['import/no-duplicates'] = 0;
config.rules['import/first'] = 0;

// 3. 禁用潜在的逻辑错误检查
config.rules['no-unreachable'] = 0;
config.rules['no-param-reassign'] = 0;
config.rules['@typescript-eslint/no-use-before-define'] = 0;
config.rules['no-useless-escape'] = 0;

// 4. 禁用代码风格和排序规则
config.rules['sort-keys-fix/sort-keys-fix'] = 0;
config.rules['typescript-sort-keys/interface'] = 0;
config.rules['react/jsx-sort-props'] = 0;
config.rules['react/jsx-no-useless-fragment'] = 0;
config.rules['react/self-closing-comp'] = 0;

// 5. 禁用最佳实践和现代化语法建议
config.rules['unicorn/prefer-node-protocol'] = 0;
config.rules['unicorn/prefer-structured-clone'] = 0;
config.rules['unicorn/prefer-array-index-of'] = 0;
config.rules['unicorn/numeric-separators-style'] = 0;
config.rules['unicorn/no-hex-escape'] = 0;
config.rules['unicorn/prefer-number-properties'] = 0; // 这是您新提到的规则
config.rules['no-var'] = 0;

// 6. 禁用 Next.js 相关建议
config.rules['@next/next/no-img-element'] = 0;


config.overrides = [
  {
    extends: ['plugin:mdx/recommended'],
    files: ['*.mdx'],
    rules: {
      '@typescript-eslint/no-unused-vars': 0,
      'no-undef': 0,
      'react/jsx-no-undef': 0,
      'react/no-unescaped-entities': 0,
    },
    settings: {
      'mdx/code-blocks': false,
    },
  },
];

module.exports = config;
