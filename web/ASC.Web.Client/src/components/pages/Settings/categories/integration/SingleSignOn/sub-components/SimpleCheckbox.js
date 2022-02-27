import React from "react";
import { observer } from "mobx-react";

import Checkbox from "@appserver/components/checkbox";
import FormStore from "@appserver/studio/src/store/SsoFormStore";

const SimpleCheckbox = ({ label, name, tabIndex }) => {
  return (
    <Checkbox
      className="checkbox-input"
      isDisabled={!FormStore.enableSso}
      isChecked={FormStore[name]}
      label={label}
      name={name}
      onChange={FormStore.onCheckboxChange}
      tabIndex={tabIndex}
    />
  );
};

export default observer(SimpleCheckbox);
