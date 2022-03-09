import styled from "styled-components";
import { tablet } from "@appserver/components/utils/device";
import ModalDialog from "@appserver/components/modal-dialog";
import { Base } from "@appserver/components/themes";

const ModalDialogContainer = styled(ModalDialog)`
  .row-main-container-wrapper {
    width: 100%;
  }

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

  .link-other-formats {
    pointer-events: none;
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
    padding-bottom: 8px;
  }

  .modal-dialog-content {
    padding: 8px 16px;
    border: ${(props) => props.theme.filesModalDialog.border};

    @media ${tablet} {
      padding: 0;
      border: 0;
    }

    .modal-dialog-checkbox:not(:last-child) {
      padding-bottom: 4px;
    }

    .delete_dialog-text:not(:first-child) {
      padding-top: 8px;
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

      .convert_dialog_checkbox,
      .convert_dialog_file-destination {
        padding-top: 16px;
      }
      .convert_dialog_file-destination {
        opacity: 0;
      }
      .file-destination_visible {
        opacity: 1;
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

ModalDialogContainer.defaultProps = { theme: Base };

export default ModalDialogContainer;
