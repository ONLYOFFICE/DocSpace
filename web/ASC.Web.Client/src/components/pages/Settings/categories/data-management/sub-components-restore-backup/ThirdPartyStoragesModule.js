import React from "react";
import { getOptions } from "../utils/getOptions";
import { getBackupStorage } from "../../../../../../../../../packages/asc-web-common/api/settings";
import ComboBox from "@appserver/components/combobox";
import { ThirdPartyStorages } from "@appserver/common/constants";
import GoogleCloudStorage from "./storages/GoogleCloudStorage";
import AmazonStorage from "./storages/AmazonStorage";
import RackspaceStorage from "./storages/RackspaceStorage";
import SelectelStorage from "./storages/SelectelStorage";

class ThirdPartyStoragesModule extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      availableOptions: [],
      availableStorage: "",

      selectedStorage: "",
      selectedId: "",
      isLoading: false,
    };
  }
  componentDidMount() {
    this.setState({ isLoading: true }, function () {
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
        .finally(() =>
          this.setState({
            isLoading: false,
          })
        );
    });
  }
  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage } = this.state;

    this.setState({
      selectedStorage: availableStorage[selectedStorageId].title,
      selectedId: availableStorage[selectedStorageId].id,
    });
  };
  render() {
    const {
      availableOptions,
      selectedStorage,
      isLoading,
      selectedId,
      availableStorage,
    } = this.state;
    const {
      onSetFormNames,
      formSettings,
      onChange,
      onResetFormSettings,
      isErrors,
    } = this.props;
    console.log("selectedId", selectedId);
    return (
      <>
        <ComboBox
          options={availableOptions}
          selectedOption={{ key: 0, label: selectedStorage }}
          onSelect={this.onSelect}
          isDisabled={isLoading}
          noBorder={false}
          scaled={true}
          scaledOptions={true}
          dropDownMaxHeight={400}
          className="backup_combo"
        />

        {selectedId === ThirdPartyStorages.GoogleId && (
          <GoogleCloudStorage
            isLoading={isLoading}
            availableStorage={availableStorage}
            selectedId={selectedId}
            onSetFormNames={onSetFormNames}
            formSettings={formSettings}
            onChange={onChange}
            onResetFormSettings={onResetFormSettings}
            isErrors={isErrors}
          />
        )}
        {selectedId === ThirdPartyStorages.RackspaceId && (
          <RackspaceStorage
            isLoading={isLoading}
            availableStorage={availableStorage}
            selectedId={selectedId}
            onSetFormNames={onSetFormNames}
            formSettings={formSettings}
            onChange={onChange}
            onResetFormSettings={onResetFormSettings}
            isErrors={isErrors}
          />
        )}
        {selectedId === ThirdPartyStorages.SelectelId && (
          <SelectelStorage
            isLoading={isLoading}
            availableStorage={availableStorage}
            selectedId={selectedId}
            onSetFormNames={onSetFormNames}
            formSettings={formSettings}
            onChange={onChange}
            onResetFormSettings={onResetFormSettings}
            isErrors={isErrors}
          />
        )}
        {selectedId === ThirdPartyStorages.AmazonId && (
          <AmazonStorage
            isLoading={isLoading}
            availableStorage={availableStorage}
            selectedId={selectedId}
            onSetFormNames={onSetFormNames}
            formSettings={formSettings}
            onChange={onChange}
            onResetFormSettings={onResetFormSettings}
            isErrors={isErrors}
          />
        )}
      </>
    );
  }
}

export default ThirdPartyStoragesModule;
