import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import Button from "@docspace/components/button";
import { getFromLocalStorage } from "../../../../../utils";
import { BackupStorageType } from "@docspace/common/constants";
import DirectThirdPartyConnection from "../../common-container/DirectThirdPartyConnection";

let folder = "";
const ThirdPartyResource = "ThirdPartyResource";

class ThirdPartyModule extends React.Component {
  constructor(props) {
    super(props);

    folder = getFromLocalStorage("LocalCopyFolder");
    const moduleType = getFromLocalStorage("LocalCopyStorageType");

    const selectedFolder = moduleType === ThirdPartyResource ? folder : "";

    this.state = {
      isStartCopy: false,
      isLoadingData: false,
      selectedFolder: selectedFolder,
      isPanelVisible: false,
      isError: false,
      isLoading: false,
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
      ThirdPartyResource,
      `${ResourcesModuleType}`
    );

    this.setState({
      isStartCopy: false,
    });
  };

  onSelectAccount = (options) => {
    const key = options.key;

    this.setState({
      selectedAccount: { ...this.accounts[+key] },
    });
  };

  render() {
    const { isMaxProgress, t, buttonSize } = this.props;
    const {
      isPanelVisible,
      isLoadingData,
      isError,
      isStartCopy,
      selectedFolder,
    } = this.state;

    const isModuleDisabled = !isMaxProgress || isStartCopy || isLoadingData;

    return (
      // !isDocSpace ? (
      //   <>
      //     <div className="manual-backup_folder-input">
      //       <SelectFolderInput
      //         onSelectFolder={this.onSelectFolder}
      //         name={"thirdParty"}
      //         onClose={this.onClose}
      //         onClickInput={this.onClickInput}
      //         onSetLoadingData={this.onSetLoadingData}
      //         isDisabled={isModuleDisabled}
      //         isPanelVisible={isPanelVisible}
      //         isError={isError}
      //         foldersType="third-party"
      //         foldersList={commonThirdPartyList}
      //         withoutBasicSelection
      //       />
      //     </div>
      //     <div className="manual-backup_buttons">
      //       <Button
      //         label={t("Common:Duplicate")}
      //         onClick={this.onMakeCopy}
      //         primary
      //         isDisabled={isModuleDisabled || selectedFolder?.trim() === ""}
      //         size={buttonSize}
      //       />
      //       {!isMaxProgress && (
      //         <Button
      //           label={t("Common:CopyOperation") + "..."}
      //           isDisabled={true}
      //           size={buttonSize}
      //           style={{ marginLeft: "8px" }}
      //         />
      //       )}
      //     </div>
      //   </>
      // ) : (
      <div className="manual-backup_third-party-module">
        <DirectThirdPartyConnection
          t={t}
          onSelectFolder={this.onSelectFolder}
          onClose={this.onClose}
          onClickInput={this.onClickInput}
          onSetLoadingData={this.onSetLoadingData}
          isDisabled={isModuleDisabled}
          isPanelVisible={isPanelVisible}
          {...(selectedFolder && { id: selectedFolder })}
          withoutBasicSelection={selectedFolder ? false : true}
          isError={isError}
        />

        <Button
          label={t("Common:Duplicate")}
          onClick={this.onMakeCopy}
          primary
          isDisabled={isModuleDisabled || selectedFolder?.trim() === ""}
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
    );
  }
}
export default inject(({ backup }) => {
  const { commonThirdPartyList } = backup;

  return {
    commonThirdPartyList,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyModule)));
