import styled from 'styled-components';

import Base from '@appserver/components/themes/base';

const StyledSelectFolder = styled.div`
  .editor-select-folder_text {
    color: ${(props) => props.theme.editor.color};
  }
  .editor-select-folder_text-input {
    margin-top: 8px;
  }
  .editor-select-folder_checkbox {
    background-color: ${(props) => props.theme.editor.background};
    word-break: break-word;
  }
`;

StyledSelectFolder.defaultProps = { theme: Base };

const StyledSelectFile = styled.div`
  .editor-select-file_text {
    word-break: break-word;
  }
`;

export { StyledSelectFolder, StyledSelectFile };
