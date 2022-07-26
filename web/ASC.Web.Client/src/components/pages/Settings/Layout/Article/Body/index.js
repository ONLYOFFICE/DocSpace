import React from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

import { withTranslation } from "react-i18next";

import { isArrayEqual } from "@appserver/components/utils/array";

import { isMobileOnly } from "react-device-detect";

import { isMobile } from "@appserver/components/utils/device";
import withLoading from "../../../../../../HOCs/withLoading";

import {
  //getKeyByLink,
  settingsTree,
  getSelectedLinkByKey,
  //selectKeyOfTreeElement,
  getCurrentSettingsCategory,
} from "../../../utils";

import CatalogItem from "@appserver/components/catalog-item";
import LoaderArticleBody from "./loaderArticleBody";

class ArticleBodyContent extends React.Component {
  constructor(props) {
    super(props);

    const fullSettingsUrl = props.match.url;
    const locationPathname = props.location.pathname;

    const fullSettingsUrlLength = fullSettingsUrl.length;
    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split("/");

    let link = "";
    const selectedItem = arrayOfParams[arrayOfParams.length - 1];
    if (
      selectedItem === "owner" ||
      selectedItem === "admins" ||
      selectedItem === "modules"
    ) {
      link = `/${resultPath}`;
    } else if (selectedItem === "accessrights") {
      link = `/${resultPath}/owner`;
    }
    const CurrentSettingsCategoryKey = getCurrentSettingsCategory(
      arrayOfParams,
      settingsTree
    );

    if (link === "") {
      link = getSelectedLinkByKey(CurrentSettingsCategoryKey, settingsTree);
    }

    this.state = {
      selectedKeys: [CurrentSettingsCategoryKey],
    };
  }

  componentDidUpdate(prevProps, prevState) {
    const { isLoaded, tReady, setIsLoadedArticleBody } = this.props;

    const isLoadedSetting = isLoaded && tReady;

    if (isLoadedSetting) setIsLoadedArticleBody(isLoadedSetting);

    if (!isArrayEqual(prevState.selectedKeys, this.state.selectedKeys)) {
      const { selectedKeys } = this.state;

      const { match, history } = this.props;
      const settingsPath = getSelectedLinkByKey(selectedKeys[0], settingsTree);
      const newPath = match.path + settingsPath;
      history.push(newPath);
    }
  }

  onSelect = (value) => {
    const { selectedKeys } = this.state;

    const { toggleArticleOpen } = this.props;

    if (isArrayEqual([value], selectedKeys)) {
      return;
    }

    this.setState({ selectedKeys: [value + "-0"] });

    if (isMobileOnly || isMobile()) {
      toggleArticleOpen();
    }
  };

  mapKeys = (tKey) => {
    const { t } = this.props;
    switch (tKey) {
      case "AccessRights":
        return t("Common:AccessRights");
      case "Common:Common":
        return t("Common:Common");
      case "Customization":
        return t("Customization");
      case "StudioTimeLanguageSettings":
        return t("StudioTimeLanguageSettings");
      case "CustomTitlesWelcome":
        return t("CustomTitlesWelcome");
      case "TeamTemplate":
        return t("TeamTemplate");
      case "ManagementCategorySecurity":
        return t("ManagementCategorySecurity");
      case "PortalAccess":
        return t("PortalAccess");
      case "TwoFactorAuth":
        return t("TwoFactorAuth");
      case "ManagementCategoryIntegration":
        return t("ManagementCategoryIntegration");
      case "ThirdPartyAuthorization":
        return t("ThirdPartyAuthorization");
      case "Migration":
        return t("Migration");
      case "Backup":
        return t("Backup");
      case "ManagementCategoryDataManagement":
        return t("ManagementCategoryDataManagement");
      default:
        throw new Error("Unexpected translation key");
    }
  };

  catalogItems = () => {
    const { selectedKeys } = this.state;
    const { showText } = this.props;

    const items = [];

    settingsTree.map((item) => {
      items.push(
        <CatalogItem
          key={item.key}
          id={item.key}
          icon={item.icon}
          showText={showText}
          text={this.mapKeys(item.tKey)}
          value={item.link}
          isActive={item.key + "-0" === selectedKeys[0]}
          onClick={() => this.onSelect(item.key)}
        />
      );
    });

    return items;
  };

  render() {
    const items = this.catalogItems();
    const { isLoadedPage, location, tReady } = this.props;

    const commonSettings =
      location.pathname.includes("common/customization") ||
      location.pathname === "/settings";

    const showLoader = commonSettings ? !isLoadedPage : !tReady;

    return showLoader ? <LoaderArticleBody /> : <>{items}</>;
  }
}

export default inject(({ auth, common }) => {
  const { isLoaded, setIsLoadedArticleBody } = common;

  return {
    showText: auth.settingsStore.showText,
    toggleArticleOpen: auth.settingsStore.toggleArticleOpen,
    isLoaded,
    setIsLoadedArticleBody,
  };
})(
  withLoading(
    withRouter(
      withTranslation(["Settings", "Common"])(observer(ArticleBodyContent))
    )
  )
);
