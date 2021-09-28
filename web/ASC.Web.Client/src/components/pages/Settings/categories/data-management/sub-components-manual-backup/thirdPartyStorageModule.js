import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import ComboBox from "@appserver/components/combobox";
import { getBackupStorage } from "@appserver/common/api/settings";

import GoogleCloudStorage from "./storages/googleCloudStorage";
import RackspaceStorage from "./storages/rackspaceStorage";
import SelectelStorage from "./storages/selectelStorage";
import AmazonStorage from "./storages/amazonStorage";
import { getOptions } from "../utils/getOptions";
import { saveToSessionStorage } from "../../../utils";
import { ThirdPartyStorages } from "@appserver/common/constants";

const StyledComponent = styled.div`
  .backup_combo {
    margin-top: 16px;
    width: 100%;
    max-width: 820px;
    .combo-button-label {
      width: 100%;
      max-width: 820px;
    }
  }
  .backup_text-input {
    margin: 10px 0;
    width: 100%;
    max-width: 820px;
  }
`;

class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);

    this.isSetDefaultIdStorage = false;

    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedStorage: "",
      selectedId: "",
      isLoading: false,
    };

    this.isFirstSet = false;
    this.firstSetId = "";
    this._isMounted = false;
  }
  componentDidMount() {
    this._isMounted = true;
    const { onSetLoadingData } = this.props;

    onSetLoadingData && onSetLoadingData(true);
    this.setState(
      {
        isLoading: true,
      },
      function () {
        getBackupStorage()
          .then((storageBackup) => {
            const parameters = getOptions(storageBackup);

            const {
              options,
              availableStorage,
              selectedStorage,
              selectedId,
            } = parameters;

            this.setState({
              availableOptions: options,
              availableStorage: availableStorage,

              selectedStorage: selectedStorage,
              selectedId: selectedId,
            });
          })
          .finally(() => {
            onSetLoadingData && onSetLoadingData(false);
            this.setState({ isLoading: false });
          });
      }
    );
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage } = this.state;

    this.setState({
      selectedStorage: availableStorage[selectedStorageId].title,
      selectedId: availableStorage[selectedStorageId].id,
    });
  };

  convertFormSettings = (inputNumber, valuesArray) => {
    const { selectedId, availableStorage } = this.state;
    const { onMakeCopy } = this.props;

    let obj = {};
    let inputValueArray = [];

    const selectedStorage = availableStorage[selectedId];

    for (let i = 1; i <= inputNumber; i++) {
      obj = {
        key: selectedStorage.properties[i - 1].name,
        value: valuesArray[i - 1],
      };
      inputValueArray.push(obj);
    }

    onMakeCopy(null, null, "5", "module", selectedId, inputValueArray);
  };

  onMakeCopyIntoStorage = (valuesArray) => {
    saveToSessionStorage("selectedManualStorageType", "thirdPartyStorage");

    const formSettings = [...valuesArray];
    const inputsNumber = formSettings.length;

    this.convertFormSettings(inputsNumber, formSettings);
  };

  isInvalidForm = (formSettings) => {
    for (let key in formSettings) {
      const elem = formSettings[key];

      if (!elem.trim()) {
        return true;
      }
    }

    return false;
  };

  render() {
    const { t, helpUrlCreatingBackup, isLoadingData, maxProgress } = this.props;
    const {
      availableOptions,
      availableStorage,
      selectedStorage,
      isLoading,
      selectedId,
    } = this.state;

    return (
      <StyledComponent>
        <Box marginProp="16px 0 16px 0">
          <Link
            color="#316DAA"
            target="_blank"
            isHovered={true}
            href={helpUrlCreatingBackup}
          >
            {t("Common:LearnMore")}
          </Link>
        </Box>
        <ComboBox
          options={availableOptions}
          selectedOption={{ key: 0, label: selectedStorage }}
          onSelect={this.onSelect}
          isDisabled={isLoadingData || isLoading}
          noBorder={false}
          scaled={true}
          scaledOptions={true}
          dropDownMaxHeight={400}
          className="backup_combo"
        />

        {selectedId === ThirdPartyStorages.GoogleId && !isLoading && (
          <GoogleCloudStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            maxProgress={maxProgress}
            selectedId={selectedId}
            onMakeCopyIntoStorage={this.onMakeCopyIntoStorage}
            isInvalidForm={this.isInvalidForm}
          />
        )}

        {selectedId === ThirdPartyStorages.RackspaceId && !isLoading && (
          <RackspaceStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            maxProgress={maxProgress}
            selectedId={selectedId}
            onMakeCopyIntoStorage={this.onMakeCopyIntoStorage}
            isInvalidForm={this.isInvalidForm}
          />
        )}

        {selectedId === ThirdPartyStorages.SelectelId && !isLoading && (
          <SelectelStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            maxProgress={maxProgress}
            selectedId={selectedId}
            onMakeCopyIntoStorage={this.onMakeCopyIntoStorage}
            isInvalidForm={this.isInvalidForm}
          />
        )}

        {selectedId === ThirdPartyStorages.AmazonId && !isLoading && (
          <AmazonStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            maxProgress={maxProgress}
            selectedId={selectedId}
            onMakeCopyIntoStorage={this.onMakeCopyIntoStorage}
            isInvalidForm={this.isInvalidForm}
          />
        )}
      </StyledComponent>
    );
  }
}

export default inject(({ auth }) => {
  const { helpUrlCreatingBackup } = auth.settingsStore;

  return {
    helpUrlCreatingBackup,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyStorageModule)));
