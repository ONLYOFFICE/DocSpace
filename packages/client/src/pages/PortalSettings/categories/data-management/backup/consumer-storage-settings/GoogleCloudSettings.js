import React from "react";
import TextInput from "@docspace/components/text-input";

const bucket = "bucket";
class GoogleCloudSettings extends React.Component {
  static formNames = () => {
    return [bucket];
  };

  constructor(props) {
    super(props);
    const { selectedStorage } = this.props;

    this.isDisabled = selectedStorage && !selectedStorage.isSet;

    this.bucketPlaceholder =
      selectedStorage && selectedStorage.properties[0].title;
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
          name={bucket}
          className="backup_text-input"
          scale
          value={formSettings.bucket}
          hasError={isError?.bucket}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.bucketPlaceholder || ""}
        />
      </>
    );
  }
}
export default GoogleCloudSettings;
