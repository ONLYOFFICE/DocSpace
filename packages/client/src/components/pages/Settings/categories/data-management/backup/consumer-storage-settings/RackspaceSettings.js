import React from "react";
import TextInput from "@docspace/components/text-input";

const regionInput = "region";
const publicInput = "public_container";
const privateInput = "private_container";
class RackspaceSettings extends React.Component {
  static formNames = () => {
    return [regionInput, publicInput, privateInput];
  };

  constructor(props) {
    super(props);
    const { selectedStorage } = this.props;

    this.isDisabled = selectedStorage && !selectedStorage.isSet;

    this.privatePlaceholder =
      selectedStorage && selectedStorage.properties[0].title;

    this.publicPlaceholder =
      selectedStorage && selectedStorage.properties[1].title;

    this.regionPlaceholder =
      selectedStorage && selectedStorage.properties[2].title;
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
          scale
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
          scale
          value={formSettings.public_container}
          hasError={isError?.public_container}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.publicPlaceholder || ""}
          tabIndex={2}
        />
        <TextInput
          name={regionInput}
          className="backup_text-input"
          scale
          value={formSettings.region}
          hasError={isError?.region}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.regionPlaceholder || ""}
          tabIndex={3}
        />
      </>
    );
  }
}
export default RackspaceSettings;
