import React from "react";
import { inject, observer } from "mobx-react";
import ComboBox from "@docspace/components/combobox";
import { ThirdPartyStorages } from "@docspace/common/constants";
import GoogleCloudStorage from "./storages/GoogleCloudStorage";
import AmazonStorage from "./storages/AmazonStorage";
import RackspaceStorage from "./storages/RackspaceStorage";
import SelectelStorage from "./storages/SelectelStorage";
import { getOptions } from "../../GetOptions";
class ThirdPartyStoragesModule extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      availableOptions: [],
      availableStorage: "",

      selectedStorage: "",
      selectedId: "",
    };
  }
  componentDidMount() {
    const { onSetStorageId, thirdPartyStorage } = this.props;

    if (thirdPartyStorage) {
      const parameters = getOptions(thirdPartyStorage);

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
      });
    }
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
      selectedId,
      availableStorage,
    } = this.state;
    const { thirdPartyStorage } = this.props;

    const commonProps = {
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
          isDisabled={!!!thirdPartyStorage}
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

export default inject(({ backup }) => {
  const { thirdPartyStorage } = backup;

  return {
    thirdPartyStorage,
  };
})(observer(ThirdPartyStoragesModule));
