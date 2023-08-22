import React from "react";

import IconButton from "../../../icon-button";
import Heading from "../../../heading";

import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";

import StyledHeader from "./StyledHeader";
import { HeaderProps } from "./Header.types";

const Header = React.memo(
  ({ onBackClickAction, withoutBackButton, headerLabel }: HeaderProps) => {
    return (
      <StyledHeader>
        {!withoutBackButton && (
          <IconButton
            className="arrow-button"
            iconName={ArrowPathReactSvgUrl}
            size={17}
            onClick={onBackClickAction}
          />
        )}
        <Heading className={"heading-text"}>{headerLabel}</Heading>
      </StyledHeader>
    );
  }
);

export default Header;
