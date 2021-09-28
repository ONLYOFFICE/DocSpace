import React from "react";
import { withTranslation } from "react-i18next";
import SelectFolderInput from "files/SelectFolderInput";
import Button from "@appserver/components/button";
import { inject, observer } from "mobx-react";
import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import { getFromSessionStorage } from "../../../utils";

let selectedFolderPathFromSessionStorage = "";
let selectedFolderFromSessionStorage = "";
class ThirdPartyModule extends React.Component {
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
      isError: false,
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
        isChanged: true,
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

  onMakeCopy = () => {
    const { onMakeCopy } = this.props;
    const { selectedFolder, isError } = this.state;
    if (this.isInvalidForm()) return;
    isError &&
      this.setState({
        isError: false,
      });

    onMakeCopy(selectedFolder, "thirdPartyResource", "1", "folderId");
  };
  render() {
    const {
      maxProgress,
      t,
      helpUrlCreatingBackup,
      isCopyingLocal,
    } = this.props;
    const { isPanelVisible, isLoadingData, isError, folderPath } = this.state;
    return (
      <div className="category-item-wrapper">
        <Box marginProp="16px 0 16px 0">
          <Link
            color="#316DAA"
            target="_blank"
            isHovered={true}
            href={helpUrlCreatingBackup}
          >
            {t("Common:LearnMore")}
          </Link>
        </Box>

        <SelectFolderInput
          onSelectFolder={this.onSelectFolder}
          name={"thirdParty"}
          onClose={this.onClose}
          onClickInput={this.onClickInput}
          onSetLoadingData={this.onSetLoadingData}
          isSavingProcess={isCopyingLocal}
          isPanelVisible={isPanelVisible}
          isError={isError}
          folderPath={folderPath}
          foldersType="third-party"
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
export default inject(({ auth }) => {
  const { helpUrlCreatingBackup } = auth.settingsStore;
  return {
    helpUrlCreatingBackup,
  };
})(withTranslation(["Settings", "Common"])(observer(ThirdPartyModule)));
