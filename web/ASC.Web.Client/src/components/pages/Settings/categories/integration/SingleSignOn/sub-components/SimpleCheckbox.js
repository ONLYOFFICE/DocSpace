import React from "react";
import { observer } from "mobx-react";

import Checkbox from "@appserver/components/checkbox";

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
