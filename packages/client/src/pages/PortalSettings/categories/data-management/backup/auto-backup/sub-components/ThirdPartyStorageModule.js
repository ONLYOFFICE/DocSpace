import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import ComboBox from "@docspace/components/combobox";
import { ThirdPartyStorages } from "@docspace/common/constants";
import GoogleCloudStorage from "./storages/GoogleCloudStorage";
import RackspaceStorage from "./storages/RackspaceStorage";
import SelectelStorage from "./storages/SelectelStorage";
import AmazonStorage from "./storages/AmazonStorage";
import { StyledAutoBackup } from "../../StyledBackup";

let googleStorageId = ThirdPartyStorages.GoogleId;

class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);
    const { thirdPartyStorage } = this.props;

    this.state = {
      availableOptions: [],
      availableStorage: {},
    };

    thirdPartyStorage && this.getOptions(thirdPartyStorage);
    this._isMount = false;
  }
  componentDidMount() {
    this._isMount = true;
  }

  componentWillUnmount() {
    this._isMount = false;
  }
  componentDidUpdate(prevProps) {
    const { isSuccessSave, thirdPartyStorage } = this.props;

    if (isSuccessSave && isSuccessSave !== prevProps.isSuccessSave) {
      thirdPartyStorage && this.getOptions(thirdPartyStorage);
    }
  }

  getOptions = (storageBackup) => {
    const { setStorageId } = this.props;

    let isSetDefaultIdStorage = false;
    let isFirstSet = false;
    let firstSetId = "";
    let options = [];
    let availableStorage = {};

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
        !this._isMount ? setStorageId(null, id) : setStorageId(id);
      }

      if (!isFirstSet && isSet) {
        isFirstSet = true;
        firstSetId = id;
      }
    }

    if (!isSetDefaultIdStorage && !isFirstSet) {
      const currentStorage = availableStorage[googleStorageId];
      const { id } = currentStorage;

      !this._isMount ? setStorageId(null, id) : setStorageId(id);
    }

    if (!isSetDefaultIdStorage && isFirstSet) {
      const currentStorage = availableStorage[firstSetId];
      const { id } = currentStorage;

      !this._isMount ? setStorageId(null, id) : setStorageId(id);
    }

    this._isMount
      ? this.setState({
          availableOptions: options,
          availableStorage: availableStorage,
        })
      : (this.state = {
          availableOptions: options,
          availableStorage: availableStorage,
        });
  };

  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage } = this.state;
    const { setStorageId } = this.props;
    const storage = availableStorage[selectedStorageId];

    setStorageId(storage.id);
  };

  render() {
    const {
      isLoadingData,
      isErrorsFields,
      selectedStorageId,
      ...rest
    } = this.props;
    const { availableOptions, availableStorage } = this.state;

    const commonProps = {
      selectedStorage: availableStorage[selectedStorageId],
      selectedId: selectedStorageId,
      formErrors: isErrorsFields,
      isLoadingData,
    };

    const { GoogleId, RackspaceId, SelectelId, AmazonId } = ThirdPartyStorages;

    const storageTitle = availableStorage[selectedStorageId]?.title;

    return (
      <StyledAutoBackup>
        <div className="auto-backup_storages-module">
          <ComboBox
            options={availableOptions}
            selectedOption={{
              key: 0,
              label: storageTitle,
            }}
            onSelect={this.onSelect}
            isDisabled={isLoadingData}
            noBorder={false}
            scaled
            scaledOptions
            dropDownMaxHeight={300}
            className="backup_combo"
          />

          {selectedStorageId === GoogleId && (
            <GoogleCloudStorage {...rest} {...commonProps} />
          )}

          {selectedStorageId === RackspaceId && (
            <RackspaceStorage {...rest} {...commonProps} />
          )}

          {selectedStorageId === SelectelId && (
            <SelectelStorage {...rest} {...commonProps} />
          )}

          {selectedStorageId === AmazonId && (
            <AmazonStorage {...rest} {...commonProps} />
          )}
        </div>
      </StyledAutoBackup>
    );
  }
}

export default inject(({ backup }) => {
  const { thirdPartyStorage, setStorageId, selectedStorageId } = backup;

  return {
    thirdPartyStorage,
    setStorageId,
    selectedStorageId,
  };
})(withTranslation("Settings")(observer(ThirdPartyStorageModule)));
