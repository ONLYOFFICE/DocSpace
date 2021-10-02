import React from "react";
import { withTranslation } from "react-i18next";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import RackspaceSettings from "../consumer-storage-settings/RackspaceSettings";

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
      formSettings: {
        private_container: this.defaultPrivateValue,
        public_container: this.defaultPublicValue,
        region: this.defaultRegion,
      },
      formErrors: {},
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
    const { private_container, public_container, region } = formSettings;

    const isInvalid = isInvalidForm({
      private_container,
      region,
      public_container,
    });

    const hasError = isInvalid[0];
    const errors = isInvalid[1];

    if (hasError) {
      this.setState({ formErrors: errors });
      return;
    }

    const valuesArray = [private_container, public_container, region];

    const inputNumber = valuesArray.length;

    this.defaultPrivateValue = private_container;
    this.defaultPublicValue = public_container;
    this.defaultRegion = region;

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
        private_container: this.defaultPrivateValue,
        public_container: this.defaultPublicValue,
        region: this.defaultRegion,
      },
    });
    onCancelSettings();
  };

  render() {
    const { isChangedInput, formSettings, formErrors } = this.state;
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
        <RackspaceSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoading={isLoading}
          isLoadingData={isLoadingData}
          isError={formErrors}
          availableStorage={availableStorage}
          selectedId={selectedId}
          t={t}
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
            isDisabled={
              isCopyingToLocal || isLoadingData || isLoading || this.isDisabled
            }
          />
        )}
      </>
    );
  }
}
export default withTranslation(["Settings", "Common"])(RackspaceStorage);
