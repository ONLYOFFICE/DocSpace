import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";
import {
  tablet,
  mobile,
  desktop,
  hugeMobile,
} from "@docspace/components/utils/device";

const StyledContainer = styled.div`
  ${(props) =>
    !props.isDropBoxComponent &&
    props.isDesktop &&
    css`
      width: fit-content;
      max-width: ${props.isInfoPanelVisible
        ? `calc(100%)`
        : `calc(100% - 72px)`};
    `}

  display: grid;
  align-items: center;

  grid-template-columns: ${({ isRootFolder, withLogo }) =>
    isRootFolder
      ? withLogo
        ? "1fr auto 1fr"
        : "auto 1fr"
      : withLogo
      ? "1fr 49px auto 1fr"
      : "49px auto 1fr"};

  .navigation-logo {
    display: flex;
    height: 24px;
    margin-right: 16px;

    .header_separator {
      display: ${({ isRootFolder }) => (isRootFolder ? "block" : "none")};
      border-left: 1px solid #dfe2e3;
      margin: 0 0 0 15px;
      height: 21px;
    }
  }

  height: 100%;
  ${(props) =>
    props.isDesktopClient &&
    props.isDropBoxComponent &&
    css`
      max-height: 32px;
    `}

  .navigation-arrow-container {
    display: flex;
  }

  .arrow-button {
    width: 17px;
    min-width: 17px;
  }

  .navigation-header-separator {
    display: ${isMobileOnly ? "none" : "block"};
    padding-left: 16px;
    border-right: ${(props) =>
      `1px solid ${props.theme.navigation.icon.stroke}`};

    @media ${mobile} {
      display: none;
    }
  }

  .headline-heading {
    display: flex;
    height: 32px;
    align-items: center;
  }

  @media ${tablet} {
    width: 100%;
    grid-template-columns: ${({ isRootFolder, withLogo }) =>
      isRootFolder
        ? withLogo
          ? "242px 1fr auto"
          : "1fr auto"
        : withLogo
        ? "226px 49px 1fr auto"
        : "49px 1fr auto"};
  }

  @media ${mobile} {
    .navigation-logo {
      display: none;
    }

    grid-template-columns: ${(props) =>
      props.isRootFolder ? "1fr auto" : "29px 1fr auto"};
  }
`;

export default StyledContainer;
