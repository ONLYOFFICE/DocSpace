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
  grid-template-columns: ${(props) =>
    props.isRootFolder ? "auto 1fr" : "49px auto 1fr"};

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

  @media ${tablet} {
    width: 100%;
    grid-template-columns: ${(props) =>
      props.isRootFolder ? "1fr auto" : "49px 1fr auto"};
  }

  @media ${mobile} {
    grid-template-columns: ${(props) =>
      props.isRootFolder ? "1fr auto" : "29px 1fr auto"};
  }
`;

export default StyledContainer;
