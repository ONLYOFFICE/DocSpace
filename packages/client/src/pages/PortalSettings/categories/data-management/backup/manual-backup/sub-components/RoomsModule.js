import React from "react";
import { withTranslation } from "react-i18next";
import SelectFolderInput from "client/SelectFolderInput";
import Button from "@docspace/components/button";
import { getFromLocalStorage } from "../../../../../utils";
import { BackupStorageType } from "@docspace/common/constants";

let folder = "";
const Documents = "Documents";

class RoomsModule extends React.Component {
  constructor(props) {
    super(props);

    folder = getFromLocalStorage("LocalCopyFolder");
    const moduleType = getFromLocalStorage("LocalCopyStorageType");

    const selectedFolder = moduleType === Documents ? folder : "";

    this.state = {
      isStartCopy: false,
      selectedFolder: selectedFolder,
      isPanelVisible: false,
    };

    this._isMount = false;
  }

  componentDidMount() {
    this._isMount = true;
  }
  componentWillUnmount() {
    this._isMount = false;
  }
  onSelectFolder = (folderId) => {
    this._isMount &&
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

    await onMakeCopy(selectedFolder, Documents, `${DocumentModuleType}`);

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
            filteredType="exceptSortedByTags"
            {...(selectedFolder && { id: selectedFolder })}
            withoutBasicSelection={selectedFolder ? false : true}
          />
        </div>
        <div className="manual-backup_buttons">
          <Button
            id="create-copy"
            label={t("Common:CreateCopy")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={isModuleDisabled || !selectedFolder}
            size={buttonSize}
          />
        </div>
      </>
    );
  }
}
export default withTranslation(["Settings", "Common"])(RoomsModule);
