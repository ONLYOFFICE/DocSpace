import React from "react";
import { inject, observer } from "mobx-react";
import TextInput from "@appserver/components/text-input";
import Checkbox from "@appserver/components/checkbox";
import ComboBox from "@appserver/components/combobox";
import RadioButton from "@appserver/components/radio-button";
import Text from "@appserver/components/text";
import styled from "styled-components";
import HelpButton from "@appserver/components/help-button";
import { Trans } from "react-i18next";
import { onSetAdditionInfo } from "./InputsMethods";

const bucket = "bucket";
const region = "region";
const serviceurl = "serviceurl";
const forcepathstyle = "forcepathstyle";
const usehttp = "usehttp";
const sse = "sse";
const sse_kms = "awskms";
const sse_key = "ssekey";
const sse_s3 = "aes256";
const sse_client_side = "clientawskms";

const filePath = "filePath";

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
  static formNames = (systemName) => {
    return {
      bucket: "",
      region: systemName ? systemName : "",
      serviceurl: "",
      forcepathstyle: "false",
      usehttp: "false",
    };
  };

  constructor(props) {
    super(props);
    const {
      t,
      selectedStorage,
      setRequiredFormSettings,
      setIsThirdStorageChanged,
      isNeedFilePath,
      storageRegions,
    } = this.props;

    this.isDisabled = selectedStorage && !selectedStorage.isSet;

    const filePathField = isNeedFilePath ? [filePath] : [];
    setRequiredFormSettings([bucket, ...filePathField]);
    setIsThirdStorageChanged(false);

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

    this.serverSideEncryption = "Server-Side Encryption";
    this.clientSideEncryption = "Client-Side Encryption";
    this.defaultManaged = "Default AWS managed CMK";
    this.customerManager = "Customer manager CMK";
    this.noneValue = "None";

    this.availableEncryptions = [
      {
        key: "0",
        label: this.noneValue,
      },
      {
        key: "1",
        label: this.serverSideEncryption,
      },
      {
        key: "2",
        label: this.clientSideEncryption,
      },
    ];

    this.regions = [];
    for (let index = 0; index < storageRegions.length; ++index) {
      const item = storageRegions[index];
      this.regions.push({
        key: index.toString(),
        label: `${item.displayName} (${item.systemName})`,
        systemName: item.systemName,
      });
    }

    this.managedKeys = [
      {
        key: "0",
        label: this.defaultManaged,
      },
      {
        key: "1",
        label: this.customerManager,
      },
    ];
    this.state = {
      selectedEncryption: this.availableEncryptions[0],
      region: this.regions[0],
      managedKeys: this.managedKeys[0],
    };
  }

  onSelectEncryptionMethod = (options) => {
    const { addValueInFormSettings, deleteValueFormSetting } = this.props;
    const key = options.key;
    const label = options.label;

    if (label === this.noneValue) {
      deleteValueFormSetting(sse_key);
      deleteValueFormSetting(sse);
    } else {
      const isServerSSE = label === this.serverSideEncryption;
      const value = isServerSSE ? sse_s3 : sse_client_side;
      addValueInFormSettings(sse, value);

      if (!isServerSSE) {
        addValueInFormSettings(sse_key, "");
      } else {
        deleteValueFormSetting(sse_key);
      }
    }

    this.setState({
      selectedEncryption: { key, label },
    });
  };

  onSelectEncryptionMode = (e) => {
    const {
      setIsThirdStorageChanged,
      addValueInFormSettings,
      deleteValueFormSetting,
    } = this.props;

    const value = e.target.name;

    if (value === sse_s3) {
      deleteValueFormSetting(sse_key);
    }
    addValueInFormSettings(sse, value);
    setIsThirdStorageChanged(true);
  };
  onSelectManagedKeys = (options) => {
    const { addValueInFormSettings, deleteValueFormSetting } = this.props;
    const key = options.key;
    const label = options.label;

    if (label === this.customerManager) {
      addValueInFormSettings(sse_key, "");
    } else {
      deleteValueFormSetting(sse_key);
    }

    this.setState({
      managedKeys: { key, label },
    });
  };
  onSelectRegion = (options) => {
    //const { replaceRequiredFormSettings } = this.props;

    const key = options.key;
    const label = options.label;

    onSetAdditionInfo("region", key);

    // key === "0"
    //   ? replaceRequiredFormSettings(region, serviceurl)
    //   : replaceRequiredFormSettings(serviceurl, region);

    this.setState({
      region: { key, label },
    });
  };

  onChangeText = (event) => {
    const { addValueInFormSettings } = this.props;
    const { target } = event;
    const value = target.value;
    const name = target.name;

    addValueInFormSettings(name, value);
  };

  onChangeCheckbox = (event) => {
    const { addValueInFormSettings } = this.props;
    const { target } = event;
    const value = target.checked;
    const name = target.name;

    addValueInFormSettings(name, value.toString());
  };

  render() {
    const {
      errorsFieldsBeforeSafe: isError,
      isLoadingData,
      isLoading,
      formSettings,
      t,
      isNeedFilePath,
      requiredFormSettings,
    } = this.props;
    const { selectedEncryption, region, managedKeys } = this.state;
    console.log(
      "amazon settings render",
      formSettings,
      "requiredFormSettings",
      requiredFormSettings
    );
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
            name={bucket}
            className="backup_text-input"
            scale
            value={formSettings[bucket]}
            hasError={isError[bucket]}
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
            options={this.regions}
            selectedOption={{
              key: 0,
              label: region.label,
            }}
            onSelect={this.onSelectRegion}
            noBorder={false}
            scaled={true}
            scaledOptions={true}
            dropDownMaxHeight={300}
            isDisabled={!!formSettings[serviceurl]?.trim() || this.isDisabled}
            tabIndex={2}
          />
        </StyledBody>

        <StyledBody>
          <div className="backup_storage-tooltip">
            <Text isBold>{this.serviceUrlPlaceholder}</Text>
            {renderTooltip(t("AmazonRegionTip"))}
          </div>
          <TextInput
            name={serviceurl}
            className="backup_text-input"
            scale
            value={formSettings[serviceurl]}
            hasError={isError[serviceurl]}
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
          name={forcepathstyle}
          label={this.forcePathStylePlaceholder}
          isChecked={formSettings[forcepathstyle] === "false" ? false : true}
          isIndeterminate={false}
          isDisabled={this.isDisabled}
          onChange={this.onChangeCheckbox}
          tabIndex={4}
        />

        <Checkbox
          className="backup_checkbox"
          name={usehttp}
          label={this.useHttpPlaceholder}
          isChecked={formSettings[usehttp] === "false" ? false : true}
          isIndeterminate={false}
          isDisabled={this.isDisabled}
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
            dropDownMaxHeight={300}
            isDisabled={this.isDisabled}
            tabIndex={7}
          />
        </StyledBody>

        {selectedEncryption.label === this.serverSideEncryption && (
          <>
            <RadioButton
              className="backup_radio-button-settings"
              value=""
              label={this.sse_s3}
              isChecked={formSettings[sse] === sse_s3 ? true : false}
              onClick={this.onSelectEncryptionMode}
              name={sse_s3}
              isDisabled={this.isDisabled}
            />

            <RadioButton
              className="backup_radio-button-settings"
              value=""
              label={this.sse_kms}
              isChecked={formSettings[sse] === sse_kms ? true : false}
              onClick={this.onSelectEncryptionMode}
              name={sse_kms}
              isDisabled={this.isDisabled}
            />

            {formSettings[sse] === sse_kms && (
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
                  scaled
                  scaledOptions
                  dropDownMaxHeight={300}
                  isDisabled={this.isDisabled}
                  tabIndex={8}
                />

                {managedKeys.label === this.customerManager && (
                  <>
                    <Text isBold>{"Key"}</Text>
                    <TextInput
                      name={sse_key}
                      className="backup_text-input"
                      scale
                      value={formSettings[sse_key]}
                      hasError={isError[sse_key]}
                      onChange={this.onChangeText}
                      isDisabled={isLoadingData || isLoading || this.isDisabled}
                      tabIndex={9}
                    />
                  </>
                )}
              </>
            )}
          </>
        )}

        {selectedEncryption.label === this.clientSideEncryption && (
          <>
            <Text isBold>{"KMS Key Id"}</Text>
            <TextInput
              name={sse_key}
              className="backup_text-input"
              scale
              value={formSettings[sse_key]}
              hasError={isError[sse_key]}
              onChange={this.onChangeText}
              isDisabled={isLoadingData || isLoading || this.isDisabled}
              tabIndex={8}
            />
          </>
        )}

        {isNeedFilePath && (
          <TextInput
            name="filePath"
            className="backup_text-input"
            scale
            value={formSettings[filePath]}
            onChange={this.onChangeText}
            isDisabled={isLoadingData || isLoading || this.isDisabled}
            placeholder={t("Path")}
            hasError={isError[filePath]}
          />
        )}
      </>
    );
  }
}

export default inject(({ backup }) => {
  const {
    setRequiredFormSettings,
    formSettings,
    errorsFieldsBeforeSafe,

    setIsThirdStorageChanged,
    addValueInFormSettings,
    deleteValueFormSetting,
    storageRegions,
    requiredFormSettings,
  } = backup;

  return {
    setRequiredFormSettings,
    formSettings,
    errorsFieldsBeforeSafe,
    storageRegions,
    setIsThirdStorageChanged,
    addValueInFormSettings,
    deleteValueFormSetting,

    requiredFormSettings,
  };
})(observer(AmazonSettings));
