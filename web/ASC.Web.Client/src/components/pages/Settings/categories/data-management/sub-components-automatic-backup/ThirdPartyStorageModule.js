import React from "react";
import { withTranslation } from "react-i18next";
import ComboBox from "@appserver/components/combobox";
import { getBackupStorage } from "@appserver/common/api/settings";
import GoogleCloudStorage from "./storages/GoogleCloudStorage";
import RackspaceStorage from "./storages/RackspaceStorage";
import SelectelStorage from "./storages/SelectelStorage";
import AmazonStorage from "./storages/AmazonStorage";
import { ThirdPartyStorages } from "@appserver/common/constants";
import { StyledAutoBackup } from "../StyledBackup";

let googleStorageId = ThirdPartyStorages.GoogleId;

class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);
    const { storageInfo } = this.props;

    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedStorage: "",
      defaultSelectedStorage: "",
      selectedId: "",
    };

    storageInfo && this.getOptions(storageInfo);
    this._isMount = false;
  }
  componentDidMount() {
    this._isMount = true;
  }

  componentWillUnmount() {
    this._isMount = false;
  }
  componentDidUpdate(prevProps) {
    const { isSuccessSave, isReset, storageInfo } = this.props;
    const {
      defaultSelectedStorage,
      selectedStorage,
      defaultSelectedId,
    } = this.state;

    if (isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      storageInfo && this.getOptions(storageInfo);
    }

    if (isReset && isReset !== prevProps.isReset) {
      if (defaultSelectedStorage !== selectedStorage) {
        this.setState({
          selectedId: defaultSelectedId,
          selectedStorage: defaultSelectedStorage,
        });
      }
    }
  }

  getOptions = (storageBackup) => {
    const { onSetStorageId } = this.props;

    let isSetDefaultIdStorage = false;
    let isFirstSet = false;
    let firstSetId = "";
    let options = [];
    let availableStorage = {};

    let newState = {};

    for (let item = 0; item < storageBackup.length; item++) {
      const backupElem = storageBackup[item];

      const { isSet, properties, title, id, current } = backupElem;

      let tempObj = {
        [id]: {
          isSet: isSet,
          properties: properties,
          title: title,
          id: id,
        },
      };

      let titleObj = {
        key: id,
        label: title,
        disabled: false,
      };

      options.push(titleObj);
      availableStorage = { ...availableStorage, ...tempObj };

      if (current) {
        isSetDefaultIdStorage = true;
        onSetStorageId(id);

        newState = {
          selectedStorage: title,
          defaultSelectedStorage: title,
          selectedId: id,
          defaultSelectedId: id,
        };
      }

      if (!isFirstSet && isSet) {
        isFirstSet = true;
        firstSetId = id;
      }
    }

    if (!isSetDefaultIdStorage && !isFirstSet) {
      const currentStorage = availableStorage[googleStorageId];
      const { id, title } = currentStorage;

      onSetStorageId(id);

      newState = {
        selectedStorage: title,
        defaultSelectedStorage: title,
        selectedId: id,
        defaultSelectedId: id,
      };
    }

    if (!isSetDefaultIdStorage && isFirstSet) {
      const currentStorage = availableStorage[firstSetId];
      const { id, title } = currentStorage;

      onSetStorageId(id);

      newState = {
        selectedStorage: title,
        defaultSelectedStorage: title,
        selectedId: id,
        defaultSelectedId: id,
      };
    }

    newState = {
      ...newState,
      availableOptions: options,
      availableStorage: availableStorage,
    };

    this._isMount
      ? this.setState({ ...newState })
      : (this.state = { ...newState });
  };

  checkChanges = () => {
    const { defaultSelectedStorage, selectedStorage } = this.state;
    const { onSetIsChanged } = this.props;

    if (defaultSelectedStorage !== selectedStorage) {
      onSetIsChanged(true);
    } else {
      onSetIsChanged(false);
    }
  };

  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage } = this.state;
    const { onSetStorageId } = this.props;
    const storage = availableStorage[selectedStorageId];

    onSetStorageId(storage.id);

    this.setState(
      {
        selectedStorage: storage.title,
        selectedId: storage.id,
      },
      () => {
        this.checkChanges();
      }
    );
  };

  render() {
    const { isLoadingData, isErrorsFields, ...rest } = this.props;
    const {
      availableOptions,
      availableStorage,
      selectedStorage,
      selectedId,
    } = this.state;

    const commonProps = {
      selectedStorage: availableStorage[selectedId],
      selectedId,
      formErrors: isErrorsFields,
      isLoadingData,
    };

    const { GoogleId, RackspaceId, SelectelId, AmazonId } = ThirdPartyStorages;

    return (
      <StyledAutoBackup>
        <div className="auto-backup_storages-module">
          <ComboBox
            options={availableOptions}
            selectedOption={{ key: 0, label: selectedStorage }}
            onSelect={this.onSelect}
            isDisabled={isLoadingData}
            noBorder={false}
            scaled
            scaledOptions
            dropDownMaxHeight={300}
            className="backup_combo"
          />

          {selectedId === GoogleId && (
            <GoogleCloudStorage {...rest} {...commonProps} />
          )}

          {selectedId === RackspaceId && (
            <RackspaceStorage {...rest} {...commonProps} />
          )}

          {selectedId === SelectelId && (
            <SelectelStorage {...rest} {...commonProps} />
          )}

          {selectedId === AmazonId && (
            <AmazonStorage {...rest} {...commonProps} />
          )}
        </div>
      </StyledAutoBackup>
    );
  }
}

export default withTranslation("Settings")(ThirdPartyStorageModule);
