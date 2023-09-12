import styled from "styled-components";

import ModalDialog from "@docspace/components/modal-dialog";
import { smallTablet } from "@docspace/components/utils/device";

const StyledModalDialog = styled(ModalDialog)`
  .heading {
    font-size: 21px;
  }

  .generate {
    font-weight: 600;
  }

  .text-area {
    width: 488px !important;
    height: 72px !important;
    margin-top: 4px;

    &-label {
      font-weight: 600;
      margin-bottom: 5px;
    }

    @media ${smallTablet} {
      width: 100% !important;
    }
  }
  .text-area-label {
    margin-top: 16px;
  }

  .modal-combo {
    margin: 16px 0 0 0;
  }
`;

export default StyledModalDialog;
