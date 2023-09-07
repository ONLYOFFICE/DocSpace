import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { FilesSelectorFilterTypes } from "@docspace/common/constants";

import FilesSelectorInput from "SRC_DIR/components/FilesSelectorInput";

const RoomsModule = (props) => {
  const { isEnableRestore, setRestoreResource } = props;

  const { t } = useTranslation("Settings");

  const onSelectFile = (file) => {
    setRestoreResource(file.id);
  };

  return (
    <FilesSelectorInput
      isDisabled={!isEnableRestore}
      onSelectFile={onSelectFile}
      filterParam={FilesSelectorFilterTypes.GZ}
      descriptionText={t("SelectFileInGZFormat")}
      isRoomsOnly
    />
  );
};

export default inject(({ auth, backup }) => {
  const { currentQuotaStore } = auth;
  const { setRestoreResource } = backup;
  const { isRestoreAndAutoBackupAvailable } = currentQuotaStore;

  return {
    setRestoreResource,
    isEnableRestore: isRestoreAndAutoBackupAvailable,
  };
})(observer(RoomsModule));
