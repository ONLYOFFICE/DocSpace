import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import GoogleCloudSettings from "../../../consumer-storage-settings/GoogleCloudSettings";
import { ThirdPartyStorages } from "@docspace/common/constants";
import { getFromLocalStorage } from "../../../../../../utils";

class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage, setCompletedFormFields } = this.props;

    const basicValues = GoogleCloudSettings.formNames();

    const moduleValues = getFromLocalStorage(
      "LocalCopyThirdPartyStorageValues"
    );

    const moduleType =
      getFromLocalStorage("LocalCopyStorage") === ThirdPartyStorages.GoogleId;

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
        <GoogleCloudSettings
          selectedStorage={selectedStorage}
          isLoadingData={isLoadingData}
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
})(observer(withTranslation(["Settings", "Common"])(GoogleCloudStorage)));
