import React from "react";
import { withTranslation } from "react-i18next";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import AmazonSettings from "../consumer-storage-settings/AmazonSettings";

class AmazonStorage extends React.Component {
  constructor(props) {
    super(props);
    const { t, availableStorage, selectedId, currentStorageId } = this.props;

    this.defaultBucketValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[0].value
        : "";

    this.defaultForcePathStyleValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[1].value
        : "";
    this.defaultRegionValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[2].value
        : "";
    this.defaultServiceUrlValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[3].value
        : "";
    this.defaultSSEValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[4].value
        : "";

    this.defaultUseHttpValue =
      currentStorageId && availableStorage && selectedId === currentStorageId
        ? availableStorage[currentStorageId].properties[5].value
        : "";

    this.state = {
      formSettings: {
        bucket: this.defaultBucketValue,
        forcePathStyle: this.defaultForcePathStyleValue,
        region: this.defaultRegionValue,
        serviceUrl: this.defaultServiceUrlValue,
        sse: this.defaultSSEValue,
        useHttp: this.defaultUseHttpValue,
      },
      formErrors: {},
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
    const {
      bucket,
      forcePathStyle,
      region,
      serviceUrl,
      sse,
      useHttp,
    } = formSettings;

    const isInvalid = isInvalidForm({
      bucket,
      region,
      forcePathStyle,
      serviceUrl,
      sse,
      useHttp,
    });

    const hasError = isInvalid[0];
    const errors = isInvalid[1];

    if (hasError) {
      this.setState({ formErrors: errors });
      return;
    }

    const valuesArray = [
      bucket,
      forcePathStyle,
      region,
      serviceUrl,
      sse,
      useHttp,
    ];

    const inputNumber = valuesArray.length;

    this.defaultBucketValue = bucket;

    this.defaultForcePathStyleValue = forcePathStyle;
    this.defaultRegionValue = region;
    this.defaultServiceUrlValue = serviceUrl;
    this.defaultSSEValue = sse;

    this.defaultUseHttpValue = useHttp;

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
        forcePathStyle: this.defaultForcePathStyleValue,
        region: this.defaultRegionValue,
        serviceUrl: this.defaultServiceUrlValue,
        sse: this.defaultSSEValue,
        useHttp: this.defaultUseHttpValue,
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
      selectedId,
      availableStorage,
    } = this.props;

    return (
      <>
        <AmazonSettings
          formSettings={formSettings}
          onChange={this.onChange}
          isLoading={isLoading}
          isLoadingData={isLoadingData}
          isError={formErrors}
          selectedStorage={availableStorage[selectedId]}
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
export default withTranslation(["Settings", "Common"])(AmazonStorage);
