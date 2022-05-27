import styled from "styled-components";
import { tablet } from "@appserver/components/utils/device";
import ModalDialog from "@appserver/components/modal-dialog";
import Base from "@appserver/components/themes/base";

const ModalDialogContainer = styled(ModalDialog)`
  .invite-link-dialog-wrapper {
    display: flex;

    @media ${tablet} {
      display: grid;
      grid-gap: 8px;
      grid-template-columns: auto;
    }
  }

  .text-dialog {
    margin: 16px 0;
  }

  .text-body {
    margin-bottom: ${(props) => (props.isTabletView ? "16px" : "12px")};
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
    transition: opacity 0.2s;
    margin-right: 12px;
    opacity: ${(props) => (props.ChangeTextAnim ? 0 : 1)};
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

    .modal-dialog-content {
      padding: 8px 16px;
      border: ${(props) => props.theme.peopleDialogs.modal.border};

      .modal-dialog-checkbox:not(:last-child) {
        padding-bottom: 4px;
      }
    }
  }

  .heading {
    max-width: calc(100% - 32px);
  }
`;

ModalDialogContainer.defaultProps = { theme: Base };

export default ModalDialogContainer;
