import React, { memo } from "react";

import IconPlay from "PUBLIC_DIR/images/videoplayer.play.react.svg";
import IconStop from "PUBLIC_DIR/images/videoplayer.stop.react.svg";
import styled from "styled-components";

type PlayerPlayButtonProps = {
  isPlaying: boolean;
  onClick: VoidFunction;
};

const WrapperPlayerPlayButton = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  width: 48px;
  height: 48px;

  margin-left: -10px;

  cursor: pointer;
`;

function PlayerPlayButton({ isPlaying, onClick }: PlayerPlayButtonProps) {
  const onTouchStart = (event: React.TouchEvent<HTMLDivElement>) => {
    event.stopPropagation();
  };
  return (
    <WrapperPlayerPlayButton onClick={onClick} onTouchStart={onTouchStart}>
      {isPlaying ? <IconStop /> : <IconPlay />}
    </WrapperPlayerPlayButton>
  );
}

export default memo(PlayerPlayButton);
