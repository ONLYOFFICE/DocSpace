import CatalogAccountsReactSvgUrl from "PUBLIC_DIR/images/catalog.accounts.react.svg?url";
import React from "react";
import { useNavigate } from "react-router-dom";
import CatalogItem from "@docspace/components/catalog-item";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import withLoader from "../../../HOCs/withLoader";

const iconUrl = CatalogAccountsReactSvgUrl;

const PureAccountsItem = ({
  showText,
  setSelectedFolder,
  selectedTreeNode,
  setSelectedNode,
  toggleArticleOpen,

  t,
}) => {
  const navigate = useNavigate();

  const onClick = React.useCallback(() => {
    setSelectedFolder(null);

    setSelectedNode(["accounts", "filter"]);

    navigate("/accounts");
    toggleArticleOpen();
  }, [setSelectedFolder, setSelectedNode, history]);

  const isActive = selectedTreeNode[0] === "accounts";

  return (
    <CatalogItem
      key="accounts"
      text={t("Common:Accounts")}
      icon={iconUrl}
      showText={showText}
      onClick={onClick}
      isActive={isActive}
      folderId="document_catalog-accounts"
    />
  );
};

const AccountsItem = withTranslation(["FilesSettings", "Common"])(
  withLoader(PureAccountsItem)(<></>)
);

export default inject(({ auth, treeFoldersStore, selectedFolderStore }) => {
  const { setSelectedFolder } = selectedFolderStore;
  const { selectedTreeNode, setSelectedNode } = treeFoldersStore;
  const { toggleArticleOpen, showText } = auth.settingsStore;

  return {
    showText,
    setSelectedFolder,
    selectedTreeNode,
    setSelectedNode,
    toggleArticleOpen,
  };
})(observer(AccountsItem));
