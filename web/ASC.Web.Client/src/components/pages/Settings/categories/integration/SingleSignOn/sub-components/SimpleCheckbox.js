import React from "react";
import { inject, observer } from "mobx-react";

import Checkbox from "@appserver/components/checkbox";

const SimpleCheckbox = (props) => {
  const {
    label,
    name,
    tabIndex,
    isChecked,
    enableSso,
    onCheckboxChange,
  } = props;

  return (
    <Checkbox
      className="checkbox-input"
      isDisabled={!enableSso}
      isChecked={isChecked}
      label={label}
      name={name}
      onChange={onCheckboxChange}
      tabIndex={tabIndex}
    />
  );
};

export default inject(({ ssoStore }) => {
  const { enableSso, onCheckboxChange } = ssoStore;

  return {
    enableSso,
    onCheckboxChange,
  };
})(observer(SimpleCheckbox));
