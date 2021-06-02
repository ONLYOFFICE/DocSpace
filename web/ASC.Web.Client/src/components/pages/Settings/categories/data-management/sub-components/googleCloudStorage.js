import React from "react";
import { withTranslation } from "react-i18next";

import Button from "@appserver/components/button";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import TextInput from "@appserver/components/text-input";

class GoogleCloudStorage extends React.Component {
  constructor(props) {
    super(props);
    const { availableStorage, selectedId, currentStorageId } = this.props;

    this.defaultBucketValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[0].value
        : "";

    this.state = {
      bucket: this.defaultBucketValue,
      isError: false,
      isChangedInput: false,
    };
    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this.placeholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[0].title;
    this._isMounted = false;
  }

  componentDidMount() {
    this._isMounted = true;
  }
  componentWillUnmount() {
    this._isMounted = false;
  }

  onChange = (event) => {
    const { target } = event;
    const value = target.value;
    const name = target.name;

    this.setState({ [name]: value, isChangedInput: true });
  };

  isInvalidForm = () => {
    const { bucket } = this.state;
    if (bucket) return false;

    this.setState({
      isError: true,
    });
    return true;
  };
  onSaveSettings = () => {
    const { fillInputValueArray } = this.props;
    const { bucket } = this.state;

    if (this.isInvalidForm()) return;

    const inputNumber = 1;
    const valuesArray = [bucket];

    this.defaultBucketValue = bucket;

    this.setState({
      isChangedInput: false,
      isError: false,
    });
    fillInputValueArray(inputNumber, valuesArray);
  };

  onCancelSettings = () => {
    const { onCancelSettings } = this.props;

    this.setState({
      isChangedInput: false,
      isError: false,
      bucket: this.defaultBucketValue,
    });
    onCancelSettings();
  };
  onMakeCopy = () => {
    const { fillInputValueArray } = this.props;
    const { bucket } = this.state;

    if (this.isInvalidForm()) return;

    const inputNumber = 1;
    const valuesArray = [bucket];

    this.setState({
      isChangedInput: false,
      isError: false,
    });
    fillInputValueArray(inputNumber, valuesArray);
  };
  render() {
    const { bucket, isChangedInput, isError } = this.state;
    const {
      t,
      isLoadingData,
      isLoading,
      isManualBackup,
      maxProgress,
      isCopyingToLocal,
      isChanged,
    } = this.props;
    console.log("isLoadingData", isLoadingData);
    return (
      <>
        <TextInput
          name="bucket"
          className="backup_text-input"
          scale={true}
          value={bucket}
          hasError={isError}
          onChange={this.onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.placeholder}
          tabIndex={1}
        />

        {!isManualBackup ? (
          (isChanged || isChangedInput) && (
            <SaveCancelButtons
              className="team-template_buttons"
              onSaveClick={this.onSaveSettings}
              onCancelClick={this.onCancelSettings}
              showReminder={false}
              reminderTest={t("YouHaveUnsavedChanges")}
              saveButtonLabel={t("SaveButton")}
              cancelButtonLabel={t("CancelButton")}
              isDisabled={isCopyingToLocal || isLoadingData || isLoading}
            />
          )
        ) : (
          <div className="manual-backup_buttons">
            <Button
              label={t("MakeCopy")}
              onClick={this.onMakeCopy}
              primary
              isDisabled={!maxProgress}
              size="medium"
              tabIndex={10}
            />
            {!maxProgress && (
              <Button
                label={t("Copying")}
                onClick={() => console.log("click")}
                isDisabled={true}
                size="medium"
                style={{ marginLeft: "8px" }}
                tabIndex={11}
              />
            )}
          </div>
        )}
      </>
    );
  }
}
export default withTranslation("Settings")(GoogleCloudStorage);
