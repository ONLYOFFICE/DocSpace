import React from "react";
import { getOptions } from "../GetOptions";
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

    const commonProps = {
      isLoading,
      availableStorage,
      selectedId,
    };

    const { GoogleId, RackspaceId, SelectelId, AmazonId } = ThirdPartyStorages;
    return (
      <>
        <ComboBox
          options={availableOptions}
          selectedOption={{ key: 0, label: selectedStorage }}
          onSelect={this.onSelect}
          isDisabled={isLoading}
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
