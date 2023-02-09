import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import FileInput from "@docspace/components/file-input";
import TextInput from "@docspace/common/components/ColorTheme/styled/textInput";

let timerId = null;
const LocalFile = ({ standalone, setRestoreResource }) => {
  const [value, setValue] = useState("");

  const onClickInput = (file) => {
    let data = new FormData();
    data.append("file", file);

    setRestoreResource(data);
  };

  const onChange = (e) => {
    if (timerId) {
      timerId = null;
      clearTimeout(timerId);
    }

    const value = e.currentTarget.value;
    setValue(value);
    timerId = setTimeout(() => setRestoreResource(value), 1000);
  };

  const SaaSMode = (
    <FileInput
      onInput={onClickInput}
      scale
      className="restore-backup_input"
      webkitdirectory
    />
  );

  const EnterpriseMode = (
    <TextInput
      onChange={onChange}
      value={value}
      type="text"
      size="base"
      scale
      className="restore-backup_input"
      placeholder="Enter  path"
    />
  );

  return standalone ? EnterpriseMode : SaaSMode;
};

export default inject(({ auth, backup }) => {
  const { standalone } = auth.settingsStore;
  const { setRestoreResource } = backup;

  return {
    standalone,
    setRestoreResource,
  };
})(observer(LocalFile));
