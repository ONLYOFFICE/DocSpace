import CatalogSettingsReactSvgUrl from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import React from "react";
import { useNavigate, useParams } from "react-router-dom";
import CatalogItem from "@docspace/components/catalog-item";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import withLoader from "../../../HOCs/withLoader";

const iconUrl = CatalogSettingsReactSvgUrl;

const PureSettingsItem = ({ t, showText, isActive, onClick }) => {
  // const { setting } = useParams();

  // React.useEffect(() => {
  //   setSelectedNode([setting]);
  // }, [setting, setSelectedNode]);

  // React.useEffect(() => {
  //   if (setting && !expandedSetting) setExpandSettingsTree(["settings"]);
  // }, [expandedSetting, setExpandSettingsTree]);

  const onClickAction = React.useCallback(() => {
    onClick && onClick("settings");
  }, [onClick]);

  return (
    <CatalogItem
      key="settings"
      text={t("Common:Settings")}
      icon={iconUrl}
      showText={showText}
      onClick={onClickAction}
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
