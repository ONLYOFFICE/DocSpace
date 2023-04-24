import React from "react";
import { inject, observer } from "mobx-react";

import FileInput from "@docspace/components/file-input";

const LocalFile = ({ setRestoreResource, isEnableRestore, t }) => {
  const onClickInput = (file) => {
    let data = new FormData();
    data.append("file", file);

    setRestoreResource(data);
  };

  return (
    <FileInput
      onInput={onClickInput}
      scale
      className="restore-backup_input"
      isDisabled={!isEnableRestore}
    />
  );
};

export default inject(({ auth, backup }) => {
  const { currentQuotaStore } = auth;
  const { setRestoreResource } = backup;
  const { isRestoreAndAutoBackupAvailable } = currentQuotaStore;

  return {
    isEnableRestore: isRestoreAndAutoBackupAvailable,
    setRestoreResource,
  };
})(observer(LocalFile));
