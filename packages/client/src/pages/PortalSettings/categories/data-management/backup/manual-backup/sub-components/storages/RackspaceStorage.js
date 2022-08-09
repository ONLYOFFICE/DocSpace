import React from "react";
import { withTranslation } from "react-i18next";
import Button from "@docspace/components/button";
import RackspaceSettings from "../../../consumer-storage-settings/RackspaceSettings";

class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage } = this.props;

    const formSettings = {};
    this.namesArray = RackspaceSettings.formNames();
    this.namesArray.forEach((elem) => (formSettings[elem] = ""));

    this.state = {
      formSettings,
      formErrors: {},
    };
    this.isDisabled = selectedStorage && !selectedStorage.isSet;
  }

  onChange = (event) => {
    const { formSettings } = this.state;
    const { target } = event;
    const value = target.value;
    const name = target.name;

    this.setState({ formSettings: { ...formSettings, [name]: value } });
  };

  onMakeCopy = () => {
    const { formSettings } = this.state;

    const { onMakeCopyIntoStorage, isInvalidForm } = this.props;

    const isInvalid = isInvalidForm(formSettings);

    const hasError = isInvalid[0];
    const errors = isInvalid[1];

    if (hasError) {
      this.setState({ formErrors: errors });
      return;
    }

    const arraySettings = Object.entries(formSettings);

    onMakeCopyIntoStorage(arraySettings);
    this.setState({ formErrors: {} });
  };
  render() {
    const { formSettings, formErrors } = this.state;
    const {
      t,
      isLoadingData,
      isMaxProgress,
      selectedStorage,
      buttonSize,
    } = this.props;

    return (
      <>
        <RackspaceSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoadingData={isLoadingData}
          isError={formErrors}
          selectedStorage={selectedStorage}
          t={t}
        />

        <div className="manual-backup_buttons">
          <Button
            label={t("Common:Duplicate")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={!isMaxProgress || this.isDisabled}
            size={buttonSize}
          />
          {!isMaxProgress && (
            <Button
              label={t("Common:CopyOperation") + "..."}
              isDisabled
              size={buttonSize}
              style={{ marginLeft: "8px" }}
            />
          )}
        </div>
      </>
    );
  }
}
export default withTranslation(["Settings", "Common"])(RackspaceStorage);
