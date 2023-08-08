import styled, { css } from "styled-components";
import { isMobile, isMobileOnly, isTablet } from "react-device-detect";
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

    @media ${tablet} {
      .logo-icon_svg {
        display: none;
      }
    }

    .header_separator {
      display: ${({ isRootFolder }) => (isRootFolder ? "block" : "none")};
      border-left: 1px solid #dfe2e3;
      margin: 0 0 0 15px;
      height: 21px;
    }

    .header-burger {
      cursor: pointer;
      display: none;
      margin-top: -2px;

      img {
        height: 28px;
        width: 28px;
      }

      @media ${tablet} {
        display: flex;
      }

      ${isTablet &&
      css`
        display: flex;
      `}

      @media ${mobile} {
        display: none;
      }

      ${isMobileOnly &&
      css`
        display: none !important;
      `}
    }
  }

  .drop-box-logo {
    display: none;

    @media ${tablet} {
      display: grid;
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
    padding-top: 2px;
    width: 17px;
    min-width: 17px;
  }

  .title-container {
    display: grid;
    grid-template-columns: minmax(1px, max-content) auto;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .navigation-header-separator {
    display: ${isMobileOnly ? "none" : "block"};
    padding-left: 16px;
    border-right: ${(props) =>
      `1px solid ${props.theme.navigation.icon.stroke}`};
    height: 21px;
    @media ${mobile} {
      display: none;
    }
  }

  .headline-heading {
    display: flex;
    height: 32px;
    align-items: center;
  }

  .title-block {
    display: flex;
    align-items: center;
    flex-direction: row;
    position: relative;
    cursor: pointer;
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
    gap: 8px;

    .title-icon {
      min-width: 16px;
      min-height: 16px;
    }
  }

  @media ${tablet} {
    width: 100%;
    grid-template-columns: ${({ isRootFolder, withLogo }) =>
      isRootFolder
        ? withLogo
          ? "59px 1fr auto"
          : "1fr auto"
        : withLogo
        ? "43px 49px 1fr auto"
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
