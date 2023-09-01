import ModalDialog from "@docspace/components/modal-dialog";
import { tablet } from "@docspace/components/utils/device";
import styled from "styled-components";
import { getCorrectFourValuesStyle } from "@docspace/components/utils/rtlUtils";

const StyledMoveToPublicRoomDialog = styled(ModalDialog)`
  /* .scroll-body {
    padding-right: 0 !important;
  } */

  .modal-dialog-content-body {
    padding: 0;
    border: none;
  }

  .modal-dialog-aside-header {
    margin: ${({ theme }) =>
      getCorrectFourValuesStyle(
        "0 -24px 0 -16px",
        theme.interfaceDirection === "rtl"
      )};
    padding: ${({ theme }) =>
      getCorrectFourValuesStyle(
        "0 0 0 16px",
        theme.interfaceDirection === "rtl"
      )};
  }

  .modal-dialog-aside-footer {
    @media ${tablet} {
      width: 100%;
      ${({ theme }) =>
        theme.interfaceDirection === "rtl"
          ? `padding-left: 32px;`
          : `padding-right: 32px;`}
      display: flex;
    }
  }

  .button-dialog-accept {
    @media ${tablet} {
      width: 100%;
    }
  }

  .button-dialog {
    @media ${tablet} {
      width: 100%;
    }

    display: inline-block;
  }
`;

export { StyledMoveToPublicRoomDialog };
