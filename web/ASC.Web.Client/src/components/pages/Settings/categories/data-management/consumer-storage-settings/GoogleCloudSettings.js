import React from "react";
import TextInput from "@appserver/components/text-input";

class GoogleCloudSettings extends React.Component {
  constructor(props) {
    super(props);
    const { availableStorage, selectedId } = this.props;

    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this.bucketPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[0].title;
  }

  render() {
    const {
      formSettings,
      isError,
      isLoadingData,
      isLoading,
      onChange,
    } = this.props;
    console.log("isError settings", isError);
    return (
      <>
        <TextInput
          name="bucket"
          className="backup_text-input"
          scale
          value={formSettings.bucket}
          hasError={isError?.bucket}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.bucketPlaceholder}
          tabIndex={1}
        />
      </>
    );
  }
}
export default GoogleCloudSettings;
