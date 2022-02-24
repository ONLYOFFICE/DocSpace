import React from "react";
import { getOptions } from "../../GetOptions";
import { getBackupStorage } from "@appserver/common/api/settings";
import ComboBox from "@appserver/components/combobox";
import { ThirdPartyStorages } from "@appserver/common/constants";
import GoogleCloudStorage from "./storages/GoogleCloudStorage";
import AmazonStorage from "./storages/AmazonStorage";
import RackspaceStorage from "./storages/RackspaceStorage";
import SelectelStorage from "./storages/SelectelStorage";

class ThirdPartyStoragesModule extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      availableOptions: [],
      availableStorage: "",

      selectedStorage: "",
      selectedId: "",
      isInitialLoading: true,
    };
  }
  componentDidMount() {
    const { onSetStorageId } = this.props;

    getBackupStorage()
      .then((storageBackup) => {
        const parameters = getOptions(storageBackup);

        const {
          options,
          availableStorage,
          selectedStorage,
          selectedId,
        } = parameters;

        onSetStorageId && onSetStorageId(selectedId);

        this.setState({
          availableOptions: options,
          availableStorage,

          selectedStorage,
          selectedId,

          isInitialLoading: false,
        });
      })
      .catch(() =>
        this.setState({
          isInitialLoading: false,
        })
      );
  }
  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { availableStorage } = this.state;
    const { onSetStorageId } = this.props;
    const storage = availableStorage[selectedStorageId];

    onSetStorageId && onSetStorageId(storage.id);

    this.setState({
      selectedStorage: storage.title,
      selectedId: storage.id,
    });
  };
  render() {
    const {
      availableOptions,
      selectedStorage,
      isInitialLoading,
      selectedId,
      availableStorage,
    } = this.state;

    const commonProps = {
      isInitialLoading,
      availableStorage,
      selectedId,
    };

    const { GoogleId, RackspaceId, SelectelId, AmazonId } = ThirdPartyStorages;

    console.log("render storage", this.state, this.props);
    return (
      <>
        <ComboBox
          options={availableOptions}
          selectedOption={{ key: 0, label: selectedStorage }}
          onSelect={this.onSelect}
          isDisabled={isInitialLoading}
          noBorder={false}
          scaled
          scaledOptions
          dropDownMaxHeight={400}
          className="backup_combo"
        />

        {selectedId === GoogleId && (
          <GoogleCloudStorage {...commonProps} {...this.props} />
        )}
        {selectedId === RackspaceId && (
          <RackspaceStorage {...commonProps} {...this.props} />
        )}
        {selectedId === SelectelId && (
          <SelectelStorage {...commonProps} {...this.props} />
        )}
        {selectedId === AmazonId && (
          <AmazonStorage {...commonProps} {...this.props} />
        )}
      </>
    );
  }
}

export default ThirdPartyStoragesModule;
