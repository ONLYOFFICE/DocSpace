import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, mobile } from "../utils/device";
import Base from "../themes/base";

const styledTabletView = css`
  width: ${(props) => props.theme.newContextMenu.devices.tabletWidth};
  max-width: ${(props) => props.theme.newContextMenu.devices.tabletWidth};
  max-height: ${(props) => props.theme.newContextMenu.devices.maxHeight};
  left: ${(props) => props.theme.newContextMenu.devices.left};
  right: ${(props) => props.theme.newContextMenu.devices.right};
  bottom: ${(props) => props.theme.newContextMenu.devices.bottom};
  margin: ${(props) => props.theme.newContextMenu.devices.margin};
`;

const styledMobileView = css`
  width: ${(props) => props.theme.newContextMenu.devices.mobileWidth};
  max-width: ${(props) => props.theme.newContextMenu.devices.mobileWidth};
  max-height: ${(props) => props.theme.newContextMenu.devices.maxHeight};
  left: ${(props) => props.theme.newContextMenu.devices.left};
  bottom: ${(props) => props.theme.newContextMenu.devices.bottom};
`;

const StyledContextMenu = styled.div`
  .p-contextmenu {
    position: absolute;
    background: ${(props) => props.theme.newContextMenu.background};
    border: ${(props) => props.theme.newContextMenu.border};
    border-radius: ${(props) => props.theme.newContextMenu.borderRadius};
    -moz-border-radius: ${(props) => props.theme.newContextMenu.borderRadius};
    -webkit-border-radius: ${(props) =>
      props.theme.newContextMenu.borderRadius};
    box-shadow: ${(props) => props.theme.newContextMenu.boxShadow};
    -moz-box-shadow: ${(props) => props.theme.newContextMenu.boxShadow};
    -webkit-box-shadow: ${(props) => props.theme.newContextMenu.boxShadow};
    padding: ${(props) => props.theme.newContextMenu.padding};

    @media ${tablet} {
      ${(props) => props.changeView && !isMobile && styledTabletView}
    }

    @media ${mobile} {
      ${(props) => props.changeView && !isMobile && styledMobileView}
    }

    ${(props) =>
      props.changeView
        ? isMobileOnly
          ? styledMobileView
          : styledTabletView
        : null}
  }
`;

StyledContextMenu.defaultProps = {
  theme: Base,
};

export default StyledContextMenu;
