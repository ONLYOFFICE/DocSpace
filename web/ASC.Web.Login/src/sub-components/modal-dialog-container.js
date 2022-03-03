import styled from "styled-components";
import ModalDialog from "@appserver/components/modal-dialog";

const ModalDialogContainer = styled(ModalDialog)`
  .modal-dialog-aside {
    padding: 0;
  }

  .modal-dialog-aside-body {
    padding: 0;
    margin: 0;
  }

  .modal-dialog-aside-footer {
    @media (max-width: 1024px) {
      width: 90%;
    }
  }
  .modal-dialog-button {
    display: inline-block;
    margin-right: 10px;

    @media (max-width: 1024px) {
      margin-right: 8px;
    }
  }
  .field-body {
    margin-top: 16px;
  }
`;

export default ModalDialogContainer;
