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
    onLoadXML,
  } = props;

  return (
    <Checkbox
      className="checkbox-input"
      isDisabled={!enableSso || onLoadXML}
      isChecked={isChecked}
      label={label}
      name={name}
      onChange={onCheckboxChange}
      tabIndex={tabIndex}
    />
  );
};

export default inject(({ ssoStore }) => {
  const { enableSso, onCheckboxChange, onLoadXML } = ssoStore;

  return {
    enableSso,
    onCheckboxChange,
    onLoadXML,
  };
})(observer(SimpleCheckbox));
