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
    !props.isDropBox &&
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

  .navigation-arrow-container {
    display: flex;
  }

  .arrow-button {
    width: 17px;
    min-width: 17px;
  }

  .navigation-header-separator {
    display: block;
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
    grid-template-columns: ${(props) =>
      props.isRootFolder ? "auto 1fr" : "49px 1fr auto"};
  }

  @media ${mobile} {
    grid-template-columns: ${(props) =>
      props.isRootFolder ? "auto 1fr" : "29px 1fr auto"};
  }

  ${isMobile &&
  css`
    width: 100%;
    grid-template-columns: ${(props) =>
      props.isRootFolder ? "auto 1fr" : "49px 1fr auto"};
    display: flex;
    align-items: center;
    justify-content: space-between;
  `}
`;

export default StyledContainer;
