import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";

import { Base } from "@docspace/components/themes";
import { hugeMobile, tablet } from "@docspace/components/utils/device";

const StyledDeepLinkWrapper = styled.div`
  width: 100%;
  height: 100vh;

  padding-top: 100px;

  box-sizing: border-box;

  display: flex;
  align-items: center;
  flex-direction: column;

  background-image: ${(props) => props.bgPattern};
  background-repeat: no-repeat;
  background-attachment: fixed;
  background-size: 100% 100%;

  @media ${tablet} {
    background-size: cover;
  }

  @media ${hugeMobile} {
    background-image: none;
  }

  ${isMobile &&
  css`
    background-size: cover !important;
  `}

  ${isMobileOnly &&
  css`
    background-image: none !important;
  `}

  .deep-link__logo {
    max-height: 44px;

    cursor: pointer;

    margin-bottom: 64px;
  }

  .deep-link__header {
    font-size: 23px;
    font-weight: 700;
    line-height: 28px;

    margin: 0;

    margin-bottom: 16px;
  }

  .deep-link__description {
    width: 386px;
    max-width: calc(100% - 32px);

    font-weight: 400;
    font-size: 16px;
    line-height: 22px;

    text-align: center;

    margin-bottom: 32px;
  }
`;

StyledDeepLinkWrapper.defaultProps = { theme: Base };

const StyledContentContainer = styled.div`
  width: 386px;
  max-width: calc(100% - 32px);

  padding: 32px;

  box-shadow: 0px 5px 20px rgba(4, 15, 27, 0.07);

  border-radius: 12px;

  box-sizing: border-box;

  display: flex;
  align-items: center;
  flex-direction: column;

  background: ${(props) => props.theme.backgroundColor};

  ${isMobile &&
  css`
    width: 480px;
  `}
  .deep-link-content__description {
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;

    margin-bottom: 32px;
  }

  .deep-link-content__button {
    margin-bottom: 24px;
  }

  .deep-link-content__without-app {
    font-weight: 600;
    font-size: 13px;
    line-height: 20px;

    margin-bottom: 8px;
  }

  .deep-link-content__download-now {
    font-weight: 600;
    font-size: 13px;
    line-height: 15px;

    color: #3b72a7;
  }
`;

StyledContentContainer.defaultProps = { theme: Base };

export { StyledDeepLinkWrapper, StyledContentContainer };
