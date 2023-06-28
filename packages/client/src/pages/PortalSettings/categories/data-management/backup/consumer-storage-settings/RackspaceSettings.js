import React from "react";
import TextInput from "@docspace/components/text-input";
import { inject, observer } from "mobx-react";

const region = "region";
const public_container = "public_container";
const private_container = "private_container";
const filePath = "filePath";
class RackspaceSettings extends React.Component {
  static formNames = () => {
    return { region: "", public_container: "", private_container: "" };
  };

  constructor(props) {
    super(props);
    const {
      selectedStorage,
      setRequiredFormSettings,
      setIsThirdStorageChanged,
      isNeedFilePath,
    } = this.props;

    setIsThirdStorageChanged(false);
    const filePathField = isNeedFilePath ? [filePath] : [];
    setRequiredFormSettings([
      region,
      public_container,
      private_container,
      ...filePathField,
    ]);

    this.isDisabled = selectedStorage && !selectedStorage.isSet;

    this.privatePlaceholder =
      selectedStorage && selectedStorage.properties[0].title;
    this.publicPlaceholder =
      selectedStorage && selectedStorage.properties[1].title;
    this.regionPlaceholder =
      selectedStorage && selectedStorage.properties[2].title;
  }
  onChangeText = (event) => {
    const { addValueInFormSettings } = this.props;

    const { target } = event;
    const value = target.value;
    const name = target.name;

    addValueInFormSettings(name, value);
  };

  render() {
    const {
      formSettings,
      errorsFieldsBeforeSafe: isError,
      isLoadingData,
      isLoading,
      t,
      isNeedFilePath,
    } = this.props;

    return (
      <>
        <TextInput
          id="private-container-input"
          name={private_container}
          className="backup_text-input"
          scale
          value={formSettings[private_container]}
          hasError={isError[private_container]}
          onChange={this.onChangeText}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.privatePlaceholder || ""}
          tabIndex={1}
        />
        <TextInput
          id="public-container-input"
          name={public_container}
          className="backup_text-input"
          scale
          value={formSettings[public_container]}
          hasError={isError[public_container]}
          onChange={this.onChangeText}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.publicPlaceholder || ""}
          tabIndex={2}
        />
        <TextInput
          id="region-input"
          name={region}
          className="backup_text-input"
          scale
          value={formSettings[region]}
          hasError={isError[region]}
          onChange={this.onChangeText}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.regionPlaceholder || ""}
          tabIndex={3}
        />
        {isNeedFilePath && (
          <TextInput
            id="file-path-input"
            name={filePath}
            className="backup_text-input"
            scale
            value={formSettings[filePath]}
            onChange={this.onChangeText}
            isDisabled={isLoadingData || isLoading || this.isDisabled}
            placeholder={t("Path")}
            tabIndex={4}
            hasError={isError[filePath]}
          />
        )}
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
    addValueInFormSettings,
  } = backup;

  return {
    setFormSettings,
    setRequiredFormSettings,
    formSettings,
    errorsFieldsBeforeSafe,
    setIsThirdStorageChanged,
    addValueInFormSettings,
  };
})(observer(RackspaceSettings));
