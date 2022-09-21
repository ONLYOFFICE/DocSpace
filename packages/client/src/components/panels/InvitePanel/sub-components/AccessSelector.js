import React from "react";
import AccessRightSelect from "@docspace/components/access-right-select";
import { getAccessOptions } from "../utils";

import { StyledAccessSelector } from "../StyledInvitePanel";

const AccessSelector = ({
  t,
  roomType,
  onSelectAccess,
  containerRef,
  defaultAccess,
}) => {
  const width = containerRef?.current?.offsetWidth - 32;
  const accessOptions = getAccessOptions(t, roomType, false, true);

  const selectedOption = accessOptions.filter(
    (access) => access.access === defaultAccess
  )[0];

  return (
    <StyledAccessSelector>
      <AccessRightSelect
        selectedOption={selectedOption}
        onSelect={onSelectAccess}
        accessOptions={accessOptions}
        noBorder={false}
        directionX="right"
        manualWidth={width + "px"}
        isDefaultMode={false}
      />
    </StyledAccessSelector>
  );
};

export default AccessSelector;
