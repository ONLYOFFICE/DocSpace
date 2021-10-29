import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, mobile } from "../utils/device";
import Base from "../themes/base";

const styledTabletView = css`
  width: 375px;
  max-width: 375px;
  max-height: calc(100vh - 64px);
  left: 0;
  right: 0;
  bottom: 0;
  margin: 0 auto;
`;

const styledMobileView = css`
  width: 100vw;
  max-width: 100vw;
  max-height: calc(100vh - 64px);
  left: 0;
  bottom: 0;
`;

const StyledContextMenu = styled.div`
  .p-contextmenu {
    position: absolute;
    background: #ffffff;
    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
    -moz-box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
    -webkit-box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
    padding: 6px 0px;

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
