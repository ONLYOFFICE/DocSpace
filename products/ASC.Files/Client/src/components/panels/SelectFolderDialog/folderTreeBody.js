import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import TreeFolders from "../../Article/Body/TreeFolders";
const FolderTreeBody = ({
  isLoadingData,
  expandedKeys,
  folderList,
  onSelect,
  isCommonWithoutProvider,
  certainFolders,
  isAvailableFolders,
  filter,
}) => {
  const { t } = useTranslation(["SelectFile", "Common"]);
  return (
    <>
      {!isLoadingData ? (
        isAvailableFolders ? (
          <TreeFolders
            expandedPanelKeys={expandedKeys}
            data={folderList}
            filter={filter}
            onSelect={onSelect}
            withoutProvider={isCommonWithoutProvider}
            certainFolders={certainFolders}
          />
        ) : (
          <Text as="span">{t("NotAvailableFolder")}</Text>
        )
      ) : (
        <div key="loader" className="panel-loader-wrapper">
          <Loader type="oval" size="16px" className="panel-loader" />
          <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
            "Common:LoadingDescription"
          )}`}</Text>
        </div>
      )}
    </>
  );
};

FolderTreeBody.defaultProps = {
  isAvailableFolders: true,
  isLoadingData: false,
};

export default inject(
  ({ filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { filter } = filesStore;
    const { expandedPanelKeys } = treeFoldersStore;
    return {
      expandedKeys: expandedPanelKeys
        ? expandedPanelKeys
        : selectedFolderStore.pathParts,
      filter,
    };
  }
)(observer(FolderTreeBody));
