import styled from "styled-components";
import Base from "@docspace/components/themes/base";
import ModalDialog from "@docspace/components/modal-dialog";

const ModalDialogContainer = styled(ModalDialog)`
  .toggle-btn {
    position: relative;
    align-items: center;
    height: 20px;
    grid-gap: 12px !important;
  }
  .toggle-btn_next {
    margin-top: 12px;
  }
  button {
    max-width: fit-content;
  }
  .subscription-container {
    margin-bottom: 24px;
    .subscription-title {
      margin-bottom: 14px;
    }
    .subscription_click-text {
      text-decoration: underline dashed;
      cursor: pointer;
    }
  }
`;

ModalDialogContainer.defaultProps = { theme: Base };

export default ModalDialogContainer;
