import React from "react";

import {
  StyledButtonScroll,
  StyledSwitchToolbar,
} from "../../StyledComponents";
import MediaNextIcon from "PUBLIC_DIR/images/viewer.next.react.svg";

type NextButtonProps = {
  nextClick: VoidFunction;
};

function NextButton({ nextClick }: NextButtonProps) {
  return (
    <StyledSwitchToolbar onClick={nextClick}>
      <StyledButtonScroll orientation="right">
        <MediaNextIcon />
      </StyledButtonScroll>
    </StyledSwitchToolbar>
  );
}

export default NextButton;
