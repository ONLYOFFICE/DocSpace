import React from "react";

import Heading from "@docspace/components/heading";
import IconButton from "@docspace/components/icon-button";

import ArrowPathReactSvgUrl from "../../../../../public/images/arrow.path.react.svg?url";

const Header = ({ headerLabel, onArrowClickAction }) => {
  return (
    <div className="header">
      <IconButton
        iconName={ArrowPathReactSvgUrl}
        size="17"
        isFill={true}
        className="arrow-button"
        onClick={onArrowClickAction}
      />
      <Heading size="medium" truncate={true}>
        {headerLabel.replace("()", "")}
      </Heading>
    </div>
  );
};

export default React.memo(Header);
