import React from "react";
import { inject, observer } from "mobx-react";
import TextInput from "@appserver/components/text-input";
import { onChangeTextInput } from "./InputsMethods";

const publicInput = "public_container";
const privateInput = "private_container";
class SelectelSettings extends React.Component {
  static formNames = () => {
    return { public_container: "", private_container: "" };
  };

  constructor(props) {
    super(props);
    const {
      selectedStorage,
      setRequiredFormSettings,
      setIsThirdStorageChanged,
    } = this.props;
    setIsThirdStorageChanged(false);
    setRequiredFormSettings([publicInput, privateInput]);

    this.isDisabled = selectedStorage && !selectedStorage.isSet;

    this.privatePlaceholder =
      selectedStorage && selectedStorage.properties[0].title;

    this.publicPlaceholder =
      selectedStorage && selectedStorage.properties[1].title;
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
      formSettings,
      errorsFieldsBeforeSafe: isError,
      isLoadingData,
      isLoading,
    } = this.props;

    return (
      <>
        <TextInput
          name={privateInput}
          className="backup_text-input"
          scale={true}
          value={formSettings.private_container}
          hasError={isError?.private_container}
          onChange={this.onChangeText}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.privatePlaceholder || ""}
          tabIndex={1}
        />
        <TextInput
          name={publicInput}
          className="backup_text-input"
          scale={true}
          value={formSettings.public_container}
          hasError={isError?.public_container}
          onChange={this.onChangeText}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.publicPlaceholder || ""}
          tabIndex={2}
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
})(observer(SelectelSettings));
