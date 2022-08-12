import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { BackupStorageType } from "@docspace/common/constants";
import ScheduleComponent from "./ScheduleComponent";

import DirectThirdPartyConnection from "../../common-container/DirectThirdPartyConnection";

class ThirdPartyModule extends React.PureComponent {
  constructor(props) {
    super(props);
    const { setSelectedFolder, isResourcesDefault, isDocSpace } = props;

    this.state = {
      isPanelVisible: false,
    };

    !isResourcesDefault && setSelectedFolder("");
  }

  onClickInput = () => {
    this.setState({
      isPanelVisible: true,
    });
  };
  onClose = () => {
    this.setState({
      isPanelVisible: false,
    });
  };
  onSelectFolder = (id) => {
    const { setSelectedFolder } = this.props;
    setSelectedFolder(`${id}`);
  };

  onConnect = () => {
    const { isConnected } = this.state;

    this.setState({ isConnected: !isConnected });
  };

  onSelectAccount = (options) => {
    const key = options.key;
    const label = options.label;

    this.setState({
      selectedAccount: { key, label },
    });
  };

  render() {
    const { isPanelVisible } = this.state;
    const {
      isError,
      isLoadingData,
      isReset,
      isSuccessSave,
      passedId,
      //commonThirdPartyList,
      isResourcesDefault,
      isResetProcess,
      isSavingProcess,
      t,
      ...rest
    } = this.props;

    return (
      <>
        {/* {!isDocSpace ? (
          <div className="auto-backup_folder-input">
            <SelectFolderInput
              onSelectFolder={this.onSelectFolder}
              onClose={this.onClose}
              onClickInput={this.onClickInput}
              isPanelVisible={isPanelVisible}
              isError={isError}
              foldersType="third-party"
              isDisabled={commonThirdPartyList.length === 0 || isLoadingData}
              id={passedId}
              isReset={isResetProcess}
              isSuccessSave={isSavingProcess}
              foldersList={commonThirdPartyList}
              withoutBasicSelection={isResourcesDefault ? false : true}
            />
          </div>
        ) : ( */}

        {/* )} */}

        <div className="auto-backup_third-party-module">
          <DirectThirdPartyConnection
            t={t}
            onSelectFolder={this.onSelectFolder}
            onClose={this.onClose}
            onClickInput={this.onClickInput}
            isDisabled={isLoadingData}
            isPanelVisible={isPanelVisible}
            withoutBasicSelection={isResourcesDefault ? false : true}
            isError={isError}
            isReset={isResetProcess}
            isSuccessSave={isSavingProcess}
            id={passedId}
          />
        </div>
        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />
      </>
    );
  }
}
export default inject(({ backup }) => {
  const {
    setSelectedFolder,

    defaultStorageType,
    commonThirdPartyList,
    defaultFolderId,
    isResetProcess,
    isSavingProcess,
  } = backup;

  const isResourcesDefault =
    defaultStorageType === `${BackupStorageType.ResourcesModuleType}`;
  const passedId = isResourcesDefault ? defaultFolderId : "";

  return {
    isResetProcess,
    isSavingProcess,
    setSelectedFolder,
    passedId,
    commonThirdPartyList,
    isResourcesDefault,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyModule)));
