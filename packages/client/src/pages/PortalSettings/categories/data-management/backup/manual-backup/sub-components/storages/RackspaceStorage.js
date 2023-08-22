import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import RackspaceSettings from "../../../consumer-storage-settings/RackspaceSettings";
import { ThirdPartyStorages } from "@docspace/common/constants";
import { getFromLocalStorage } from "../../../../../../utils";

class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage, setCompletedFormFields } = this.props;

    const basicValues = RackspaceSettings.formNames();

    const moduleValues = getFromLocalStorage(
      "LocalCopyThirdPartyStorageValues"
    );
    const moduleType =
      getFromLocalStorage("LocalCopyStorage") ===
      ThirdPartyStorages.RackspaceId;

    setCompletedFormFields(
      moduleType && moduleValues ? moduleValues : basicValues
    );

    this.isDisabled = selectedStorage && !selectedStorage.isSet;
  }

  render() {
    const {
      t,
      isLoadingData,
      isMaxProgress,
      selectedStorage,
      buttonSize,
      onMakeCopyIntoStorage,
      isValidForm,
    } = this.props;

    return (
      <>
        <RackspaceSettings
          isLoadingData={isLoadingData}
          selectedStorage={selectedStorage}
          t={t}
        />

        <div className="manual-backup_buttons">
          <Button
            id="create-copy"
            label={t("Common:CreateCopy")}
            onClick={onMakeCopyIntoStorage}
            primary
            isDisabled={!isValidForm || !isMaxProgress || this.isDisabled}
            size={buttonSize}
          />
        </div>
      </>
    );
  }
}

export default inject(({ backup }) => {
  const { setCompletedFormFields, isValidForm } = backup;

  return {
    isValidForm,
    setCompletedFormFields,
  };
})(observer(withTranslation(["Settings", "Common"])(RackspaceStorage)));
