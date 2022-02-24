import React from "react";
import { withTranslation } from "react-i18next";
import ComboBox from "@appserver/components/combobox";
import { getBackupStorage } from "@appserver/common/api/settings";

import GoogleCloudStorage from "./storages/GoogleCloudStorage";
import RackspaceStorage from "./storages/RackspaceStorage";
import SelectelStorage from "./storages/SelectelStorage";
import AmazonStorage from "./storages/AmazonStorage";
import { getOptions } from "../../GetOptions";
import { ThirdPartyStorages } from "@appserver/common/constants";
import { getFromSessionStorage } from "../../../../../utils";
import { StyledManualBackup } from "../../StyledBackup";

let storage = "";
let storageId = "";
class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);

    this.isSetDefaultIdStorage = false;

    storage = getFromSessionStorage("LocalCopyThirdPartyStorageType");
    storageId = getFromSessionStorage("LocalCopyStorage");

    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedStorage: "",
      selectedId: "",
      isInitialLoading: true,
      isStartCopy: false,
    };

    this.isFirstSet = false;
  }
  componentDidMount() {
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

          selectedStorage: storage || selectedStorage,
          selectedId: storageId || selectedId,

          isInitialLoading: false,
        });
      })
      .catch(() => {
        this.setState({ isInitialLoading: false });
      });
  }

  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage } = this.state;

    const selectedStorage = availableStorage[selectedStorageId];
    this.setState({
      selectedStorage: selectedStorage.title,
      selectedId: selectedStorage.id,
    });
  };

  onMakeCopyIntoStorage = async (arraySettings) => {
    const { selectedId, selectedStorage } = this.state;
    const { onMakeCopy } = this.props;

    let obj = {};
    let inputValueArray = [];

    for (let i = 0; i < arraySettings.length; i++) {
      obj = {
        key: arraySettings[i][0],
        value: arraySettings[i][1],
      };

      inputValueArray.push(obj);
    }

    this.setState({
      isStartCopy: true,
    });

    await onMakeCopy(
      null,
      "ThirdPartyStorage",
      "5",
      "module",
      selectedId,
      inputValueArray,
      selectedId,
      selectedStorage
    );

    this.setState({
      isStartCopy: false,
    });
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
    const { isMaxProgress } = this.props;
    const {
      availableOptions,
      selectedStorage,
      isInitialLoading,
      selectedId,
      isStartCopy,
      availableStorage,
    } = this.state;

    const commonProps = {
      isLoadingData: !isMaxProgress || isStartCopy,
      selectedStorage: availableStorage[selectedId],
      isMaxProgress,
      selectedId,
      onMakeCopyIntoStorage: this.onMakeCopyIntoStorage,
      isInvalidForm: this.isInvalidForm,
    };

    const { GoogleId, RackspaceId, SelectelId, AmazonId } = ThirdPartyStorages;
    return (
      <StyledManualBackup>
        <div className="manual-backup_storages-module">
          <ComboBox
            options={availableOptions}
            selectedOption={{ key: 0, label: selectedStorage }}
            onSelect={this.onSelect}
            isDisabled={!isMaxProgress || isStartCopy || isInitialLoading}
            noBorder={false}
            scaled
            scaledOptions
            dropDownMaxHeight={400}
            className="backup_combo"
          />

          {selectedId === GoogleId && !isInitialLoading && (
            <GoogleCloudStorage {...commonProps} />
          )}

          {selectedId === RackspaceId && !isInitialLoading && (
            <RackspaceStorage {...commonProps} />
          )}

          {selectedId === SelectelId && !isInitialLoading && (
            <SelectelStorage {...commonProps} />
          )}

          {selectedId === AmazonId && !isInitialLoading && (
            <AmazonStorage {...commonProps} />
          )}
        </div>
      </StyledManualBackup>
    );
  }
}

export default withTranslation("Settings")(ThirdPartyStorageModule);
