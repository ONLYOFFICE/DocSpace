import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import CatalogItem from "@docspace/components/catalog-item";
import { combineUrl } from "@docspace/common/utils";
import history from "@docspace/common/history";

import GiftReactSvgUrl from "PUBLIC_DIR/images/gift.react.svg?url";

const PROXY_BASE_URL = combineUrl(
  window.DocSpaceConfig?.proxy?.url,
  "/portal-settings"
);

const bonusUrl = combineUrl(PROXY_BASE_URL, "/bonus");
const BonusItem = ({ showText, toggleArticleOpen }) => {
  const { t } = useTranslation("Common");

  const onClick = React.useCallback(() => {
    history.push(bonusUrl);
    toggleArticleOpen();
  }, []);

  return (
    <CatalogItem
      key="bonus"
      text={t("Common:Bonus")}
      icon={GiftReactSvgUrl}
      showText={showText}
      onClick={onClick}
      folderId="document_catalog-bonus"
      style={{ marginTop: "16px" }}
    />
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { showText, toggleArticleOpen } = settingsStore;
  return {
    showText,
    toggleArticleOpen,
  };
})(observer(BonusItem));
