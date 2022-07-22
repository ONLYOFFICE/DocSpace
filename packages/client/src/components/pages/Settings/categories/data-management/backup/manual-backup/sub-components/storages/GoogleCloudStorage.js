import React from "react";
import { withTranslation } from "react-i18next";
import Button from "@docspace/components/button";
import GoogleCloudSettings from "../../../consumer-storage-settings/GoogleCloudSettings";

class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage } = this.props;
    const formSettings = {};
    this.namesArray = GoogleCloudSettings.formNames();
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
        <GoogleCloudSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isError={formErrors}
          selectedStorage={selectedStorage}
          isLoadingData={isLoadingData}
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
export default withTranslation(["Settings", "Common"])(GoogleCloudStorage);
