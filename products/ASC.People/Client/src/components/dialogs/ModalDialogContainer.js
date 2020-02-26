import styled from "styled-components";

const ModalDialogContainer = styled.div`
  .flex {
    display: flex;
    justify-content: space-between;
  }

  .text-dialog {
    margin: 16px 0;
  }

  .input-dialog {
    margin-top: 16px;
  }

  .button-dialog {
    margin-left: 8px;
  }

  .warning-text {
    margin: 20px 0;
  }

  .textarea-dialog {
    margin-top: 12px;
  }

  .link-dialog {
    margin-right: 12px;
  }

  .error-label {
    position: absolute;
    max-width: 100%;
  }

  .field-body {
    position: relative;
  }

  .toggle-content-dialog {
    .heading-toggle-content {
      font-size: 16px;
    }

    .modal-dialog-scrollbar {
      height: 250px !important;
      border: 1px solid lightgray;

      .modal-dialog-content {
        padding: 8px 16px;

        .modal-dialog-checkbox:not(:last-child) {
          padding-bottom: 4px;
        }
      }
    }
  }
`;

export default ModalDialogContainer;
