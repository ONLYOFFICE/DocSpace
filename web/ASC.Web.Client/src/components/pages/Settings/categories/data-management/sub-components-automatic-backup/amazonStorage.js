import React from "react";
import { withTranslation } from "react-i18next";
import TextInput from "@appserver/components/text-input";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";

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
      bucket: this.defaultBucketValue,
      forcePathStyle: this.defaultForcePathStyleValue,
      region: this.defaultRegionValue,
      serviceUrl: this.defaultServiceUrlValue,
      sse: this.defaultSSEValue,
      useHttp: this.defaultUseHttpValue,
      isError: false,
      isChangedInput: false,
    };
    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this.defaultBucketPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[0].title;

    this.defaultForcePathStylePlaceholder = t("ForcePathStyle");

    this.defaultRegionPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[2].title;

    this.defaultServiceUrlPlaceholder = t("ServiceUrl");

    this.defaultSSEPlaceholder = t("SSE");

    this.defaultUseHttpPlaceholder = t("UseHttp");
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
    const { bucket, region } = this.state;

    if (bucket || region) return false;

    this.setState({
      isError: true,
    });
    return true;
  };
  onSaveSettings = () => {
    const { fillInputValueArray } = this.props;
    const {
      bucket,
      forcePathStyle,
      region,
      serviceUrl,
      sse,
      useHttp,
    } = this.state;

    if (this.isInvalidForm()) return;

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
      forcePathStyle: this.defaultForcePathStyleValue,
      region: this.defaultRegionValue,
      serviceUrl: this.defaultServiceUrlValue,
      sse: this.defaultSSEValue,
      useHttp: this.defaultUseHttpValue,
    });
    onCancelSettings();
  };

  render() {
    const {
      isChangedInput,
      isError,
      bucket,
      forcePathStyle,
      region,
      serviceUrl,
      sse,
      useHttp,
    } = this.state;
    const {
      t,
      isLoadingData,
      isLoading,
      isCopyingToLocal,
      isChanged,
    } = this.props;

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
          placeholder={this.defaultBucketPlaceholder || ""}
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
          placeholder={this.defaultRegionPlaceholder || ""}
          tabIndex={1}
        />
        <TextInput
          name="serviceUrl"
          className="backup_text-input"
          scale={true}
          value={serviceUrl}
          onChange={this.onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.defaultServiceUrlPlaceholder || ""}
          tabIndex={1}
        />
        <TextInput
          name="forcePathStyle"
          className="backup_text-input"
          scale={true}
          value={forcePathStyle}
          onChange={this.onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.defaultForcePathStylePlaceholder || ""}
          tabIndex={1}
        />
        <TextInput
          name="useHttp"
          className="backup_text-input"
          scale={true}
          value={useHttp}
          onChange={this.onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.defaultUseHttpPlaceholder || ""}
          tabIndex={1}
        />
        <TextInput
          name="sse"
          className="backup_text-input"
          scale={true}
          value={sse}
          onChange={this.onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.defaultSSEPlaceholder || ""}
          tabIndex={1}
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
