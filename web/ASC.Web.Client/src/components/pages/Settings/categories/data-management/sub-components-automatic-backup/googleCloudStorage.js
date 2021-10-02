import React from "react";
import { withTranslation } from "react-i18next";

import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import TextInput from "@appserver/components/text-input";
import GoogleCloudSettings from "../consumer-storage-settings/GoogleCloudSettings";

class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { availableStorage, selectedId, currentStorageId } = this.props;

    this.defaultBucketValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[0].value
        : "";

    this.state = {
      formSettings: {
        bucket: this.defaultBucketValue,
      },
      formErrors: {},
      isError: false,
      isChangedInput: false,
    };
    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this._isMounted = false;
  }

  componentDidMount() {
    this._isMounted = true;
  }
  componentWillUnmount() {
    this._isMounted = false;
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
    const { fillInputValueArray, isInvalidForm } = this.props;
    const { formSettings } = this.state;
    const { bucket } = formSettings;
    const isInvalid = isInvalidForm({
      bucket,
    });

    const hasError = isInvalid[0];
    const errors = isInvalid[1];

    if (hasError) {
      this.setState({ formErrors: errors });
      return;
    }

    const inputNumber = 1;
    const valuesArray = [bucket];

    this.defaultBucketValue = bucket;

    this.setState({
      isChangedInput: false,
      formErrors: {},
    });
    fillInputValueArray(inputNumber, valuesArray);
  };

  onCancelSettings = () => {
    const { onCancelSettings } = this.props;

    this.setState({
      isChangedInput: false,
      formErrors: {},
      formSettings: {
        bucket: this.defaultBucketValue,
      },
    });
    onCancelSettings();
  };

  render() {
    const { isChangedInput, formErrors, formSettings } = this.state;
    const {
      t,
      isLoadingData,
      isLoading,
      isCopyingToLocal,
      isChanged,
      availableStorage,
      selectedId,
    } = this.props;

    return (
      <>
        <GoogleCloudSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoading={isLoading}
          isError={formErrors}
          selectedStorage={availableStorage[selectedId]}
        />

        {(isChanged || isChangedInput) && (
          <SaveCancelButtons
            className="team-template_buttons"
            onSaveClick={this.onSaveSettings}
            onCancelClick={this.onCancelSettings}
            showReminder={false}
            reminderTest={t("YouHaveUnsavedChanges")}
            saveButtonLabel={t("Common:SaveButton")}
            cancelButtonLabel={t("Common:CancelButton")}
            isDisabled={isCopyingToLocal || isLoadingData || isLoading}
          />
        )}
      </>
    );
  }
}
export default withTranslation(["Settings", "Common"])(GoogleCloudStorage);
