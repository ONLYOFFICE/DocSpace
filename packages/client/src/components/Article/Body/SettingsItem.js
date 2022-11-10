import React from "react";
import { withRouter } from "react-router";
import CatalogItem from "@docspace/components/catalog-item";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import { AppServerConfig } from "@docspace/common/constants";
import withLoader from "../../../HOCs/withLoader";
import { isMobile } from "@docspace/components/utils/device";
import { isMobileOnly } from "react-device-detect";

const iconUrl = "/static/images/catalog.settings.react.svg";

const PureSettingsItem = ({
  match,
  expandedSetting,
  setSelectedNode,
  setExpandSettingsTree,
  setSelectedFolder,
  history,
  setIsLoading,
  t,
  showText,
  toggleArticleOpen,
}) => {
  const { setting } = match.params;

  React.useEffect(() => {
    setIsLoading(true);
    setSelectedNode([setting]);
    setIsLoading(false);
  }, [setting, setIsLoading, setSelectedNode]);

  React.useEffect(() => {
    if (setting && !expandedSetting) setExpandSettingsTree(["settings"]);
  }, [expandedSetting, setExpandSettingsTree]);

  const onClick = React.useCallback(() => {
    setSelectedFolder(null);

    setSelectedNode(["common"]);
    setExpandSettingsTree(["common"]);
    if (isMobile() || isMobileOnly) toggleArticleOpen();
    history.push(
      combineUrl(AppServerConfig.proxyURL, config.homepage, "/settings/common")
    );
  }, [
    setSelectedFolder,
    setSelectedNode,
    setExpandSettingsTree,
    toggleArticleOpen,
    history,
  ]);

  const isActive = window.location.pathname.includes("settings");

  return (
    <CatalogItem
      id="settings"
      key="settings"
      text={t("Common:Settings")}
      icon={iconUrl}
      showText={showText}
      onClick={onClick}
      isActive={isActive}
      forderName="catalog-settings"
    />
  );
};

const SettingsItem = withTranslation(["FilesSettings", "Common"])(
  withRouter(withLoader(PureSettingsItem)(<></>))
);

export default inject(
  ({
    auth,
    filesStore,
    settingsStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const { setIsLoading } = filesStore;
    const { setSelectedFolder } = selectedFolderStore;
    const { setSelectedNode } = treeFoldersStore;
    const { expandedSetting, setExpandSettingsTree } = settingsStore;
    return {
      expandedSetting,
      setIsLoading,
      setSelectedFolder,
      setSelectedNode,
      setExpandSettingsTree,
      showText: auth.settingsStore.showText,
      toggleArticleOpen: auth.settingsStore.toggleArticleOpen,
    };
  }
)(observer(SettingsItem));
