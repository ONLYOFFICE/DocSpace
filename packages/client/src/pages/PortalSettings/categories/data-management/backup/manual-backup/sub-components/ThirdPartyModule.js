import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import SelectFolderInput from "client/SelectFolderInput";
import Button from "@docspace/components/button";
import { getFromSessionStorage } from "../../../../../utils";
import { BackupStorageType } from "@docspace/common/constants";

let folder = "";
class ThirdPartyModule extends React.Component {
  constructor(props) {
    super(props);

    folder = getFromSessionStorage("LocalCopyFolder");

    this.state = {
      isStartCopy: false,
      isLoadingData: false,
      selectedFolder: folder || "",
      isPanelVisible: false,
      isError: false,
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
      `${ResourcesModuleType}`,
      "folderId"
    );

    this.setState({
      isStartCopy: false,
    });
  };
  render() {
    const { isMaxProgress, t, commonThirdPartyList, buttonSize } = this.props;
    const { isPanelVisible, isLoadingData, isError, isStartCopy } = this.state;

    const isModuleDisabled = !isMaxProgress || isStartCopy || isLoadingData;
    return (
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
            ignoreSelectedFolderTree
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
    );
  }
}
export default inject(({ backup }) => {
  const { commonThirdPartyList } = backup;

  return {
    commonThirdPartyList,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyModule)));
