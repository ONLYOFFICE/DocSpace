import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import { tablet } from "@docspace/components/utils/device";

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
      !props.isInfoPanelVisible
        ? "none"
        : props.theme.infoPanel.sectionHeaderToggleBgActive};

    path {
      fill: ${(props) => props.theme.infoPanel.sectionHeaderToggleIconActive};
    }
  }
`;

const getDefaultStyles = ({ $currentColorScheme, isInfoPanelVisible }) =>
  $currentColorScheme &&
  isInfoPanelVisible &&
  css`
    .info-panel-toggle-bg {
      path {
        fill: ${$currentColorScheme.main.accent};
      }
      &:hover {
        path {
          fill: ${$currentColorScheme.main.accent};
        }
      }
    }
  `;

StyledInfoPanelToggleWrapper.defaultProps = { theme: Base };

export default styled(StyledInfoPanelToggleWrapper)(getDefaultStyles);
