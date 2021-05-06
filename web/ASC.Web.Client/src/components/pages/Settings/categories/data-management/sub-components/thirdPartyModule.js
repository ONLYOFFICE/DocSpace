import { useState } from "react";
import React from "react";
import { useTranslation } from "react-i18next";
import OperationsDialog from "files/OperationsDialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";

const DocumentsModule = ({
  maxProgress,
  commonThirdPartyList,
  setInterval,
}) => {
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

  return (
    <div className="category-item-wrapper">
      <div className="category-item-heading">
        <Text className="inherit-title-link header">
          {t("ThirdPartyResource")}
        </Text>
      </div>

      <Text className="category-item-description">
        {t("ThirdPartyResourceDescription")}
      </Text>
      <Text className="category-item-description note_description">
        {t("ThirdPartyResourceNoteDescription")}
      </Text>

      <OperationsDialog
        onSelectFolder={onSelectFolder}
        name={"common"}
        onClose={onClose}
        onClickInput={onClickInput}
        isPanelVisible={isPanelVisible}
        folderList={commonThirdPartyList}
      />

      <div className="manual-backup_buttons">
        <Button
          label={t("MakeCopy")}
          onClick={() => console.log("click")}
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
