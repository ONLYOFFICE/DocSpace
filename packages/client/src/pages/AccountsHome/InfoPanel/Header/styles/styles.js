import styled from "styled-components";
import { isMobile, isMobileOnly } from "react-device-detect";

import { Base } from "@docspace/components/themes";
import { tablet, mobile } from "@docspace/components/utils/device";

const StyledInfoPanelHeader = styled.div`
  width: 100%;
  max-width: 100%;
  box-sizing: border-box;

  height: 69px;
  min-height: 69px;

  @media ${tablet} {
    height: 53px;
    min-height: 53px;
  }

  ${isMobile &&
  css`
    height: 53px;
    min-height: 53px;
  `}

  @media ${mobile} {
    height: 53px;
    min-height: 53px;
  }

  ${isMobileOnly &&
  css`
    height: 53px;
    min-height: 53px;
  `}

  display: flex;
  justify-content: space-between;
  align-items: center;
  align-self: center;

  border-bottom: ${(props) => `1px solid ${props.theme.infoPanel.borderColor}`};

  .header-text {
    margin-left: 20px;
  }
`;

const StyledInfoPanelToggleWrapper = styled.div`
  display: flex;

  @media ${tablet} {
    display: none;
  }

  align-items: center;
  justify-content: center;
  padding-right: 20px;

  .info-panel-toggle-bg {
    height: 32px;
    width: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    background-color: ${(props) =>
      props.theme.infoPanel.sectionHeaderToggleBgActive};

    path {
      fill: ${(props) => props.theme.infoPanel.sectionHeaderToggleIconActive};
    }
  }
`;

StyledInfoPanelHeader.defaultProps = { theme: Base };
StyledInfoPanelToggleWrapper.defaultProps = { theme: Base };

export { StyledInfoPanelHeader, StyledInfoPanelToggleWrapper };
