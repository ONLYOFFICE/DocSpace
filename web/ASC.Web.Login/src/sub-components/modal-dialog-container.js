import styled from "styled-components";
import ModalDialog from "@appserver/components/modal-dialog";

const ModalDialogContainer = styled(ModalDialog)`
  .modal-dialog-aside {
    padding: 0;
    transform: translateX(${(props) => (props.visible ? "0" : "480px")});
    width: 480px;

    @media (max-width: 550px) {
      width: 320px;
      transform: translateX(${(props) => (props.visible ? "0" : "320px")});
    }
  }

  .header_aside-panel {
    transition: unset;
    transform: translateX(${(props) => (props.visible ? "0" : "480px")});
    width: 480px;
    overflow-y: hidden;

    @media (max-width: 550px) {
      width: 320px;
      transform: translateX(${(props) => (props.visible ? "0" : "320px")});
    }
  }

  .modal-dialog-aside-footer {
    @media (max-width: 1024px) {
      width: 90%;
    }
  }
  .modal-dialog-button {
    @media (max-width: 1024px) {
      width: 100%;
    }
  }
  .field-body {
    margin-top: 16px;
  }
`;

export default ModalDialogContainer;
