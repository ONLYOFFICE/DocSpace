import { useState } from "react";
import React from "react";
import { useTranslation } from "react-i18next";
import SelectedFolder from "files/SelectedFolder";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { useEffect } from "react";
import { startBackup } from "@appserver/common/api/portal";
const DocumentsModule = ({ maxProgress, setInterval }) => {
  const [selectedFolder, setSelectedFolder] = useState("");
  const [isPanelVisible, setPanelVisible] = useState(false);
  const { t } = useTranslation("Settings");

  const onSelectFolder = (folderId) => {
    setSelectedFolder(folderId);
  };

  const onClickInput = (e) => {
    setPanelVisible(true);
  };

  const onClose = () => {
    setPanelVisible(false);
  };

  const onClickButton = () => {
    console.log("selectedFolder", selectedFolder);
    const storageParams = [
      {
        key: "folderId",
        value: selectedFolder[0],
      },
    ];
    startBackup("0", storageParams);
    setInterval();
  };

  return (
    <div className="category-item-wrapper">
      <SelectedFolder
        onSelectFolder={onSelectFolder}
        name={"common"}
        onClose={onClose}
        onClickInput={onClickInput}
        isPanelVisible={isPanelVisible}
        isCommonWithoutProvider
      />

      <div className="manual-backup_buttons">
        <Button
          label={t("MakeCopy")}
          onClick={onClickButton}
          primary
          isDisabled={!maxProgress}
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
};

export default DocumentsModule;
