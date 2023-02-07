import React from "react";

import IconButton from "../../../icon-button";
import Heading from "../../../heading";

import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";

import { StyledSelectorHeader } from "../../StyledSelector";

const Header = React.memo(({ onBackClickAction, headerLabel }) => {
  return (
    <StyledSelectorHeader>
      <IconButton
        className="arrow-button"
        iconName={ArrowPathReactSvgUrl}
        size={17}
        onClick={onBackClickAction}
      />
      <Heading className={"heading-text"}>{headerLabel}</Heading>
    </StyledSelectorHeader>
  );
});

export default Header;
