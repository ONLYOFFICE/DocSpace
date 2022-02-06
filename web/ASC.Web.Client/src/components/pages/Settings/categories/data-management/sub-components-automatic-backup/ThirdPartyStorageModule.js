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
let inputValueArray;

class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);

    this.isSetDefaultIdStorage = false;

    this.state = {
      availableOptions: [],
      availableStorage: {},
      selectedStorage: "",
      defaultSelectedStorage: "",
      selectedId: "",
      isLoading: false,
      isChangedThirdParty: false,
    };
    this.isFirstSet = false;
    this.firstSetId = "";
    this._isMounted = false;
  }
  componentDidMount() {
    this._isMounted = true;
    const { onSetLoadingData, checkChanges } = this.props;

    onSetLoadingData && onSetLoadingData(true);
    this.setState(
      {
        isLoading: true,
      },
      function () {
        getBackupStorage()
          .then((storageBackup) => this.getOptions(storageBackup))
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
  getOptions = (storageBackup) => {
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
        this.isSetDefaultIdStorage = true;

        this.setState({
          selectedStorage: storageBackup[item].title,
          defaultSelectedStorage: storageBackup[item].title,
          selectedId: storageBackup[item].id,
          defaultSelectedId: storageBackup[item].id,
        });
      }

      if (!this.isFirstSet && storageBackup[item].isSet) {
        this.isFirstSet = true;
        this.firstSetId = storageBackup[item].id;
      }
    }

    if (!this.isSetDefaultIdStorage && !this.isFirstSet) {
      this.setState({
        selectedStorage: availableStorage[googleStorageId].title,
        defaultSelectedStorage: availableStorage[googleStorageId].title,
        selectedId: availableStorage[googleStorageId].id,
        defaultSelectedId: availableStorage[googleStorageId].id,
      });
    }

    if (!this.isSetDefaultIdStorage && this.isFirstSet) {
      this.setState({
        selectedStorage: availableStorage[this.firstSetId].title,
        defaultSelectedStorage: availableStorage[this.firstSetId].title,
        selectedId: availableStorage[this.firstSetId].id,
        defaultSelectedId: availableStorage[this.firstSetId].id,
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

    if (defaultSelectedStorage !== selectedStorage) {
      this.setState({
        isChangedThirdParty: true,
      });
    } else {
      this.setState({ isChangedThirdParty: false });
    }
  };
  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage } = this.state;

    this.setState(
      {
        selectedStorage: availableStorage[selectedStorageId].title,
        selectedId: availableStorage[selectedStorageId].id,
      },
      function () {
        this.checkChanges();
      }
    );
  };

  convertSettings = (inputNumber, valuesArray) => {
    const { selectedId, availableStorage } = this.state;
    let obj = {};
    inputValueArray = [];

    const selectedStorage = availableStorage[selectedId];

    for (let i = 1; i <= inputNumber; i++) {
      obj = {
        key: selectedStorage.properties[i - 1]?.name,
        value: valuesArray[i - 1],
      };
      inputValueArray.push(obj);
    }
    this.onSaveModuleSettings();
  };

  onSaveModuleSettings = async () => {
    const { onSaveModuleSettings, onSetLoadingData } = this.props;

    const { selectedId, availableStorage } = this.state;

    await onSaveModuleSettings(selectedId, inputValueArray);
    this.isSetDefaultIdStorage = true;

    this._isMounted &&
      this.setState({
        defaultSelectedId: selectedId,
        defaultSelectedStorage: availableStorage[selectedId].title,
        isChangedThirdParty: false,
      });
  };

  onCancelModuleSettings = () => {
    const { onCancelModuleSettings } = this.props;
    const {
      defaultSelectedStorage,
      selectedStorage,
      defaultSelectedId,
    } = this.state;

    onCancelModuleSettings();

    if (defaultSelectedStorage !== selectedStorage) {
      this.setState({
        selectedId: defaultSelectedId,
        selectedStorage: defaultSelectedStorage,
        isChangedThirdParty: false,
      });
    }
  };
  isInvalidForm = (formSettings) => {
    let errors = {};
    let firstError = false;

    for (let key in formSettings) {
      const elem = formSettings[key];
      errors[key] = elem ? !elem.trim() : true;

      if (errors[key] && !firstError) {
        firstError = true;
      }
    }

    return [firstError, errors];
  };

  render() {
    const { isLoadingData, ...rest } = this.props;
    const {
      availableOptions,
      availableStorage,
      selectedStorage,
      isLoading,
      selectedId,
      isChangedThirdParty,
    } = this.state;

    const commonProps = {
      selectedStorage: availableStorage[selectedId],
      convertSettings: this.convertSettings,
      isInvalidForm: this.isInvalidForm,
      onCancelModuleSettings: this.onCancelModuleSettings,
      isChangedThirdParty: isChangedThirdParty,
    };

    const { GoogleId, RackspaceId, SelectelId, AmazonId } = ThirdPartyStorages;

    return (
      <StyledAutoBackup>
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
          <GoogleCloudStorage
            isLoadingData={isLoadingData}
            {...rest}
            {...commonProps}
          />
        )}

        {selectedId === RackspaceId && !isLoading && <RackspaceStorage />}

        {selectedId === SelectelId && !isLoading && (
          <SelectelStorage
            isLoadingData={isLoadingData}
            {...rest}
            {...commonProps}
          />
        )}

        {selectedId === AmazonId && !isLoading && (
          <AmazonStorage
            isLoadingData={isLoadingData}
            {...rest}
            {...commonProps}
          />
        )}
      </StyledAutoBackup>
    );
  }
}

export default withTranslation("Settings")(ThirdPartyStorageModule);
