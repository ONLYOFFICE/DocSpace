import React from "react";
import { withTranslation } from "react-i18next";
import ComboBox from "@appserver/components/combobox";
import { getBackupStorage } from "@appserver/common/api/settings";

import GoogleCloudStorage from "./storages/GoogleCloudStorage";
import RackspaceStorage from "./storages/RackspaceStorage";
import SelectelStorage from "./storages/SelectelStorage";
import AmazonStorage from "./storages/AmazonStorage";
import { getOptions } from "../GetOptions";
import { ThirdPartyStorages } from "@appserver/common/constants";
import { getFromSessionStorage } from "../../../utils";
import { StyledManualBackup } from "../StyledBackup";

let selectedStorageFromSessionStorage = "";
let selectedIdFromSessionStorage = "";
class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);

    this.isSetDefaultIdStorage = false;

    selectedStorageFromSessionStorage = getFromSessionStorage(
      "selectedStorage"
    );
    selectedIdFromSessionStorage = getFromSessionStorage("selectedStorageId");

    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedStorage: "",
      selectedId: "",
      isInitialLoading: false,
    };

    this.isFirstSet = false;
  }
  componentDidMount() {
    const { onSetLoadingData } = this.props;

    onSetLoadingData && onSetLoadingData(true);
    this.setState(
      {
        isInitialLoading: true,
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

              selectedStorage:
                selectedStorageFromSessionStorage || selectedStorage,
              selectedId: selectedIdFromSessionStorage || selectedId,
            });
          })
          .finally(() => {
            onSetLoadingData && onSetLoadingData(false);
            this.setState({ isInitialLoading: false });
          });
      }
    );
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
    const { selectedId, availableStorage, selectedStorage } = this.state;
    const { onMakeCopy } = this.props;

    let obj = {};
    let inputValueArray = [];

    const availableStorageParams = availableStorage[selectedId];

    for (let i = 1; i <= inputNumber; i++) {
      obj = {
        key: availableStorageParams.properties[i - 1].name,
        value: valuesArray[i - 1],
      };
      inputValueArray.push(obj);
    }

    onMakeCopy(
      null,
      "thirdPartyStorage",
      "5",
      "module",
      selectedId,
      inputValueArray,
      selectedId,
      selectedStorage
    );
  };

  onMakeCopyIntoStorage = (valuesArray) => {
    const formSettings = [...valuesArray];
    const inputsNumber = formSettings.length;

    this.convertFormSettings(inputsNumber, formSettings);
  };

  isInvalidForm = (formSettings) => {
    let errors = {};
    let firstError = false;

    for (let key in formSettings) {
      const elem = formSettings[key];
      errors[key] = !elem.trim();

      if (!elem.trim() && !firstError) {
        firstError = true;
      }
    }

    return [firstError, errors];
  };
  render() {
    const { isLoadingData, isMaxProgress } = this.props;
    const {
      availableOptions,
      availableStorage,
      selectedStorage,
      isInitialLoading,
      selectedId,
    } = this.state;

    return (
      <StyledManualBackup>
        <ComboBox
          options={availableOptions}
          selectedOption={{ key: 0, label: selectedStorage }}
          onSelect={this.onSelect}
          isDisabled={isLoadingData || isInitialLoading}
          noBorder={false}
          scaled
          scaledOptions
          dropDownMaxHeight={400}
          className="backup_combo"
        />

        {selectedId === ThirdPartyStorages.GoogleId && !isInitialLoading && (
          <GoogleCloudStorage
            isLoadingData={isLoadingData}
            availableStorage={availableStorage}
            isMaxProgress={isMaxProgress}
            selectedId={selectedId}
            onMakeCopyIntoStorage={this.onMakeCopyIntoStorage}
            isInvalidForm={this.isInvalidForm}
          />
        )}

        {selectedId === ThirdPartyStorages.RackspaceId && !isInitialLoading && (
          <RackspaceStorage
            isLoadingData={isLoadingData}
            availableStorage={availableStorage}
            isMaxProgress={isMaxProgress}
            selectedId={selectedId}
            onMakeCopyIntoStorage={this.onMakeCopyIntoStorage}
            isInvalidForm={this.isInvalidForm}
          />
        )}

        {selectedId === ThirdPartyStorages.SelectelId && !isInitialLoading && (
          <SelectelStorage
            isLoadingData={isLoadingData}
            availableStorage={availableStorage}
            isMaxProgress={isMaxProgress}
            selectedId={selectedId}
            onMakeCopyIntoStorage={this.onMakeCopyIntoStorage}
            isInvalidForm={this.isInvalidForm}
          />
        )}

        {selectedId === ThirdPartyStorages.AmazonId && !isInitialLoading && (
          <AmazonStorage
            isLoadingData={isLoadingData}
            availableStorage={availableStorage}
            isMaxProgress={isMaxProgress}
            selectedId={selectedId}
            onMakeCopyIntoStorage={this.onMakeCopyIntoStorage}
            isInvalidForm={this.isInvalidForm}
          />
        )}
      </StyledManualBackup>
    );
  }
}

export default withTranslation("Settings")(ThirdPartyStorageModule);
