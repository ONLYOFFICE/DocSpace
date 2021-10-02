import React from "react";
import TextInput from "@appserver/components/text-input";

const bucketInput = "bucket";
const regionInput = "region";
const urlInput = "serviceUrl";
const forcePathStyleInput = "forcePathStyle";
const httpInput = "useHttp";
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

  constructor(props) {
    super(props);
    const { t, availableStorage, selectedId } = this.props;

    this.isDisabled =
      availableStorage[selectedId] && !availableStorage[selectedId].isSet;

    this.bucketPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[0].title;

    this.forcePathStylePlaceholder = t("ForcePathStyle");

    this.regionPlaceholder =
      availableStorage[selectedId] &&
      availableStorage[selectedId].properties[2].title;

    this.serviceUrlPlaceholder = t("ServiceUrl");
    this.SSEPlaceholder = t("SSE");
    this.useHttpPlaceholder = t("UseHttp");
  }

  render() {
    const {
      isError,
      serviceUrl,
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
          value={serviceUrl}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.serviceUrlPlaceholder || ""}
          tabIndex={3}
        />
        <TextInput
          name={forcePathStyleInput}
          className="backup_text-input"
          scale
          value={formSettings.forcePathStyle}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.forcePathStylePlaceholder || ""}
          tabIndex={4}
        />
        <TextInput
          name={httpInput}
          className="backup_text-input"
          scale
          value={formSettings.useHttp}
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
