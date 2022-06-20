import React from "react";
import TextInput from "@appserver/components/text-input";
import Checkbox from "@appserver/components/checkbox";
import ComboBox from "@appserver/components/combobox";
import RadioButton from "@appserver/components/radio-button";

const bucketInput = "bucket";
const regionInput = "region";
const urlInput = "serviceurl";
const forcePathStyleInput = "forcepathstyle";
const httpInput = "usehttp";
const sseInput = "sse";
class AmazonSettings extends React.Component {
  static formNames = () => {
    return {
      bucket: "",
      region: "",
      serviceurl: "",
      forcepathstyle: false,
      usehttp: false,
      sse: "none",
    };

    // [
    //   bucketInput,
    //   regionInput,
    //   urlInput,
    //   forcePathStyleInput,
    //   httpInput,
    //   sseInput,
    // ];
  };

  static requiredFormsName = () => {
    return [bucketInput, regionInput, httpInput, sseInput];
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

    this.availableEncryptions = [
      {
        key: "0",
        label: "None",
      },
      {
        key: "1",
        label: "Server-Side Encryption",
      },
      {
        key: "2",
        label: "Client-Side Encryption",
      },
    ];

    this.state = {
      selectedEncryption: this.availableEncryptions[0],
    };
  }

  setMaxCopies = (options) => {
    const key = options.key;
    this.selectedMaxCopiesNumber = key;
  };

  onSelect = (options) => {
    const key = options.key;
    const label = options.label;

    console.log("options", options);
    this.setState = {
      selectedEncryption: { key, label },
    };
  };
  render() {
    const {
      isError,
      isLoadingData,
      isLoading,
      onChange,
      formSettings,
    } = this.props;
    const { selectedEncryption } = this.state;
    console.log(
      "selectedEncryption",
      selectedEncryption,
      selectedEncryption.label
    );

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

        <Checkbox
          name={forcePathStyleInput}
          label={this.forcePathStylePlaceholder}
          isChecked={formSettings.forcepathstyle}
          isIndeterminate={false}
          isDisabled={false}
          onChange={onChange}
          tabIndex={4}
        />

        <Checkbox
          name={httpInput}
          label={this.useHttpPlaceholder}
          isChecked={formSettings.usehttp}
          isIndeterminate={false}
          isDisabled={false}
          onChange={onChange}
          tabIndex={5}
        />

        <TextInput
          hasError={isError?.sse}
          name={sseInput}
          className="backup_text-input"
          scale
          value={formSettings.sse}
          onChange={onChange}
          isDisabled={isLoadingData || isLoading || this.isDisabled}
          placeholder={this.SSEPlaceholder || ""}
          tabIndex={6}
        />

        <ComboBox
          options={this.availableEncryptions}
          selectedOption={{
            key: 0,
            label: selectedEncryption.label,
          }}
          onSelect={this.onSelect}
          noBorder={false}
          scaled={true}
          scaledOptions={true}
          dropDownMaxHeight={300}
          tabIndex={7}
        />
        {/* 
{selectedEncryption.key === 1 &&    <RadioButton
                // fontSize="13px"
                // fontWeight="400"
                value=""
                label=""
                isChecked={isChecked}
                onClick={onSelectFile}
                name=``
              />} */}
      </>
    );
  }
}
export default AmazonSettings;
