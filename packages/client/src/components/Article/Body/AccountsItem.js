import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import CatalogItem from "@docspace/components/catalog-item";

import CatalogAccountsReactSvgUrl from "PUBLIC_DIR/images/catalog.accounts.react.svg?url";

import withLoader from "../../../HOCs/withLoader";

const PureAccountsItem = ({ showText, isActive, onClick, t }) => {
  const onClickAction = React.useCallback(() => {
    onClick && onClick("accounts");
  }, [onClick]);

  return (
    <CatalogItem
      key="accounts"
      text={t("Accounts")}
      icon={CatalogAccountsReactSvgUrl}
      showText={showText}
      onClick={onClickAction}
      isActive={isActive}
      folderId="document_catalog-accounts"
    />
  );
};

const AccountsItem = withTranslation(["Common"])(PureAccountsItem);

export default inject(({ auth }) => {
  const { showText } = auth.settingsStore;

  return {
    showText,
  };
})(observer(AccountsItem));
