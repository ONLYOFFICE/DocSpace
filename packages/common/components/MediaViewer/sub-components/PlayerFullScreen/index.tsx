import React, { memo } from "react";
import PlayerFullSceenProps from "./PlayerFullScreen.props";

import { PlayerFullSceenWrapper } from "./PlayerFullScreen.styled";

import IconFullScreen from "PUBLIC_DIR/images/videoplayer.full.react.svg";
import IconExitFullScreen from "PUBLIC_DIR/images/videoplayer.exit.react.svg";

function PlayerFullScreen({
  isAudio,
  onClick,
  isFullScreen,
}: PlayerFullSceenProps) {
  if (isAudio) return <></>;

  return (
    <PlayerFullSceenWrapper onClick={onClick}>
      {isFullScreen ? <IconExitFullScreen /> : <IconFullScreen />}
    </PlayerFullSceenWrapper>
  );
}

export default memo(PlayerFullScreen);
