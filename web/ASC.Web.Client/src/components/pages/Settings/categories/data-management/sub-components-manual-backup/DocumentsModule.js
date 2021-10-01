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
    this._isMounted = false;
  }

  componentDidMount() {
    this._isMounted = true;
  }
  componentWillUnmount() {
    this._isMounted = false;
  }
  onSetLoadingData = (isLoading) => {
    this._isMounted &&
      this.setState({
        isLoadingData: isLoading,
      });
  };

  onSelectFolder = (folderId) => {
    console.log("folderId", folderId);
    this._isMounted &&
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
    const { maxProgress, t, isCopyingLocal } = this.props;
    const { isPanelVisible, isLoadingData, folderPath } = this.state;

    return (
      <div className="category-item-wrapper">
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
          isCommonWithoutProvider
          fontSizeInput={"13px"}
        />

        <div className="manual-backup_buttons">
          <Button
            label={t("MakeCopy")}
            onClick={this.onMakeCopy}
            primary
            isDisabled={!maxProgress || isLoadingData}
            size="medium"
            tabIndex={10}
          />
          {!maxProgress && (
            <Button
              label={t("Copying")}
              onClick={() => console.log("click")}
              isDisabled={true}
              size="medium"
              style={{ marginLeft: "8px" }}
              tabIndex={11}
            />
          )}
        </div>
      </div>
    );
  }
}
export default withTranslation("Settings")(DocumentsModule);
