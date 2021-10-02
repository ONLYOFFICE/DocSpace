import React from "react";
import TextInput from "@appserver/components/text-input";

const publicInput = "public_container";
const privateInput = "private_container";
class SelectelSettings extends React.Component {
  static formNames = () => {
    return [publicInput, privateInput];
  };

  constructor(props) {
    super(props);
    const { availableStorage, selectedId } = this.props;

    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this.privatePlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[0].title;

    this.publicPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[1].title;
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
          hasError={isError.private_container}
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
          hasError={isError.public_container}
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
