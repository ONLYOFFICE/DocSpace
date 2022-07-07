import styled, { css } from "styled-components";
import ModalDialog from "@appserver/components/modal-dialog";

const StyledDownloadDialog = styled(ModalDialog)`
  .modal-dialog-content {
    padding: 0 0 0 16px;
  }

  .download-dialog-description {
    margin-bottom: 18px;
  }

  .modal-dialog-aside-footer {
    display: flex;
    padding-right: 32px;
    width: 100%;
    gap: 10px;
  }
`;

const StyledDownloadContent = styled.div`
  .download-dialog_row {
    display: flex;
    align-items: center;
    height: 48px;

    .download-dialog-other-text {
      min-width: 141px;
    }
  }

  .download-dialog-checkbox {
    padding: 8px 6px 8px 8px;
  }

  .download-dialog_row-text {
    width: 100%;
    margin-right: 10px;
  }

  .download-dialog-heading {
    display: flex;
    align-items: center;
    gap: 8px;
    cursor: pointer;
  }

  .download-dialog_content-wrapper {
    height: 48px;
    display: flex;
    align-items: center;

    ${({ isOpen }) =>
      isOpen &&
      css`
        background: ${(props) => props.theme.downloadDialog.background};
        margin: 0 -16px;
        padding: 0 16px;
      `}

    .download-dialog-icon {
      width: 12px;
      height: 12px;
      transform: ${({ isOpen }) =>
        isOpen ? "rotate(270deg)" : "rotate(90deg)"};

      svg {
        path {
          fill: #333;
        }
      }
    }
  }

  .download-dialog_hidden-items {
    display: ${({ isOpen }) => (isOpen ? "block" : "none")};
  }
`;

export { StyledDownloadDialog, StyledDownloadContent };
