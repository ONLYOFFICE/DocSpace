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

    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedStorage: "",
      defaultSelectedStorage: "",
      selectedId: "",
      isLoading: false,
    };
  }
  componentDidMount() {
    const { storageInfo } = this.props;

    storageInfo && this.getOptions(storageInfo);
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

    for (let item = 0; item < storageBackup.length; item++) {
      let obj = {
        [storageBackup[item].id]: {
          isSet: storageBackup[item].isSet,
          properties: storageBackup[item].properties,
          title: storageBackup[item].title,
          id: storageBackup[item].id,
        },
      };
      let titleObj = {
        key: storageBackup[item].id,
        label: storageBackup[item].title,
        disabled: false,
      };
      options.push(titleObj);

      availableStorage = { ...availableStorage, ...obj };

      if (storageBackup[item].current) {
        isSetDefaultIdStorage = true;
        onSetStorageId(storageBackup[item].id);
        this.setState({
          selectedStorage: storageBackup[item].title,
          defaultSelectedStorage: storageBackup[item].title,
          selectedId: storageBackup[item].id,
          defaultSelectedId: storageBackup[item].id,
        });
      }

      if (!isFirstSet && storageBackup[item].isSet) {
        isFirstSet = true;
        firstSetId = storageBackup[item].id;
      }
    }

    if (!isSetDefaultIdStorage && !isFirstSet) {
      onSetStorageId(availableStorage[googleStorageId].id);
      this.setState({
        selectedStorage: availableStorage[googleStorageId].title,
        defaultSelectedStorage: availableStorage[googleStorageId].title,
        selectedId: availableStorage[googleStorageId].id,
        defaultSelectedId: availableStorage[googleStorageId].id,
      });
    }

    if (!isSetDefaultIdStorage && isFirstSet) {
      onSetStorageId(availableStorage[firstSetId].id);

      this.setState({
        selectedStorage: availableStorage[firstSetId].title,
        defaultSelectedStorage: availableStorage[firstSetId].title,
        selectedId: availableStorage[firstSetId].id,
        defaultSelectedId: availableStorage[firstSetId].id,
      });
    }

    this.setState({
      availableOptions: options,
      availableStorage: availableStorage,
      isLoading: false,
    });
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
      isLoading,
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
            isDisabled={isLoadingData || isLoading}
            noBorder={false}
            scaled
            scaledOptions
            dropDownMaxHeight={300}
            className="backup_combo"
          />

          {selectedId === GoogleId && !isLoading && (
            <GoogleCloudStorage {...rest} {...commonProps} />
          )}

          {selectedId === RackspaceId && !isLoading && (
            <RackspaceStorage {...rest} {...commonProps} />
          )}

          {selectedId === SelectelId && !isLoading && (
            <SelectelStorage {...rest} {...commonProps} />
          )}

          {selectedId === AmazonId && !isLoading && (
            <AmazonStorage {...rest} {...commonProps} />
          )}
        </div>
      </StyledAutoBackup>
    );
  }
}

export default withTranslation("Settings")(ThirdPartyStorageModule);
