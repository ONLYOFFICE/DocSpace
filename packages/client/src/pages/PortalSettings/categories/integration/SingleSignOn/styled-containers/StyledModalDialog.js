import styled from "styled-components";

import ModalDialog from "@docspace/components/modal-dialog";

const StyledModalDialog = styled(ModalDialog)`
  .generate {
    font-weight: 600;
  }

  .text-area {
    margin-top: 4px;
    margin-bottom: 16px;

    &-label {
      font-weight: 600;
      margin-bottom: 5px;
    }
  }

  .field-label-icon {
    margin-bottom: 5px;
  }

  .ok-button {
    padding: 8px 26px;
    margin-right: 10px;
  }
`;

export default StyledModalDialog;
