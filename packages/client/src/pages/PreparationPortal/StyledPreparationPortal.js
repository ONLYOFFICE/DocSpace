import styled from "styled-components";
import { isMobile } from "react-device-detect";

const StyledPreparationPortal = styled.div`
  width: 100%;
  ${isMobile &&
  `
    margin-top: 48px;
  `}

  #header {
    font-size: 23px;
  }
  #text {
    color: #a3a9ae;
    font-size: 13px;
    line-height: 20px;
    max-width: 480px;
  }

  .preparation-portal_body-wrapper {
    margin-bottom: 24px;
    width: 100%;
    max-width: ${(props) => (props.errorMessage ? "560px" : "480px")};
    box-sizing: border-box;
    align-items: center;
    .preparation-portal_error {
      text-align: center;
      color: ${(props) => props.theme.preparationPortalProgress.errorTextColor};
    }

    .preparation-portal_text {
      text-align: center;
      color: ${(props) => props.theme.text.disableColor};
    }
  }
`;

export { StyledPreparationPortal };
