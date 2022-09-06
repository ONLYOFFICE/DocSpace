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
import { getOptions } from "../../common-container/GetThirdPartyStoragesOptions";

class ThirdPartyStorageModule extends React.PureComponent {
  constructor(props) {
    super(props);
    const { thirdPartyStorage, defaultStorageId, setStorageId } = this.props;

    this.state = {
      availableOptions: [],
      availableStorage: {},
    };

    const { comboBoxOptions, storagesInfo, selectedStorageId } = getOptions(
      thirdPartyStorage
    );

    !defaultStorageId && setStorageId(selectedStorageId);

    this.state = {
      comboBoxOptions,
      storagesInfo,
    };
  }

  onSelect = (option) => {
    const selectedStorageId = option.key;
    const { storagesInfo } = this.state;
    const { setStorageId } = this.props;
    const storage = storagesInfo[selectedStorageId];

    setStorageId(storage.id);
  };

  render() {
    const { isLoadingData, selectedStorageId, ...rest } = this.props;
    const { comboBoxOptions, storagesInfo } = this.state;

    const commonProps = {
      selectedStorage: storagesInfo[selectedStorageId],
      selectedId: selectedStorageId,
      isLoadingData,
    };
    const { GoogleId, RackspaceId, SelectelId, AmazonId } = ThirdPartyStorages;

    const storageTitle = storagesInfo[selectedStorageId]?.title;

    return (
      <StyledAutoBackup>
        <div className="auto-backup_storages-module">
          <ComboBox
            options={comboBoxOptions}
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
  const {
    thirdPartyStorage,
    setStorageId,
    selectedStorageId,
    defaultStorageId,
  } = backup;

  return {
    thirdPartyStorage,
    setStorageId,
    selectedStorageId,
    defaultStorageId,
  };
})(withTranslation("Settings")(observer(ThirdPartyStorageModule)));
