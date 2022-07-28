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
    setCheckbox,
    onLoadXML,
  } = props;

  return (
    <Checkbox
      className="checkbox-input"
      isDisabled={!enableSso || onLoadXML}
      isChecked={isChecked}
      label={label}
      name={name}
      onChange={setCheckbox}
      tabIndex={tabIndex}
    />
  );
};

export default inject(({ ssoStore }) => {
  const { enableSso, setCheckbox, onLoadXML } = ssoStore;

  return {
    enableSso,
    setCheckbox,
    onLoadXML,
  };
})(observer(SimpleCheckbox));
