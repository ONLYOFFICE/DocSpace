import styled from "styled-components";
import { utils } from "asc-web-components";

const tablet = utils.device.tablet;

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
    display: inline-block;
    margin-left: 8px;

    @media ${tablet} {
      display: none;
    }
  }

  .button-dialog-accept {
    @media ${tablet} {
      width: 100%;
    }
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
  }

  .delete_dialog-header-text {
    padding-bottom: 8px;
  }

  .delete_dialog-text {
    padding-top: 8px;
  }

  .modal-dialog-content {
    padding: 8px 16px;
    border: 1px solid lightgray;

    @media ${tablet} {
      padding: 0;
      border: 0;
    }

    .modal-dialog-checkbox:not(:last-child) {
      padding-bottom: 4px;
    }
  }

  .convert_dialog_content {
    display: flex;
    padding: 24px 0;

    .convert_dialog_image {
      display: block;
      @media ${tablet} {
        display: none;
      }
    }

    .convert_dialog-content {
      padding-left: 16px;

      @media ${tablet} {
        padding: 0;
        white-space: normal;
      }

      .convert_dialog_checkbox {
        padding-top: 16px;
      }
    }
  }
  .convert_dialog_footer {
    display: flex;

    .convert_dialog_button {
      margin-left: auto;
      display: inline-block;

      @media ${tablet} {
        display: none;
      }
    }

    .convert_dialog_button-accept {
      @media ${tablet} {
        width: 100%;
      }
    }
  }

  .modal-dialog-aside-footer {
    @media ${tablet} {
      width: 90%;
    }
  }
`;

export default ModalDialogContainer;
