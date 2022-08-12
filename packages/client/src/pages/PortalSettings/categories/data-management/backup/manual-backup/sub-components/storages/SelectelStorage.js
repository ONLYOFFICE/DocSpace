import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import SelectelSettings from "../../../consumer-storage-settings/SelectelSettings";

class SelectelStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage, setCompletedFormFields } = this.props;

    setCompletedFormFields(SelectelSettings.formNames());

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
        <SelectelSettings
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
  const { setCompletedFormFields, isValidForm } = backup;

  return {
    isValidForm,
    setCompletedFormFields,
  };
})(observer(withTranslation(["Settings", "Common"])(SelectelStorage)));
