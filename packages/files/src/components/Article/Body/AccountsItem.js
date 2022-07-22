import React from "react";
import { withRouter } from "react-router";
import CatalogItem from "@docspace/components/catalog-item";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import withLoader from "../../../HOCs/withLoader";

const iconUrl = "/static/images/catalog.accounts.react.svg";

const PureAccountsItem = ({ showText }) => {
  return (
    <CatalogItem
      id="accounts"
      key="accounts"
      text={"Accounts"}
      icon={iconUrl}
      showText={showText}
    />
  );
};

const AccountsItem = withTranslation(["Settings", "Common"])(
  withRouter(withLoader(PureAccountsItem)(<></>))
);

export default inject(({ auth, e }) => {
  return {
    showText: auth.settingsStore.showText,
  };
})(observer(AccountsItem));
