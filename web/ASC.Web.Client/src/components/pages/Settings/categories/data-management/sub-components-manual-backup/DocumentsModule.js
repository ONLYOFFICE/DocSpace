import React from "react";
import { withTranslation } from "react-i18next";
import SelectFolderInput from "files/SelectFolderInput";
import Button from "@appserver/components/button";
import { getFromSessionStorage } from "../../../utils";

let selectedFolderPathFromSessionStorage = "";
let selectedFolderFromSessionStorage = "";
class DocumentsModule extends React.Component {
  constructor(props) {
    super(props);

    selectedFolderPathFromSessionStorage = getFromSessionStorage(
      "selectedFolderPath"
    );
    selectedFolderFromSessionStorage = getFromSessionStorage("selectedFolder");

    this.state = {
      isLoadingData: false,
      selectedFolder: selectedFolderFromSessionStorage || "",
      isPanelVisible: false,
      folderPath: selectedFolderPathFromSessionStorage || "",
    };
  }

  onSetLoadingData = (isLoading) => {
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

  onMakeCopy = () => {
    const { onMakeCopy } = this.props;
    const { selectedFolder } = this.state;

    onMakeCopy(selectedFolder, "documents", "0", "folderId");
  };

  render() {
    const { isMaxProgress, t, isCopyingLocal } = this.props;
    const { isPanelVisible, isLoadingData, folderPath } = this.state;

    return (
      <>
        <SelectFolderInput
          onSelectFolder={this.onSelectFolder}
          name={"common"}
          onClose={this.onClose}
          onClickInput={this.onClickInput}
          onSetLoadingData={this.onSetLoadingData}
          folderPath={folderPath}
          isPanelVisible={isPanelVisible}
          isSavingProcess={isCopyingLocal}
          foldersType="common"
          withoutProvider
          fontSizeInput={"13px"}
        />

        <div className="manual-backup_buttons">
          <Button
            label={t("MakeCopy")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={!isMaxProgress || isLoadingData}
            size="medium"
          />
          {!isMaxProgress && (
            <Button
              label={t("Copying")}
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
export default withTranslation("Settings")(DocumentsModule);
