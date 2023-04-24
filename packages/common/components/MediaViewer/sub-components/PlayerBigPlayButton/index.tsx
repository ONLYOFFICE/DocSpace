import React from "react";
import BigIconPlay from "PUBLIC_DIR/images/media.bgplay.react.svg";
import WrapperPlayerBigPlayButton from "./PlayerBigPlayButton.styled";
import PlayerBigPlayButtonProps from "./PlayerBigPlayButton.props";

function PlayerBigPlayButton({ visible, onClick }: PlayerBigPlayButtonProps) {
  if (!visible) return <></>;

  return (
    <WrapperPlayerBigPlayButton>
      <BigIconPlay onClick={onClick} />
    </WrapperPlayerBigPlayButton>
  );
}

export default PlayerBigPlayButton;
