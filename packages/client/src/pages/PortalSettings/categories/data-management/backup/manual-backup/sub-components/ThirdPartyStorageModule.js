import React from "react";
import { inject, observer } from "mobx-react";
import ComboBox from "@docspace/components/combobox";
import {
  BackupStorageType,
  ThirdPartyStorages,
} from "@docspace/common/constants";
import GoogleCloudStorage from "./storages/GoogleCloudStorage";
import RackspaceStorage from "./storages/RackspaceStorage";
import SelectelStorage from "./storages/SelectelStorage";
import AmazonStorage from "./storages/AmazonStorage";
import { getOptions } from "../../GetOptions";
import { getFromSessionStorage } from "../../../../../utils";
import { StyledManualBackup } from "../../StyledBackup";

let storage = "";
let storageId = "";
class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);

    storage = getFromSessionStorage("LocalCopyThirdPartyStorageType");
    storageId = getFromSessionStorage("LocalCopyStorage");

    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedStorage: "",
      selectedId: "",
      isStartCopy: false,
    };

    this.isFirstSet = false;
  }
  componentDidMount() {
    const { thirdPartyStorage } = this.props;

    if (thirdPartyStorage) {
      const parameters = getOptions(thirdPartyStorage);

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
      });
    }
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
    const { StorageModuleType } = BackupStorageType;

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
      `${StorageModuleType}`,
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
    const { isMaxProgress, thirdPartyStorage, buttonSize } = this.props;
    const {
      availableOptions,
      selectedStorage,
      selectedId,
      isStartCopy,
      availableStorage,
    } = this.state;

    const commonProps = {
      isLoadingData: !isMaxProgress || isStartCopy,
      selectedStorage: availableStorage[selectedId],
      isMaxProgress,
      selectedId,
      buttonSize,
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
            isDisabled={!isMaxProgress || isStartCopy || !!!thirdPartyStorage}
            noBorder={false}
            scaled
            scaledOptions
            dropDownMaxHeight={400}
            className="backup_combo"
          />

          {selectedId === GoogleId && <GoogleCloudStorage {...commonProps} />}

          {selectedId === RackspaceId && <RackspaceStorage {...commonProps} />}

          {selectedId === SelectelId && <SelectelStorage {...commonProps} />}

          {selectedId === AmazonId && <AmazonStorage {...commonProps} />}
        </div>
      </StyledManualBackup>
    );
  }
}

export default inject(({ backup }) => {
  const { thirdPartyStorage } = backup;

  return {
    thirdPartyStorage,
  };
})(observer(ThirdPartyStorageModule));
