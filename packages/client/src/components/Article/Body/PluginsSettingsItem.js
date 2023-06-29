import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import CatalogItem from "@docspace/components/catalog-item";

import CatalogAccountsReactSvgUrl from "PUBLIC_DIR/images/catalog.accounts.react.svg?url";

const PluginsSettingsItem = ({ showText, isActive, onClick }) => {
  const { t } = useTranslation([]);

  const onClickAction = React.useCallback(() => {
    onClick && onClick("plugins-settings");
  }, [onClick]);

  return (
    <CatalogItem
      key="plugins-settings"
      text={"Plugins settings"}
      icon={CatalogAccountsReactSvgUrl}
      showText={showText}
      onClick={onClickAction}
      isActive={isActive}
      folderId="document_catalog-plugins-settings"
    />
  );
};

export default inject(({ auth }) => {
  const { showText } = auth.settingsStore;

  return {
    showText,
  };
})(observer(PluginsSettingsItem));
