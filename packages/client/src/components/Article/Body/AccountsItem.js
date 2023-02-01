﻿import CatalogAccountsReactSvgUrl from "PUBLIC_DIR/images/catalog.accounts.react.svg?url";
import React from "react";
import { withRouter } from "react-router";
import CatalogItem from "@docspace/components/catalog-item";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { combineUrl } from "@docspace/common/utils";
import withLoader from "../../../HOCs/withLoader";
import config from "PACKAGE_FILE";

const iconUrl = CatalogAccountsReactSvgUrl;

const PureAccountsItem = ({
  showText,
  setSelectedFolder,
  selectedTreeNode,
  setSelectedNode,
  toggleArticleOpen,
  history,
  t,
}) => {
  const onClick = React.useCallback(() => {
    setSelectedFolder(null);

    setSelectedNode(["accounts", "filter"]);

    history.push(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        "/accounts"
      )
    );
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
  withRouter(withLoader(PureAccountsItem)(<></>))
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
