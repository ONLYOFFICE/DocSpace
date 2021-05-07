import { useState } from "react";
import React from "react";
import { useTranslation } from "react-i18next";
import OperationsDialog from "files/OperationsDialog";
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
    startBackup("0", "folderId", selectedFolder[0]);
    setInterval();
  };
  //console.log("thirdparty!!! commonThirdPartyList", commonThirdPartyList);
  return (
    <div className="category-item-wrapper">
      <div className="category-item-heading">
        <Text className="inherit-title-link header">
          {t("DocumentsModule")}
        </Text>
      </div>

      <Text className="category-item-description">
        {t("DocumentsModuleDescription")}
      </Text>

      <OperationsDialog
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
      </div>
    </div>
  );
};

export default DocumentsModule;
