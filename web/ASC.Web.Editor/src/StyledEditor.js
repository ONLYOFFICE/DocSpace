import styled from "styled-components";

const StyledSelectFolder = styled.div`
  .editor-select-folder_text {
    color: #555f65;
  }
  .editor-select-folder_text-input {
    margin-top: 8px;
  }
  .editor-select-folder_checkbox {
    background-color: white;
    word-break: break-word;
  }
`;

const StyledSelectFile = styled.div`
  .editor-select-file_text {
    word-break: break-word;
  }
`;

const EditorWrapper = styled.div`
  display: ${(props) => !props.isShowEditor && "none"};
  height: 100vh;

  .dynamic-sharing-dialog {
    ${(props) => !props.isVisibleSharingDialog && "display: none"}
  }
`;

export { StyledSelectFolder, StyledSelectFile, EditorWrapper };
