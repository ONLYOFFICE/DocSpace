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
import { getOptions } from "../../common-container/GetThirdPartyStoragesOptions";
import { getFromSessionStorage } from "../../../../../utils";
import { StyledManualBackup } from "../../StyledBackup";

let storage = "";
let storageId = "";
class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);

    // storage = getFromSessionStorage("LocalCopyThirdPartyStorageType");
    // storageId = getFromSessionStorage("LocalCopyStorage");

    this.state = {
      comboBoxOptions: [],
      storagesInfo: {},
      selectedStorageTitle: "",
      selectedId: "",
      isStartCopy: false,
    };

    this.isFirstSet = false;
  }
  componentDidMount() {
    const { thirdPartyStorage } = this.props;

    if (thirdPartyStorage && thirdPartyStorage.length > 0) {
      const parameters = getOptions(thirdPartyStorage);

      const {
        comboBoxOptions,
        storagesInfo,
        selectedStorageTitle,
        selectedStorageId,
      } = parameters;

      this.setState({
        comboBoxOptions,
        storagesInfo,
        selectedStorageTitle: storage || selectedStorageTitle,
        selectedId: storageId || selectedStorageId,
      });
    }
  }

  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { storagesInfo } = this.state;

    const selectedStorage = storagesInfo[selectedStorageId];
    this.setState({
      selectedStorageTitle: selectedStorage.title,
      selectedId: selectedStorage.id,
    });
  };

  onMakeCopyIntoStorage = async () => {
    const { selectedId, selectedStorageTitle } = this.state;
    const { onMakeCopy, isFormReady } = this.props;
    const { StorageModuleType } = BackupStorageType;

    if (!isFormReady()) return;

    this.setState({
      isStartCopy: true,
    });

    await onMakeCopy(
      null,
      "ThirdPartyStorage",
      `${StorageModuleType}`,
      selectedId,
      selectedStorageTitle
    );

    this.setState({
      isStartCopy: false,
    });
  };

  render() {
    const { isMaxProgress, thirdPartyStorage, buttonSize } = this.props;
    const {
      comboBoxOptions,
      selectedStorageTitle,
      selectedId,
      isStartCopy,
      storagesInfo,
    } = this.state;

    const commonProps = {
      isLoadingData: !isMaxProgress || isStartCopy,
      selectedStorage: storagesInfo[selectedId],
      isMaxProgress,
      selectedId,
      buttonSize,
      onMakeCopyIntoStorage: this.onMakeCopyIntoStorage,
    };

    const { GoogleId, RackspaceId, SelectelId, AmazonId } = ThirdPartyStorages;
    return (
      <StyledManualBackup>
        <div className="manual-backup_storages-module">
          <ComboBox
            options={comboBoxOptions}
            selectedOption={{ key: 0, label: selectedStorageTitle }}
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
  const { thirdPartyStorage, isFormReady } = backup;

  return {
    thirdPartyStorage,
    isFormReady,
  };
})(observer(ThirdPartyStorageModule));
