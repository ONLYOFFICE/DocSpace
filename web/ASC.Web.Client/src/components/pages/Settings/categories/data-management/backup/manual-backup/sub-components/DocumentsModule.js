import React from "react";
import { withTranslation } from "react-i18next";
import SelectFolderInput from "files/SelectFolderInput";
import Button from "@appserver/components/button";
import { getFromSessionStorage } from "../../../../../utils";
import { BackupStorageType } from "@appserver/common/constants";

let folder = "";
class DocumentsModule extends React.Component {
  constructor(props) {
    super(props);

    folder = getFromSessionStorage("LocalCopyFolder");

    this.state = {
      isStartCopy: false,
      selectedFolder: folder || "",
      isPanelVisible: false,
    };
  }

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

  onMakeCopy = async () => {
    const { onMakeCopy } = this.props;
    const { selectedFolder } = this.state;
    const { DocumentModuleType } = BackupStorageType;

    this.setState({
      isStartCopy: true,
    });

    await onMakeCopy(selectedFolder, "Documents", `${DocumentModuleType}`);

    this.setState({
      isStartCopy: false,
    });
  };

  render() {
    const { isMaxProgress, t, buttonSize } = this.props;
    const { isPanelVisible, isStartCopy, selectedFolder } = this.state;

    const isModuleDisabled = !isMaxProgress || isStartCopy;
    return (
      <>
        <div className="manual-backup_folder-input">
          <SelectFolderInput
            onSelectFolder={this.onSelectFolder}
            onClose={this.onClose}
            onClickInput={this.onClickInput}
            isPanelVisible={isPanelVisible}
            isDisabled={isModuleDisabled}
            foldersType="rooms"
            withoutBasicSelection
          />
        </div>
        <div className="manual-backup_buttons">
          <Button
            label={t("Common:Duplicate")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={isModuleDisabled || !selectedFolder}
            size={buttonSize}
          />
          {!isMaxProgress && (
            <Button
              label={t("Common:CopyOperation") + "..."}
              isDisabled={true}
              size={buttonSize}
            />
          )}
        </div>
      </>
    );
  }
}
export default withTranslation(["Settings", "Common"])(DocumentsModule);
