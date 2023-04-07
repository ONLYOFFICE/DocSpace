import CatalogSettingsReactSvgUrl from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import React from "react";
import { useNavigate, useParams } from "react-router-dom";
import CatalogItem from "@docspace/components/catalog-item";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import withLoader from "../../../HOCs/withLoader";
import { isMobile } from "@docspace/components/utils/device";
import { isMobileOnly } from "react-device-detect";

const iconUrl = CatalogSettingsReactSvgUrl;

const PureSettingsItem = ({
  expandedSetting,
  setSelectedNode,
  setExpandSettingsTree,
  setSelectedFolder,

  t,
  showText,
  toggleArticleOpen,
}) => {
  const { setting } = useParams();
  const navigate = useNavigate();

  React.useEffect(() => {
    setSelectedNode([setting]);
  }, [setting, setSelectedNode]);

  React.useEffect(() => {
    if (setting && !expandedSetting) setExpandSettingsTree(["settings"]);
  }, [expandedSetting, setExpandSettingsTree]);

  const onClick = React.useCallback(() => {
    setSelectedFolder(null);

    setSelectedNode(["common"]);
    setExpandSettingsTree(["common"]);
    if (isMobile() || isMobileOnly) toggleArticleOpen();
    navigate("/settings/common");
  }, [
    setSelectedFolder,
    setSelectedNode,
    setExpandSettingsTree,
    toggleArticleOpen,
  ]);

  const isActive = window.location.pathname.includes("settings");

  return (
    <CatalogItem
      key="settings"
      text={t("Common:Settings")}
      icon={iconUrl}
      showText={showText}
      onClick={onClick}
      isActive={isActive}
      folderId="document_catalog-settings"
    />
  );
};

const SettingsItem = withTranslation(["FilesSettings", "Common"])(
  withLoader(PureSettingsItem)(<></>)
);

export default inject(
  ({ auth, settingsStore, treeFoldersStore, selectedFolderStore }) => {
    const { setSelectedFolder } = selectedFolderStore;
    const { setSelectedNode } = treeFoldersStore;
    const { expandedSetting, setExpandSettingsTree } = settingsStore;
    return {
      expandedSetting,

      setSelectedFolder,
      setSelectedNode,
      setExpandSettingsTree,
      showText: auth.settingsStore.showText,
      toggleArticleOpen: auth.settingsStore.toggleArticleOpen,
    };
  }
)(observer(SettingsItem));
