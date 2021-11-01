import styled from "styled-components";

const StyledDialogLoader = styled.div`
  .dialog-loader-header {
    border-bottom: 1px solid rgb(222, 226, 230);
    display: flex;
    padding: 12px 0;
  }

  .dialog-loader-body {
    display: flex;
    flex-direction: column;
    justify-content: center;
    padding-top: 12px;
  }

  .dialog-loader-footer {
    display: flex;
    padding-top: 12px;
  }

  .dialog-loader-icon {
    margin-left: auto;
  }
`;

export default StyledDialogLoader;
