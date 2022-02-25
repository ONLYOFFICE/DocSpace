import React from "react";
import { withTranslation } from "react-i18next";
import SelectFolderInput from "files/SelectFolderInput";
import Button from "@appserver/components/button";

import { getFromSessionStorage } from "../../../../../utils";

let folderPath = "";
let folder = "";
class ThirdPartyModule extends React.Component {
  constructor(props) {
    super(props);

    folderPath = getFromSessionStorage("LocalCopyPath");
    folder = getFromSessionStorage("LocalCopyFolder");

    this.state = {
      isStartCopy: false,
      isLoadingData: true,
      selectedFolder: folder || "",
      isPanelVisible: false,
      isError: false,
      folderPath: folderPath || "",
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

    if (this.isInvalidForm()) return;

    isError &&
      this.setState({
        isError: false,
      });

    this.setState({
      isStartCopy: true,
    });

    await onMakeCopy(selectedFolder, "ThirdPartyResource", "1", "folderId");

    this.setState({
      isStartCopy: false,
    });
  };
  render() {
    const { isMaxProgress, t, commonThirdPartyList } = this.props;
    const {
      isPanelVisible,
      isLoadingData,
      isError,
      folderPath,
      isStartCopy,
    } = this.state;

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
            isSavingProcess={isModuleDisabled}
            isPanelVisible={isPanelVisible}
            isError={isError}
            folderPath={folderPath}
            foldersType="third-party"
            fontSizeInput={"13px"}
            foldersList={commonThirdPartyList}
          />
        </div>
        <div className="manual-backup_buttons">
          <Button
            label={t("MakeCopy")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={isModuleDisabled}
            size="medium"
          />
          {!isMaxProgress && (
            <Button
              label={t("Copying")}
              onClick={() => console.log("click")}
              isDisabled={true}
              size="medium"
              style={{ marginLeft: "8px" }}
            />
          )}
        </div>
      </>
    );
  }
}
export default withTranslation("Settings")(ThirdPartyModule);
