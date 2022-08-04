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
    props.isRootFolder ? "auto 1fr" : "29px auto 1fr"};

  padding: ${(props) => (props.isDropBox ? "10px 0 5px" : "10px 0 11px")};

  .arrow-button {
    width: 17px;
    min-width: 17px;
  }

  .headline-heading {
    display: flex;
    height: 32px;
    align-items: center;
  }

  @media ${tablet} {
    width: 100%;
    grid-template-columns: ${(props) =>
      props.isRootFolder ? "auto 1fr" : "29px 1fr auto"};
    padding: ${(props) => (props.isDropBox ? "14px 0 5px" : "14px 0 15px")};
  }

  ${isMobile &&
  css`
    width: 100%;
    grid-template-columns: ${(props) =>
      props.isRootFolder ? "auto 1fr" : "29px 1fr auto"};
    padding: ${(props) => (props.isDropBox ? "14px 0 5px" : "14px 0 15px")};
  `}

  @media ${desktop} {
    padding: ${(props) => (props.isDropBox ? "10px 0 5px" : "10px 0 11px")};
  }

  @media ${mobile}, ${hugeMobile} {
    padding: ${(props) =>
      props.isDropBox ? "10px 0 5px" : "10px 0 11px"} !important;
  }
`;

export default StyledContainer;
