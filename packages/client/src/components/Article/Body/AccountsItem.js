import React from "react";
import { withRouter } from "react-router";
import CatalogItem from "@docspace/components/catalog-item";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import withLoader from "../../../HOCs/withLoader";
import config from "PACKAGE_FILE";

const iconUrl = "/static/images/catalog.accounts.react.svg";

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
      combineUrl(AppServerConfig.proxyURL, config.homepage, "/accounts")
    );
    toggleArticleOpen();
  }, [setSelectedFolder, setSelectedNode, history]);

  const isActive = selectedTreeNode[0] === "accounts";

  return (
    <CatalogItem
      id="accounts"
      key="accounts"
      text={t("Accounts")}
      icon={iconUrl}
      showText={showText}
      onClick={onClick}
      isActive={isActive}
      forderName="folder-accounts"
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
