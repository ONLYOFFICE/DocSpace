import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import SelectFolderInput from "files/SelectFolderInput";
import Button from "@appserver/components/button";
import { getFromSessionStorage } from "../../../../../utils";
import { BackupStorageType } from "@appserver/common/constants";
import DirectConnectionContainer from "../../common-container/DirectConnectionContainer";

let folder = "";
class ThirdPartyModule extends React.Component {
  constructor(props) {
    super(props);

    const { isDocSpace } = props;

    folder = getFromSessionStorage("LocalCopyFolder");

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
      isStartCopy: false,
      isLoadingData: false,
      selectedFolder: folder || "",
      isPanelVisible: false,
      isError: false,
      ...(isDocSpace && { selectedAccount: this.accounts[0] }),
      isConnected: false,
    };
  }

  onSetLoadingData = (isLoading) => {
    const { isLoadingData } = this.state;
    isLoading !== isLoadingData &&
      this.setState({
        isLoadingData: isLoading,
      });
  };

  onSelectFolder = (folderId) => {
    this.setState({
      selectedFolder: folderId,
    });
  };

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
  isInvalidForm = () => {
    const { selectedFolder } = this.state;

    if (selectedFolder) return false;

    this.setState({
      isError: true,
    });

    return true;
  };

  onMakeCopy = async () => {
    const { onMakeCopy } = this.props;
    const { selectedFolder, isError } = this.state;
    const { ResourcesModuleType } = BackupStorageType;
    if (this.isInvalidForm()) return;

    isError &&
      this.setState({
        isError: false,
      });

    this.setState({
      isStartCopy: true,
    });

    await onMakeCopy(
      selectedFolder,
      "ThirdPartyResource",
      `${ResourcesModuleType}`
    );

    this.setState({
      isStartCopy: false,
    });
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

  onConnect = () => {
    const { isConnected } = this.state;

    this.setState({ isConnected: !isConnected });
  };
  render() {
    const {
      isMaxProgress,
      t,
      commonThirdPartyList,
      buttonSize,
      isDocSpace,
    } = this.props;
    const {
      isPanelVisible,
      isLoadingData,
      isError,
      isStartCopy,
      selectedAccount,
      isConnected,
      selectedFolder,
    } = this.state;

    const isModuleDisabled = !isMaxProgress || isStartCopy || isLoadingData;

    return !isDocSpace ? (
      <>
        <div className="manual-backup_folder-input">
          <SelectFolderInput
            onSelectFolder={this.onSelectFolder}
            name={"thirdParty"}
            onClose={this.onClose}
            onClickInput={this.onClickInput}
            onSetLoadingData={this.onSetLoadingData}
            isDisabled={isModuleDisabled}
            isPanelVisible={isPanelVisible}
            isError={isError}
            foldersType="third-party"
            foldersList={commonThirdPartyList}
            withoutBasicSelection
          />
        </div>
        <div className="manual-backup_buttons">
          <Button
            label={t("Common:Duplicate")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={isModuleDisabled}
            size={buttonSize}
          />
          {!isMaxProgress && (
            <Button
              label={t("Common:CopyOperation") + "..."}
              isDisabled={true}
              size={buttonSize}
              style={{ marginLeft: "8px" }}
            />
          )}
        </div>
      </>
    ) : (
      <div className="manual-backup_third-party-module">
        <DirectConnectionContainer
          accounts={this.accounts}
          selectedAccount={selectedAccount}
          onSelectAccount={this.onSelectAccount}
          onConnect={this.onConnect}
          t={t}
        />

        <SelectFolderInput
          onSelectFolder={this.onSelectFolder}
          name={"thirdParty"}
          onClose={this.onClose}
          onClickInput={this.onClickInput}
          onSetLoadingData={this.onSetLoadingData}
          isDisabled={isModuleDisabled || !isConnected}
          isPanelVisible={isPanelVisible}
          isError={isError}
          foldersType="third-party"
          foldersList={commonThirdPartyList}
          withoutBasicSelection
        />

        <Button
          label={t("Common:Duplicate")}
          onClick={this.onCopyingDirectly}
          primary
          isDisabled={isModuleDisabled || selectedFolder?.trim() === ""}
          size={buttonSize}
        />
      </div>
    );
  }
}
export default inject(({ backup }) => {
  const { commonThirdPartyList } = backup;
  const isDocSpace = false;
  return {
    commonThirdPartyList,
    isDocSpace,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyModule)));
