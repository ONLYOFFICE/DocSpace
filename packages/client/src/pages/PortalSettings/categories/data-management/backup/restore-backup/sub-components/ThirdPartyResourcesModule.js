import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import DirectThirdPartyConnection from "../../common-container/DirectThirdPartyConnection";
import { FilesSelectorFilterTypes } from "@docspace/common/constants";

const ThirdPartyResources = (props) => {
  const { setRestoreResource, buttonSize } = props;

  const { t } = useTranslation("Settings");

  const onSelectFile = (file) => {
    setRestoreResource(file.id);
  };

  return (
    <div className="restore-backup_third-party-module">
      <DirectThirdPartyConnection
        onSelectFile={onSelectFile}
        filterParam={FilesSelectorFilterTypes.GZ}
        descriptionText={t("SelectFileInGZFormat")}
        withoutInitPath
        buttonSize={buttonSize}
      />
    </div>
  );
};

export default inject(({ backup }) => {
  const { setRestoreResource } = backup;

  return {
    setRestoreResource,
  };
})(observer(ThirdPartyResources));
