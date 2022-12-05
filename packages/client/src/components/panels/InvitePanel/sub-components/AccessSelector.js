import React from "react";
import AccessRightSelect from "@docspace/components/access-right-select";
import { getAccessOptions } from "../utils";
import { isMobileOnly } from "react-device-detect";

import { StyledAccessSelector } from "../StyledInvitePanel";

const AccessSelector = ({
  t,
  roomType,
  onSelectAccess,
  containerRef,
  defaultAccess,
  isOwner,
}) => {
  const width = containerRef?.current?.offsetWidth - 32;

  const accessOptions = getAccessOptions(t, roomType, false, true, isOwner);

  const selectedOption = accessOptions.filter(
    (access) => access.access === +defaultAccess
  )[0];

  return (
    <StyledAccessSelector>
      <AccessRightSelect
        selectedOption={selectedOption}
        onSelect={onSelectAccess}
        accessOptions={accessOptions}
        noBorder={false}
        directionX="right"
        directionY="bottom"
        fixedDirection={true}
        manualWidth={width + "px"}
        isDefaultMode={false}
        withBackdrop={isMobileOnly}
        isAside={true}
        withBackground={isMobileOnly}
      />
    </StyledAccessSelector>
  );
};

export default AccessSelector;
