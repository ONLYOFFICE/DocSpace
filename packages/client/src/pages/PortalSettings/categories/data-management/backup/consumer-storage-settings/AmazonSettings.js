import React from "react";
import TextInput from "@docspace/components/text-input";

const bucketInput = "bucket";
const regionInput = "region";
const urlInput = "serviceurl";
const forcePathStyleInput = "forcepathstyle";
const httpInput = "usehttp";
const sseInput = "sse";
class AmazonSettings extends React.Component {
  static formNames = () => {
    return [
      bucketInput,
      regionInput,
      urlInput,
      forcePathStyleInput,
      httpInput,
      sseInput,
    ];
  };

  static requiredFormsName = () => {
    return [bucketInput, regionInput];
  };

  constructor(props) {
    super(props);
    const { t, selectedStorage } = this.props;

    this.isDisabled = selectedStorage && !selectedStorage.isSet;

    this.bucketPlaceholder =
      selectedStorage && selectedStorage.properties[0].title;

    this.forcePathStylePlaceholder = t("ForcePathStyle");

    this.regionPlaceholder =
      selectedStorage && selectedStorage.properties[2].title;

    this.serviceUrlPlaceholder = t("ServiceUrl");
    this.SSEPlaceholder = t("ServerSideEncryptionMethod");
    this.useHttpPlaceholder = t("UseHttp");
  }

  render() {
    const {
      isError,
      isLoadingData,
      isLoading,
      onChange,
      formSettings,
    } = this.props;

    return (
      <>
        <TextInput
          name={bucketInput}
          className="backup_text-input"
          scale
          value={formSettings.bucket}
          hasError={isError?.bucket}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.bucketPlaceholder || ""}
          tabIndex={1}
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
          tabIndex={2}
        />
        <TextInput
          name={urlInput}
          className="backup_text-input"
          scale
          value={formSettings.serviceurl}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.serviceUrlPlaceholder || ""}
          tabIndex={3}
        />
        <TextInput
          name={forcePathStyleInput}
          className="backup_text-input"
          scale
          value={formSettings.forcepathstyle}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.forcePathStylePlaceholder || ""}
          tabIndex={4}
        />
        <TextInput
          name={httpInput}
          className="backup_text-input"
          scale
          value={formSettings.usehttp}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.useHttpPlaceholder || ""}
          tabIndex={5}
        />
        <TextInput
          name={sseInput}
          className="backup_text-input"
          scale
          value={formSettings.sse}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.SSEPlaceholder || ""}
          tabIndex={6}
        />
      </>
    );
  }
}
export default AmazonSettings;
