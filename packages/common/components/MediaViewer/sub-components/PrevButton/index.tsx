import React from "react";

import {
  StyledButtonScroll,
  StyledSwitchToolbar,
} from "../../StyledComponents";
import MediaPrevIcon from "PUBLIC_DIR/images/viewer.prew.react.svg";

type PrevButtonProps = {
  prevClick: VoidFunction;
};

function PrevButton({ prevClick }: PrevButtonProps) {
  return (
    <StyledSwitchToolbar left onClick={prevClick}>
      <StyledButtonScroll orientation="left">
        <MediaPrevIcon />
      </StyledButtonScroll>
    </StyledSwitchToolbar>
  );
}

export default PrevButton;
