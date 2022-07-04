import React from "react";
import TextInput from "@appserver/components/text-input";
import Checkbox from "@appserver/components/checkbox";
import ComboBox from "@appserver/components/combobox";
import RadioButton from "@appserver/components/radio-button";
import Text from "@appserver/components/text";
import styled, { css } from "styled-components";
import HelpButton from "@appserver/components/help-button";
import { Trans, withTranslation } from "react-i18next";
import {
  onChangeCheckbox,
  onChangeTextInput,
  onSelectEncryptionMode,
  onSelectManagedKeys,
  onSetAdditionInfo,
} from "./InputsMethods";
import { inject, observer } from "mobx-react";

const bucketInput = "bucket";
const regionInput = "region";
const urlInput = "serviceurl";
const forcePathStyleInput = "forcepathstyle";
const httpInput = "usehttp";
const sseInput = "sse";
const sse_kms = "ssekms";
const customKey = "customKey";
const clientKey = "clientKey";

const StyledBody = styled.div`
  margin-bottom: 16px;

  .backup_storage-tooltip {
    display: flex;
    align-items: center;
    p {
      margin-right: 8px;
    }
  }
`;
class AmazonSettings extends React.Component {
  static formNames = () => {
    return {
      bucket: "",
      region: "0",
      serviceurl: "",
      customKey: "",
      clientKey: "",
      forcepathstyle: false,
      usehttp: false,
      sse: "0",
      kms: true,
      s3: false,
    };
  };

  static requiredFormsName = () => {
    return [urlInput, bucketInput, customKey, clientKey];
  };

  constructor(props) {
    super(props);
    const { t, selectedStorage, setRequiredFormSettings } = this.props;

    this.isDisabled = selectedStorage && !selectedStorage.isSet;
    console.log("selectedStorage", selectedStorage);

    setRequiredFormSettings([urlInput, bucketInput]); //TODO: add the flexible urlInput or region. customKey, clientKey

    this.bucketPlaceholder =
      selectedStorage && selectedStorage.properties[0]?.title;

    this.forcePathStylePlaceholder = t("ForcePathStyle");

    this.regionPlaceholder =
      selectedStorage && selectedStorage.properties[2]?.title;

    this.serviceUrlPlaceholder = t("ServiceUrl");
    this.SSEPlaceholder = t("ServerSideEncryptionMethod");
    this.useHttpPlaceholder = t("UseHttp");

    this.sse_kms = "SSE-KMS";
    this.sse_s3 = "SSE-S3";

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

    this.region = [
      {
        key: "0",
        label: "None",
      },
      {
        key: "1",
        label: "Example-1",
      },
    ];

    this.managedKeys = [
      {
        key: "0",
        label: "Default AWS managed CMK",
      },
      {
        key: "1",
        label: "Сustomer managed CMK ",
      },
    ];
    this.state = {
      selectedEncryption: this.availableEncryptions[0],
      region: this.region[0],
      managedKeys: this.managedKeys[0],
    };
  }

  onSelectEncryptionMethod = (options) => {
    const key = options.key;
    const label = options.label;

    onSetAdditionInfo("sse", key);
    this.setState({
      selectedEncryption: { key, label },
    });
  };

  onSelectRegion = (options) => {
    const { replaceRequiredFormSettings } = this.props;

    const key = options.key;
    const label = options.label;

    onSetAdditionInfo("region", key);

    key === "0"
      ? replaceRequiredFormSettings(regionInput, urlInput)
      : replaceRequiredFormSettings(urlInput, regionInput);

    this.setState({
      region: { key, label },
    });
  };

  onSelectManagedKeys = (options) => {
    const { setFormSettings } = this.props;
    const key = options.key;
    const label = options.label;

    const newState = onSetAdditionInfo("managedkeys", key);
    setFormSettings(newState);

    this.setState({
      managedKeys: { key, label },
    });
  };

  onSelectEncryptionMode = (e) => {
    const { setFormSettings, onSetIsChanged } = this.props;
    const name = e.target.name;
    const nonCheck = name === "s3" ? "kms" : "s3"; //TODO: names from api
    const newState = onSelectEncryptionMode(name, nonCheck);
    onSetIsChanged && onSetIsChanged(true);
    setFormSettings(newState);
  };

  onUpdateValue = (e, type) => {
    const { formSettings, setFormSettings, onSetIsChanged } = this.props;
    onSetIsChanged && onSetIsChanged(true);

    const newState =
      type === "checkbox"
        ? onChangeCheckbox
        : onChangeTextInput(formSettings, e);

    setFormSettings(newState);
  };

  onChangeText = (e) => {
    this.onUpdateValue(e);
  };

  onChangeCheckbox = (e) => {
    this.onUpdateValue(e, "checkbox");
  };

  render() {
    const {
      errorsFieldsBeforeSafe: isError,
      isLoadingData,
      isLoading,
      formSettings,
      t,
      onChange,
    } = this.props;
    const { selectedEncryption, region, managedKeys } = this.state;
    console.log("amazon settings render", isError);
    const renderTooltip = (helpInfo) => {
      return (
        <>
          <HelpButton
            iconName={"/static/images/help.react.svg"}
            tooltipContent={
              <>
                <Trans t={t} i18nKey={`${helpInfo}`} ns="Settings">
                  {helpInfo}
                </Trans>
              </>
            }
          />
        </>
      );
    };
    return (
      <>
        <StyledBody>
          <div className="backup_storage-tooltip">
            <Text isBold>{this.bucketPlaceholder}</Text>
            {renderTooltip(t("AmazonServiceTip"))}
          </div>
          <TextInput
            name={bucketInput}
            className="backup_text-input"
            scale
            value={formSettings.bucket}
            hasError={isError?.bucket}
            onChange={this.onChangeText}
            isDisabled={isLoadingData || isLoading || this.isDisabled}
            tabIndex={1}
          />
        </StyledBody>
        <StyledBody>
          <div className="backup_storage-tooltip">
            <Text isBold>{this.regionPlaceholder}</Text>
            {renderTooltip(t("AmazonBucketTip"))}
          </div>
          <ComboBox
            className="backup_text-input"
            options={this.region}
            selectedOption={{
              key: 0,
              label: region.label,
            }}
            onSelect={this.onSelectRegion}
            noBorder={false}
            scaled={true}
            scaledOptions={true}
            dropDownMaxHeight={300}
            isDisabled={!!formSettings.serviceurl?.trim()}
            tabIndex={2}
          />
        </StyledBody>

        <StyledBody>
          <div className="backup_storage-tooltip">
            <Text isBold>{this.serviceUrlPlaceholder}</Text>
            {renderTooltip(t("AmazonRegionTip"))}
          </div>
          <TextInput
            name={urlInput}
            className="backup_text-input"
            scale
            value={formSettings.serviceurl}
            hasError={isError?.serviceurl}
            onChange={this.onChangeText}
            isDisabled={
              isLoadingData ||
              isLoading ||
              this.isDisabled ||
              region.key !== "0"
            }
            tabIndex={3}
          />
        </StyledBody>

        <Checkbox
          name={forcePathStyleInput}
          label={this.forcePathStylePlaceholder}
          isChecked={formSettings.forcepathstyle}
          isIndeterminate={false}
          isDisabled={false}
          onChange={this.onChangeCheckbox}
          tabIndex={4}
        />

        <Checkbox
          className="backup_checkbox"
          name={httpInput}
          label={this.useHttpPlaceholder}
          isChecked={formSettings.usehttp}
          isIndeterminate={false}
          isDisabled={false}
          onChange={this.onChangeCheckbox}
          tabIndex={5}
        />
        <StyledBody>
          <Text isBold>{this.SSEPlaceholder}</Text>

          <ComboBox
            className="backup_text-input"
            options={this.availableEncryptions}
            selectedOption={{
              key: 0,
              label: selectedEncryption.label,
            }}
            onSelect={this.onSelectEncryptionMethod}
            noBorder={false}
            scaled={true}
            scaledOptions={true}
            dropDownMaxHeight={50}
            tabIndex={7}
          />
        </StyledBody>
        {selectedEncryption.key === "1" && (
          <>
            <RadioButton
              value=""
              label={this.sse_kms}
              isChecked={formSettings.kms}
              onClick={this.onSelectEncryptionMode}
              name={"kms"}
            />

            <RadioButton
              className="backup_radio-button-settings"
              value=""
              label={this.sse_s3}
              isChecked={formSettings.s3}
              onClick={this.onSelectEncryptionMode}
              name={"s3"}
            />
          </>
        )}

        {formSettings.kms && selectedEncryption.key === "1" && (
          <>
            <Text isBold>{"Managed CMK"}</Text>
            <ComboBox
              className="backup_text-input"
              options={this.managedKeys}
              selectedOption={{
                key: 0,
                label: managedKeys.label,
              }}
              onSelect={this.onSelectManagedKeys}
              noBorder={false}
              scaled={true}
              scaledOptions={true}
              dropDownMaxHeight={50}
              tabIndex={8}
            />
          </>
        )}

        {managedKeys.key !== "0" && (
          <>
            <Text isBold>{"Key"}</Text>
            <TextInput
              name={customKey}
              className="backup_text-input"
              scale
              value={formSettings.customKey}
              hasError={isError?.customKey}
              onChange={this.onChangeText}
              isDisabled={isLoadingData || isLoading || this.isDisabled}
              tabIndex={9}
            />
          </>
        )}

        {selectedEncryption.key === "2" && (
          <>
            <Text isBold>{"Key"}</Text>
            <TextInput
              name={clientKey}
              className="backup_text-input"
              scale
              value={formSettings.clientKey}
              hasError={isError?.clientKey}
              onChange={this.onChangeText}
              isDisabled={isLoadingData || isLoading || this.isDisabled}
              tabIndex={8}
            />
          </>
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
    replaceRequiredFormSettings,
  } = backup;

  return {
    setFormSettings,
    setRequiredFormSettings,
    formSettings,
    errorsFieldsBeforeSafe,
    replaceRequiredFormSettings,
  };
})(observer(AmazonSettings));
