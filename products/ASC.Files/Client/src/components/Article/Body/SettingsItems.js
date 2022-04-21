import React from "react";
import { withRouter } from "react-router";
import CatalogItem from "@appserver/components/catalog-item";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { combineUrl } from "@appserver/common/utils";
import config from "../../../../package.json";
import { AppServerConfig } from "@appserver/common/constants";
import withLoader from "../../../HOCs/withLoader";
import { isMobile } from "@appserver/components/utils/device";
import { isMobileOnly } from "react-device-detect";
const iconUrl = "/static/images/settings.react.svg";
const PureSettingsItems = ({
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

  const isActive = () => {
    return window.location.pathname.indexOf("/settings") > 0;
  };

  return (
    <CatalogItem
      id="settings"
      key="settings"
      text={t("Common:Settings")}
      icon={iconUrl}
      showText={showText}
      onClick={onClick}
      isActive={isActive()}
    />
  );
};

const SettingsItems = withTranslation(["Settings", "Common"])(
  withRouter(withLoader(PureSettingsItems)(<></>))
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
)(observer(SettingsItems));
