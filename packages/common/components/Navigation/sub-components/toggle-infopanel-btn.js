import React from "react";
import styled, { css } from "styled-components";
import PanelReactSvgUrl from "PUBLIC_DIR/images/panel.react.svg?url";
import IconButton from "@docspace/components/icon-button";
import { tablet } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const StyledInfoPanelToggleColorThemeWrapper = styled(ColorTheme)`
  align-self: center;
  margin-left: auto;

  margin-bottom: 1px;
  padding: 0;

  @media ${tablet} {
    display: none;
    margin-left: ${(props) => (props.isRootFolder ? "auto" : "0")};
  }
`;
StyledInfoPanelToggleColorThemeWrapper.defaultProps = { theme: Base };

const ToggleInfoPanelButton = ({
  isRootFolder,
  isInfoPanelVisible,
  toggleInfoPanel,
  id,
  titles,
}) => {
  return (
    <StyledInfoPanelToggleColorThemeWrapper
      isRootFolder={isRootFolder}
      themeId={ThemeType.InfoPanelToggle}
      isInfoPanelVisible={isInfoPanelVisible}
    >
      <div className="info-panel-toggle-bg">
        <IconButton
          id={id}
          className="info-panel-toggle"
          iconName={PanelReactSvgUrl}
          size="16"
          isFill={true}
          onClick={toggleInfoPanel}
          title={titles?.infoPanel}
        />
      </div>
    </StyledInfoPanelToggleColorThemeWrapper>
  );
};

export default ToggleInfoPanelButton;
