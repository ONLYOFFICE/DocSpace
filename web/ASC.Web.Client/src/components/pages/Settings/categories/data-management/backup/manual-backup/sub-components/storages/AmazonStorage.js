import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import AmazonSettings from "../../../consumer-storage-settings/AmazonSettings";

class AmazonStorage extends React.Component {
  constructor(props) {
    super(props);
    const {
      selectedStorage,
      setCompletedFormFields,
      storageRegions,
    } = this.props;

    setCompletedFormFields(
      AmazonSettings.formNames(storageRegions[0].systemName)
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
    console.log("amazon storage render");

    return (
      <>
        <AmazonSettings
          isLoadingData={isLoadingData}
          selectedStorage={selectedStorage}
          t={t}
        />

        <div className="manual-backup_buttons">
          <Button
            label={t("Common:Duplicate")}
            onClick={onMakeCopyIntoStorage}
            primary
            isDisabled={!isValidForm || !isMaxProgress || this.isDisabled}
            size={buttonSize}
          />
          {!isMaxProgress && (
            <Button
              label={t("Common:CopyOperation") + "..."}
              isDisabled={true}
              size={buttonSize}
              style={{ marginLeft: "8px" }}
            />
          )}
        </div>
      </>
    );
  }
}

export default inject(({ backup }) => {
  const { setCompletedFormFields, storageRegions, isValidForm } = backup;

  return {
    isValidForm,
    storageRegions,
    setCompletedFormFields,
  };
})(observer(withTranslation(["Settings", "Common"])(AmazonStorage)));
