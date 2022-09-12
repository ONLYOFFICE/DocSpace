import React from "react";
import AccessRightSelect from "@docspace/components/access-right-select";
import { getAccessOptions } from "../utils";

import { StyledAccessSelector } from "../StyledInvitePanel";

const AccessSelector = ({ t, roomType, onSelectAccess, containerRef }) => {
  const width = containerRef?.current?.offsetWidth - 32;
  const accessOptions = getAccessOptions(t, roomType, false, true);
  return (
    <StyledAccessSelector>
      <AccessRightSelect
        selectedOption={accessOptions[0]}
        onSelect={onSelectAccess}
        accessOptions={accessOptions}
        noBorder={false}
        directionX="right"
        manualWidth={width + "px"}
      />
    </StyledAccessSelector>
  );
};

export default AccessSelector;
