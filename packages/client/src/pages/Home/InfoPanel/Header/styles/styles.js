import styled from "styled-components";
import { Base } from "@docspace/components/themes";
import { tablet } from "@docspace/components/utils/device";

const StyledInfoPanelHeader = styled.div`
  width: 100%;
  max-width: 100%;
  height: ${(props) => (props.isRoom ? "85px" : "52px")};
  min-height: ${(props) => (props.isRoom ? "85px" : "52px")};
  display: flex;
  flex-direction: column;
  border-bottom: ${(props) =>
    props.isRoom ? "none" : `1px solid ${props.theme.infoPanel.borderColor}`};
  .main {
    height: ${(props) => (props.isRoom ? "53px" : "52px")};
    min-height: ${(props) => (props.isRoom ? "53px" : "52px")};
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
    .header-text {
      margin-left: 20px;
    }
  }
  .submenu {
    display: flex;
    width: 100%;
    justify-content: center;
    .sticky {
      display: flex;
      flex-direction: column;
      align-items: center;
    }
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
