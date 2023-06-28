import HelpReactSvgUrl from "PUBLIC_DIR/images/help.react.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";
import TextInput from "@docspace/components/text-input";
import Checkbox from "@docspace/components/checkbox";
import ComboBox from "@docspace/components/combobox";
import RadioButton from "@docspace/components/radio-button";
import Text from "@docspace/components/text";
import styled from "styled-components";
import HelpButton from "@docspace/components/help-button";
import { Trans } from "react-i18next";

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
  svg {
    path {
      fill: ${(props) => props.theme.iconButton.color} !important;
    }
  }
`;
class AmazonSettings extends React.Component {
  static formNames = (systemName) => {
    return {
      bucket: "",
      region: systemName,
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
      defaultRegion,
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

    this.serverSideEncryption = t("AmazonSSE");
    this.clientSideEncryption = t("AmazonCSE");
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

    this.regions = [];
    let defaultRegionValue;

    for (let index = 0; index < storageRegions.length; ++index) {
      const item = storageRegions[index];

      this.regions.push({
        key: index.toString(),
        label: `${item.displayName} (${item.systemName})`,
        systemName: item.systemName,
      });

      if (defaultRegion === item.systemName) {
        defaultRegionValue = this.regions[index];
      }
    }

    this.state = {
      region: defaultRegionValue ? defaultRegionValue : this.regions[0],
    };
  }

  componentDidUpdate(prevProps) {
    const { formSettings } = this.props;

    if (formSettings[region] !== prevProps.formSettings[region]) {
      for (let value of this.regions) {
        if (value.systemName === formSettings[region]) {
          this.region = value.label;

          this.setState({
            region: value,
          });
          return;
        }
      }
    }
  }
  onSelectEncryptionMethod = (options) => {
    const {
      addValueInFormSettings,
      deleteValueFormSetting,
      setIsThirdStorageChanged,
    } = this.props;

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
    setIsThirdStorageChanged(true);
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
    const {
      addValueInFormSettings,
      deleteValueFormSetting,
      setIsThirdStorageChanged,
    } = this.props;
    const key = options.key;
    const label = options.label;

    if (label === this.customerManager) {
      addValueInFormSettings(sse_key, "");
    } else {
      deleteValueFormSetting(sse_key);
    }

    setIsThirdStorageChanged(true);
  };
  onSelectRegion = (options) => {
    const { addValueInFormSettings, setIsThirdStorageChanged } = this.props;

    const key = options.key;
    const label = options.label;
    const systemName = options.systemName;

    addValueInFormSettings(region, systemName);
    setIsThirdStorageChanged(true);
  };

  onChangeText = (event) => {
    const { addValueInFormSettings, setIsThirdStorageChanged } = this.props;
    const { target } = event;
    const value = target.value;
    const name = target.name;

    setIsThirdStorageChanged(true);

    addValueInFormSettings(name, value);
  };

  onChangeCheckbox = (event) => {
    const { addValueInFormSettings, setIsThirdStorageChanged } = this.props;
    const { target } = event;
    const value = target.checked;
    const name = target.name;

    setIsThirdStorageChanged(true);
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
      theme,
    } = this.props;
    const { region } = this.state;

    const renderTooltip = (helpInfo, className) => {
      return (
        <>
          <HelpButton
            className={className}
            offsetRight={0}
            iconName={HelpReactSvgUrl}
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

    const selectedEncryption =
      formSettings[sse] === sse_kms || formSettings[sse] === sse_s3
        ? this.availableEncryptions[1].label
        : formSettings.hasOwnProperty(sse)
        ? this.availableEncryptions[2].label
        : this.availableEncryptions[0].label;

    const managedKeys =
      formSettings[sse] === sse_kms
        ? formSettings.hasOwnProperty(sse_key)
          ? this.managedKeys[1]
          : this.managedKeys[0]
        : this.managedKeys[0];

    return (
      <>
        <StyledBody>
          <div className="backup_storage-tooltip">
            <Text isBold>{this.bucketPlaceholder}</Text>
            {renderTooltip(t("AmazonBucketTip"), "bucket-tooltip")}
          </div>
          <TextInput
            id="bucket-input"
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
            {renderTooltip(t("AmazonRegionTip"), "region-tooltip")}
          </div>
          <ComboBox
            className="region-combo-box backup_text-input"
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
            isDisabled={this.isDisabled}
            tabIndex={2}
            showDisabledItems
          />
        </StyledBody>

        <StyledBody>
          <div className="backup_storage-tooltip">
            <Text isBold>{this.serviceUrlPlaceholder}</Text>
            {renderTooltip(t("AmazonServiceTip"), "service-tooltip")}
          </div>
          <TextInput
            id="service-url-input"
            name={serviceurl}
            className="backup_text-input"
            scale
            value={formSettings[serviceurl]}
            hasError={isError[serviceurl]}
            onChange={this.onChangeText}
            isDisabled={isLoadingData || isLoading || this.isDisabled}
            tabIndex={3}
          />
        </StyledBody>

        <StyledBody theme={theme}>
          <Checkbox
            id="force-path-style"
            name={forcepathstyle}
            label={this.forcePathStylePlaceholder}
            isChecked={formSettings[forcepathstyle] === "false" ? false : true}
            isIndeterminate={false}
            isDisabled={this.isDisabled}
            onChange={this.onChangeCheckbox}
            tabIndex={4}
            helpButton={
              <div className="backup_storage-tooltip">
                {renderTooltip(
                  t("AmazonForcePathStyleTip"),
                  "force-path-style-tooltip"
                )}
              </div>
            }
          />
        </StyledBody>
        <StyledBody theme={theme}>
          <Checkbox
            id="use-http"
            className="backup_checkbox"
            name={usehttp}
            label={this.useHttpPlaceholder}
            isChecked={formSettings[usehttp] === "false" ? false : true}
            isIndeterminate={false}
            isDisabled={this.isDisabled}
            onChange={this.onChangeCheckbox}
            tabIndex={5}
            helpButton={
              <div className="backup_storage-tooltip">
                {renderTooltip(t("AmazonHTTPTip"), "http-tooltip")}
              </div>
            }
          />
        </StyledBody>
        <StyledBody>
          <div className="backup_storage-tooltip">
            <Text isBold>{this.SSEPlaceholder}</Text>
            {renderTooltip(t("AmazonSSETip"), "sse-method-tooltip")}
          </div>
          <ComboBox
            className="sse-method-combo-box backup_text-input"
            options={this.availableEncryptions}
            selectedOption={{
              key: 0,
              label: selectedEncryption,
            }}
            onSelect={this.onSelectEncryptionMethod}
            noBorder={false}
            scaled={true}
            scaledOptions={true}
            dropDownMaxHeight={300}
            isDisabled={this.isDisabled}
            tabIndex={7}
            showDisabledItems
          />
        </StyledBody>

        {selectedEncryption === this.serverSideEncryption && (
          <>
            <RadioButton
              id="sse-s3"
              className="backup_radio-button-settings"
              value=""
              label={this.sse_s3}
              isChecked={formSettings[sse] === sse_s3 ? true : false}
              onClick={this.onSelectEncryptionMode}
              name={sse_s3}
              isDisabled={this.isDisabled}
            />

            <RadioButton
              id="sse-kms"
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
                  className="managed-cmk-combo-box backup_text-input"
                  options={this.managedKeys}
                  selectedOption={{
                    key: 0,
                    label: managedKeys.label,
                    systemName: managedKeys.systemName,
                  }}
                  onSelect={this.onSelectManagedKeys}
                  noBorder={false}
                  scaled
                  scaledOptions
                  dropDownMaxHeight={300}
                  isDisabled={this.isDisabled}
                  tabIndex={8}
                  showDisabledItems
                />

                {managedKeys.label === this.customerManager && (
                  <>
                    <Text isBold>{"KMS Key Id:"}</Text>
                    <TextInput
                      id="customer-manager-kms-key-id"
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

        {selectedEncryption === this.clientSideEncryption && (
          <>
            <Text isBold>{"KMS Key Id:"}</Text>
            <TextInput
              id="client-side-encryption-kms-key-id"
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
            id="file-path-input"
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

export default inject(({ auth, backup }) => {
  const {
    setRequiredFormSettings,
    formSettings,
    errorsFieldsBeforeSafe,

    setIsThirdStorageChanged,
    addValueInFormSettings,
    deleteValueFormSetting,
    storageRegions,
    requiredFormSettings,
    defaultFormSettings,
  } = backup;
  const defaultRegion = defaultFormSettings.region;
  const { theme } = auth.settingsStore;
  return {
    setRequiredFormSettings,
    formSettings,
    errorsFieldsBeforeSafe,
    storageRegions,
    setIsThirdStorageChanged,
    addValueInFormSettings,
    deleteValueFormSetting,
    defaultRegion,
    requiredFormSettings,
    theme,
  };
})(observer(AmazonSettings));
