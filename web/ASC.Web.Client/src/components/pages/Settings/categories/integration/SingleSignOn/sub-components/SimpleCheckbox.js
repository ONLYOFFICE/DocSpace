import React from "react";
import Checkbox from "@appserver/components/checkbox";
import { observer } from "mobx-react";

const SimpleCheckbox = ({ FormStore, label, name, tabIndex }) => {
  return (
    <Checkbox
      className="checkbox-input"
      isChecked={FormStore[name]}
      label={label}
      name={name}
      onChange={FormStore.onCheckboxChange}
      tabIndex={tabIndex}
    />
  );
};

export default observer(SimpleCheckbox);
