import React from 'react';
import { diffLines, Change } from 'diff';
import './diffviewer.css'; // Don't forget to create this CSS file!

interface CodeDiffViewerProps {
  oldCode: string;
  newCode: string;
}

const CodeDiffViewer: React.FC<CodeDiffViewerProps> = ({ oldCode, newCode }) => {
  const diff = diffLines(oldCode, newCode);

  return (
    <div className="code-diff-viewer">
      <div>Code repairing...</div>
      <div className="diff-container">
        <pre className="old-code">
          {diff.map((part: Change, index: number) => {
            if (part.added) {
              return null;
            }
            return (
              <span key={index} className={`line ${part.removed ? 'removed' : 'unchanged'}`}>
                {part.value}
              </span>
            );
          })}
        </pre>
        <pre className="new-code">
          {diff.map((part: Change, index: number) => {
            if (part.removed) {
              return null;
            }
            return (
              <span key={index} className={`line ${part.added ? 'added' : 'unchanged'}`}>
                {part.value}
              </span>
            );
          })}
        </pre>
      </div>
    </div>
  );
};

export default CodeDiffViewer;