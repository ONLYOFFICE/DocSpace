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
    isLoadingXml,
  } = props;

  return (
    <Checkbox
      className="checkbox-input"
      isDisabled={!enableSso || isLoadingXml}
      isChecked={isChecked}
      label={label}
      name={name}
      onChange={setCheckbox}
      tabIndex={tabIndex}
    />
  );
};

export default inject(({ ssoStore }) => {
  const { enableSso, setCheckbox, isLoadingXml } = ssoStore;

  return {
    enableSso,
    setCheckbox,
    isLoadingXml,
  };
})(observer(SimpleCheckbox));
