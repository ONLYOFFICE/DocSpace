import styled from "styled-components";

import Base from "@appserver/components/themes/base";

const StyledSelectFolder = styled.div`
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

const EditorWrapper = styled.div`
  height: 100vh;

  .dynamic-sharing-dialog {
    ${(props) => !props.isVisibleSharingDialog && "display: none"}
  }
`;

export { StyledSelectFolder, StyledSelectFile, EditorWrapper };
