import React from "react";
import TextInput from "@docspace/components/text-input";

const publicInput = "public_container";
const privateInput = "private_container";
class SelectelSettings extends React.Component {
  static formNames = () => {
    return [publicInput, privateInput];
  };

  constructor(props) {
    super(props);
    const { selectedStorage } = this.props;

    this.isDisabled = selectedStorage && !selectedStorage.isSet;

    this.privatePlaceholder =
      selectedStorage && selectedStorage.properties[0].title;

    this.publicPlaceholder =
      selectedStorage && selectedStorage.properties[1].title;
  }

  render() {
    const {
      formSettings,
      isError,
      isLoadingData,
      isLoading,
      onChange,
    } = this.props;

    return (
      <>
        <TextInput
          name={privateInput}
          className="backup_text-input"
          scale={true}
          value={formSettings.private_container}
          hasError={isError?.private_container}
          onChange={onChange}
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
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.publicPlaceholder || ""}
          tabIndex={2}
        />
      </>
    );
  }
}
export default SelectelSettings;
