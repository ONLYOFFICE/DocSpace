import React from "react";
import { inject, observer } from "mobx-react";
import TextInput from "@appserver/components/text-input";
import { onChangeTextInput } from "./InputsMethods";

const bucket = "bucket";
class GoogleCloudSettings extends React.Component {
  static formNames = () => {
    return { bucket: "" };
  };

  constructor(props) {
    super(props);
    const {
      selectedStorage,
      setRequiredFormSettings,
      setIsThirdStorageChanged,
    } = this.props;

    setRequiredFormSettings([bucket]);
    setIsThirdStorageChanged(false);
    this.isDisabled = selectedStorage && !selectedStorage.isSet;

    this.bucketPlaceholder =
      selectedStorage && selectedStorage.properties[0].title;
  }

  onChangeText = (e) => {
    const {
      formSettings,
      setFormSettings,
      setIsThirdStorageChanged,
    } = this.props;
    const newState = onChangeTextInput(formSettings, e);
    setIsThirdStorageChanged(true);
    setFormSettings(newState);
  };
  render() {
    const {
      errorsFieldsBeforeSafe: isError,
      formSettings,
      isLoadingData,
      isLoading,
    } = this.props;

    return (
      <>
        <TextInput
          name={bucket}
          className="backup_text-input"
          scale
          value={formSettings.bucket}
          hasError={isError?.bucket}
          onChange={this.onChangeText}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.bucketPlaceholder || ""}
        />
      </>
    );
  }
}

export default inject(({ backup }) => {
  const {
    setFormSettings,
    setRequiredFormSettings,
    formSettings,
    errorsFieldsBeforeSafe,
    setIsThirdStorageChanged,
  } = backup;

  return {
    setFormSettings,
    setRequiredFormSettings,
    formSettings,
    errorsFieldsBeforeSafe,
    setIsThirdStorageChanged,
  };
})(observer(GoogleCloudSettings));
