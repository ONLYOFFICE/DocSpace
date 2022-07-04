import React from "react";
import { withTranslation } from "react-i18next";
import Button from "@appserver/components/button";
import AmazonSettings from "../../../consumer-storage-settings/AmazonSettings";
import { inject, observer } from "mobx-react";
class AmazonStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage, setCompletedFormFields } = this.props;

    setCompletedFormFields(AmazonSettings.formNames());

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
            isDisabled={!isMaxProgress || this.isDisabled}
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
  const { setCompletedFormFields } = backup;

  return {
    setCompletedFormFields,
  };
})(observer(withTranslation(["Settings", "Common"])(AmazonStorage)));
