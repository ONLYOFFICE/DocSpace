import React from "react";
import { withTranslation } from "react-i18next";
import SelectelSettings from "../../consumer-storage-settings/SelectelSettings";
import Button from "@appserver/components/button";
import ScheduleComponent from "../../sub-components-automatic-backup/ScheduleComponent";
import { StyledStoragesModule } from "../../StyledBackup";
class SelectelStorage extends React.Component {
  constructor(props) {
    super(props);
    const { selectedStorage } = this.props;

    this.defaultPrivateValue = selectedStorage.properties[0].value;

    this.defaultPublicValue = selectedStorage.properties[1].value;

    this.state = {
      formSettings: {
        private_container: this.defaultPrivateValue,
        public_container: this.defaultPublicValue,
      },
      formErrors: {},
      isError: false,
      isChangedInput: false,
    };
    this.isDisabled = !selectedStorage.isSet;
  }

  onChange = (event) => {
    const { formSettings } = this.state;
    const { target } = event;
    const value = target.value;
    const name = target.name;

    this.setState({
      isChangedInput: true,
      formSettings: { ...formSettings, [name]: value },
    });
  };

  onSaveSettings = () => {
    const { convertSettings, isInvalidForm } = this.props;
    const { formSettings } = this.state;
    const { private_container, public_container } = formSettings;

    const isInvalid = isInvalidForm({
      private_container,
      public_container,
    });

    const hasError = isInvalid[0];
    const errors = isInvalid[1];

    if (hasError) {
      this.setState({ formErrors: errors });
      return;
    }

    const valuesArray = [private_container, public_container];

    this.setState({
      isChangedInput: false,
      formErrors: {},
    });
    convertSettings(valuesArray.length, valuesArray);
  };

  onCancelSettings = () => {
    const { onCancelModuleSettings } = this.props;

    onCancelModuleSettings();

    this.setState({
      formSettings: {
        private_container: this.defaultPrivateValue,
        public_container: this.defaultPublicValue,
      },
      formErrors: {},
      isChangedInput: false,
    });
  };

  render() {
    const { isChangedInput, formSettings, formErrors } = this.state;
    const {
      t,
      isLoadingData,
      isCopyingToLocal,
      isChangedThirdParty,
      isChanged,
      selectedStorage,
      ...rest
    } = this.props;

    return (
      <StyledStoragesModule>
        <SelectelSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoadingData={isLoadingData}
          isError={formErrors}
          selectedStorage={selectedStorage}
          t={t}
        />

        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />

        {(isChanged || isChangedThirdParty || isChangedInput) && (
          //isChanged - from auto backup, monitor  period, time and etc. options;
          //isChangedThirdParty - from storages module, monitors selection storage changes
          //isChangedInput - monitors inputs changes
          <div className="backup_storages-buttons">
            <Button
              label={t("Common:SaveButton")}
              onClick={this.onSaveSettings}
              primary
              isDisabled={isCopyingToLocal || isLoadingData || this.isDisabled}
              size="medium"
              tabIndex={10}
              className="save-button"
            />

            <Button
              label={t("Common:CancelButton")}
              isDisabled={isLoadingData}
              onClick={this.onCancelSettings}
              primary
              size="medium"
              tabIndex={10}
            />
          </div>
        )}
      </StyledStoragesModule>
    );
  }
}
export default withTranslation(["Settings", "Common"])(SelectelStorage);
