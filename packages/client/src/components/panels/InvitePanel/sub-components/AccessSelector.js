import React from "react";
import AccessRightSelect from "@docspace/components/access-right-select";
import { getAccessOptions } from "../utils";

const AccessSelector = ({ t, roomType }) => {
  const accessOptions = getAccessOptions(t, roomType);
  return (
    <AccessRightSelect
      selectedOption={accessOptions[0]}
      options={accessOptions}
    />
  );
};

export default AccessSelector;
