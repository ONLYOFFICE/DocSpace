import React from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { BackupStorageType } from "@appserver/common/constants";
import SelectFolderInput from "files/SelectFolderInput";
import ScheduleComponent from "./ScheduleComponent";
import DirectConnectionContainer from "../../common-container/DirectConnectionContainer";

class ThirdPartyModule extends React.PureComponent {
  constructor(props) {
    super(props);
    const { setSelectedFolder, isResourcesDefault, isDocSpace } = props;

    if (isDocSpace) {
      this.accounts = [
        {
          key: "0",
          label: "Google Drive",
        },
        {
          key: "1",
          label: "OneDrive ",
        },
        {
          key: "2",
          label: "Dropbox ",
        },
        {
          key: "3",
          label: "Box.com",
        },
      ];
    }

    this.state = {
      isPanelVisible: false,
      ...(isDocSpace && { selectedAccount: this.accounts[0] }),
      isConnected: false,
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

  onCopyingDirectly = () => {
    console.log("copy");
  };
  onSelectAccount = (options) => {
    const key = options.key;
    const label = options.label;

    this.setState({
      selectedAccount: { key, label },
    });
  };

  render() {
    const { isPanelVisible, selectedAccount, isConnected } = this.state;
    const {
      isError,
      isLoadingData,
      isReset,
      isSuccessSave,
      passedId,
      commonThirdPartyList,
      isDocSpace,
      isResourcesDefault,
      isResetProcess,
      isSavingProcess,
      t,
      ...rest
    } = this.props;
    console.log("passedId", passedId);
    return (
      <>
        {!isDocSpace ? (
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
        ) : (
          <div className="auto-backup_third-party-module">
            <DirectConnectionContainer
              accounts={this.accounts}
              selectedAccount={selectedAccount}
              onSelectAccount={this.onSelectAccount}
              onConnect={this.onConnect}
              t={t}
            />

            <SelectFolderInput
              onSelectFolder={this.onSelectFolder}
              onClose={this.onClose}
              onClickInput={this.onClickInput}
              isPanelVisible={isPanelVisible}
              isError={isError}
              foldersType="third-party"
              isDisabled={isLoadingData || !isConnected}
              id={passedId}
              isReset={isResetProcess}
              isSuccessSave={isSavingProcess}
              foldersList={commonThirdPartyList}
              withoutBasicSelection={isResourcesDefault ? false : true}
            />
          </div>
        )}
        <ScheduleComponent isLoadingData={isLoadingData} {...rest} />
      </>
    );
  }
}

export default inject(({ backup }) => {
  const {
    setSelectedFolder,
    //selectedFolderId,
    defaultStorageType,
    commonThirdPartyList,
    defaultFolderId,
    isResetProcess,
    isSavingProcess,
  } = backup;

  const isResourcesDefault =
    defaultStorageType === `${BackupStorageType.ResourcesModuleType}`;

  const passedId = isResourcesDefault ? defaultFolderId : "";
  const isDocSpace = false;
  return {
    isResetProcess,
    isSavingProcess,
    setSelectedFolder,
    passedId,
    commonThirdPartyList,
    isResourcesDefault,
    isDocSpace,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyModule)));
