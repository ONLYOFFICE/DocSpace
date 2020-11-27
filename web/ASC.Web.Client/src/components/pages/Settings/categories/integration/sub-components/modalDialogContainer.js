import styled from "styled-components";
import { tablet } from "@appserver/components/src/utils/device";

const ModalDialogContainer = styled.div`
  .modal-dialog-aside-footer {
    @media ${tablet} {
      width: 90%;
    }
  }
  .modal-dialog-button {
    @media ${tablet} {
      width: 100%;
    }
  }
  .field-body {
    margin-top: 16px;
  }
`;

export default ModalDialogContainer;
