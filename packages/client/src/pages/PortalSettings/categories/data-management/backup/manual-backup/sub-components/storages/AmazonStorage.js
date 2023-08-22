import React from "react";
import { withTranslation } from "react-i18next";
import Button from "@docspace/components/button";
import AmazonSettings from "../../../consumer-storage-settings/AmazonSettings";
import { inject, observer } from "mobx-react";
import { getFromLocalStorage } from "../../../../../../utils";
import { ThirdPartyStorages } from "@docspace/common/constants";
class AmazonStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage, setCompletedFormFields, storageRegions } =
      this.props;

    const basicValues = AmazonSettings.formNames(storageRegions[0].systemName);

    const moduleValues = getFromLocalStorage(
      "LocalCopyThirdPartyStorageValues"
    );
    const moduleType =
      getFromLocalStorage("LocalCopyStorage") === ThirdPartyStorages.AmazonId;

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
        <AmazonSettings
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
  const { setCompletedFormFields, storageRegions, isValidForm } = backup;

  return {
    setCompletedFormFields,
    storageRegions,
    isValidForm,
  };
})(observer(withTranslation(["Settings", "Common"])(AmazonStorage)));
