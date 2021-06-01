import React from "react";
import { withTranslation } from "react-i18next";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";

class RackspaceStorage extends React.Component {
  constructor(props) {
    super(props);
    const { availableStorage, selectedId, currentStorageId } = this.props;

    this.defaultPrivateValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[0].value
        : "";

    this.defaultPublicValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[1].value
        : "";
    this.defaultRegion =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[2].value
        : "";

    this.state = {
      private_container: this.defaultPrivateValue,
      public_container: this.defaultPublicValue,
      region: this.defaultRegion,
      isError: false,
      isChangedInput: false,
    };
    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this.privatePlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[0].title;

    this.publicPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[1].title;

    this.regionPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[2].title;

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
    const { private_container, public_container, region } = this.state;
    if (private_container || public_container || region) return false;

    this.setState({
      isError: true,
    });
    return true;
  };
  onSaveSettings = () => {
    const { fillInputValueArray } = this.props;
    const { private_container, public_container, region } = this.state;

    if (this.isInvalidForm()) return;

    const valuesArray = [private_container, public_container, region];

    const inputNumber = valuesArray.length;

    this.defaultPrivateValue = private_container;
    this.defaultPublicValue = public_container;
    this.defaultRegion = region;

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
      private_container: this.defaultPrivateValue,
      public_container: this.defaultPublicValue,
      region: this.defaultRegion,
    });
    onCancelSettings();
  };
  onMakeCopy = () => {
    const { fillInputValueArray } = this.props;
    const { private_container, public_container, region } = this.state;

    if (this.isInvalidForm()) return;

    const valuesArray = [private_container, public_container, region];

    const inputNumber = valuesArray.length;

    this.setState({
      isChangedInput: false,
      isError: false,
    });
    fillInputValueArray(inputNumber, valuesArray);
  };
  render() {
    const {
      private_container,
      public_container,
      region,
      isChangedInput,
      isError,
    } = this.state;
    const {
      t,
      isLoadingData,
      isLoading,
      isManualBackup,
      maxProgress,
      isCopyingToLocal,
      isChanged,
    } = this.props;

    return (
      <>
        <TextInput
          name="private_container"
          className="backup_text-input"
          scale={true}
          value={private_container}
          hasError={isError}
          onChange={this.onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.privatePlaceholder || ""}
          tabIndex={1}
        />
        <TextInput
          name="public_container"
          className="backup_text-input"
          scale={true}
          value={public_container}
          hasError={isError}
          onChange={this.onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.publicPlaceholder || ""}
          tabIndex={1}
        />
        <TextInput
          name="region"
          className="backup_text-input"
          scale={true}
          value={region}
          hasError={isError}
          onChange={this.onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.regionPlaceholder || ""}
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
              isDisabled={
                isCopyingToLocal ||
                isLoadingData ||
                isLoading ||
                this.isDisabled
              }
            />
          )
        ) : (
          <div className="manual-backup_buttons">
            <Button
              label={t("MakeCopy")}
              onClick={this.onMakeCopy}
              primary
              isDisabled={!maxProgress || this.isDisabled}
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
export default withTranslation("Settings")(RackspaceStorage);
