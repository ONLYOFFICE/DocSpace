import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import ComboBox from "@appserver/components/combobox";
import { getBackupStorage } from "@appserver/common/api/settings";

import GoogleCloudStorage from "./googleCloudStorage";
import RackspaceStorage from "./rackspaceStorage";
import SelectelStorage from "./selectelStorage";
import AmazonStorage from "./amazonStorage";
import { getOptions } from "../utils/getOptions";

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

let googleStorageId = "GoogleCloud";
let inputValueArray;

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

  fillInputValueArray = (inputNumber, valuesArray) => {
    const { selectedId, availableStorage } = this.state;
    const { onMakeCopy } = this.props;

    let obj = {};
    inputValueArray = [];

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

        {selectedId === "GoogleCloud" && !isLoading && (
          <GoogleCloudStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            maxProgress={maxProgress}
            selectedId={selectedId}
            fillInputValueArray={this.fillInputValueArray}
          />
        )}

        {selectedId === "Rackspace" && !isLoading && (
          <RackspaceStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            maxProgress={maxProgress}
            selectedId={selectedId}
            fillInputValueArray={this.fillInputValueArray}
          />
        )}

        {selectedId === "Selectel" && !isLoading && (
          <SelectelStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            maxProgress={maxProgress}
            selectedId={selectedId}
            fillInputValueArray={this.fillInputValueArray}
          />
        )}

        {selectedId === "S3" && !isLoading && (
          <AmazonStorage
            isLoadingData={isLoadingData}
            isLoading={isLoading}
            availableStorage={availableStorage}
            maxProgress={maxProgress}
            selectedId={selectedId}
            fillInputValueArray={this.fillInputValueArray}
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
