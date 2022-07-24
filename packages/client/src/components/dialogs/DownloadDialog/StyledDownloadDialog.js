import styled, { css } from "styled-components";
import ModalDialog from "@docspace/components/modal-dialog";

const StyledDownloadDialog = styled(ModalDialog)`
  .download-dialog-description {
    margin-bottom: 16px;
  }

  .download-dialog-convert-message {
    margin-top: 16px;
  }
`;

const StyledDownloadContent = styled.div`
  .download-dialog_content-wrapper {
    ${({ isOpen }) =>
      isOpen &&
      css`
        background: ${(props) => props.theme.downloadDialog.background};
        margin: 0 -16px;
        padding: 0 16px;
      `}

    .download-dialog-heading {
      display: flex;
      align-items: center;
      gap: 8px;
      cursor: pointer;
    }

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

  .download-dialog-row {
    max-width: 100%;
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 12px;
    height: 48px;

    .download-dialog-main-content {
      min-width: 0;
      display: flex;
      flex-direction: row;
      align-items: center;
      justify-content: start;
      width: 100%;

      .checkbox,
      svg {
        margin: 0 !important;
      }
      .download-dialog-checkbox {
        padding: 8px;
      }
      .download-dialog-icon-contatiner {
        padding: 0 8px;
        max-height: 32px;
        max-width: 32px;
      }
      .download-dialog-title {
        min-width: 0;
        width: 100%;
      }
    }

    .download-dialog-actions {
      .download-dialog-link {
        a {
          padding-right: 0;
          text-underline-offset: 1px;
        }
      }
      .download-dialog-other-text {
        text-align: end;
        color: #a3a9ae;
      }
    }
  }
`;

export { StyledDownloadDialog, StyledDownloadContent };
