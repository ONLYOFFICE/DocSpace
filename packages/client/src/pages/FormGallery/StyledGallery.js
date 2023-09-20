import styled, { css } from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, mobile } from "@docspace/components/utils/device";
import Headline from "@docspace/common/components/Headline";
import { Base } from "@docspace/components/themes";
import { Button } from "@docspace/components";

const StyledHeadline = styled(Headline)`
  width: 100%;
  font-weight: 700;
  font-size: ${isMobile ? "21px !important" : "18px"};
  line-height: ${isMobile ? "28px !important" : "24px"};
  @media ${tablet} {
    font-size: 21px;
    line-height: 28px;
  }
`;

const StyledContainer = styled.div`
  width: 100%;
  height: 32px;
  display: grid;

  grid-template-columns: ${(props) =>
    props.isRootFolder ? "1fr auto auto" : "29px 1fr auto auto"};

  align-items: center;

  .arrow-button {
    width: 17px;
    min-width: 17px;

    ${({ theme }) =>
      theme.interfaceDirection === "rtl" && "transform: scaleX(-1);"}
  }

  @media ${tablet} {
    width: 100%;
    padding: 16px 0 0px;
  }

  ${isMobile &&
  css`
    width: 100% !important;
    padding: 16px 0 0px;
  `}

  @media ${mobile} {
    width: 100%;
    padding: 12px 0 0;
  }

  ${isMobileOnly &&
  css`
    width: 100% !important;
    padding: 12px 0 0;
  `}
`;

const StyledSubmitToGalleryButton = styled(Button)`
  margin-left: auto;
  ${(props) =>
    props.theme.interfaceDirection === "ltr"
      ? css`
          margin-right: 12px;
        `
      : css`
          margin-left: 12px;
        `}
`;
StyledSubmitToGalleryButton.defaultProps = { theme: Base };

const StyledInfoPanelToggleWrapper = styled.div`
  ${(props) =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-right: auto;
        `
      : css`
          margin-left: auto;
        `}

  display: ${(props) => (props.isInfoPanelVisible ? "none" : "flex")};
  align-items: center;
  justify-content: center;

  @media ${tablet} {
    display: none;
  }

  .info-panel-toggle-bg {
    height: 32px;
    width: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    margin-bottom: 1px;

    background-color: ${(props) =>
      props.isInfoPanelVisible
        ? props.theme.infoPanel.sectionHeaderToggleBgActive
        : props.theme.infoPanel.sectionHeaderToggleBg};

    svg {
      ${({ theme }) =>
        theme.interfaceDirection === "rtl" && "transform: scaleX(-1);"}
    }

    path {
      fill: ${(props) =>
        props.isInfoPanelVisible
          ? props.theme.infoPanel.sectionHeaderToggleIconActive
          : props.theme.infoPanel.sectionHeaderToggleIcon};
    }
  }
`;
StyledInfoPanelToggleWrapper.defaultProps = { theme: Base };

export {
  StyledHeadline,
  StyledContainer,
  StyledSubmitToGalleryButton,
  StyledInfoPanelToggleWrapper,
};
