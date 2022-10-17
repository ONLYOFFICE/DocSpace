import React from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

import { withTranslation } from "react-i18next";

import { isArrayEqual } from "@docspace/components/utils/array";

import { isMobileOnly } from "react-device-detect";

import { isMobile } from "@docspace/components/utils/device";
import withLoading from "SRC_DIR/HOCs/withLoading";

import {
  //getKeyByLink,
  settingsTree,
  getSelectedLinkByKey,
  //selectKeyOfTreeElement,
  getCurrentSettingsCategory,
} from "../../../utils";

import CatalogItem from "@docspace/components/catalog-item";
import LoaderArticleBody from "./loaderArticleBody";

const getTreeItems = (data, path, t) => {
  const maptKeys = (tKey) => {
    switch (tKey) {
      case "AccessRights":
        return t("AccessRights");
      case "ManagementCategoryCommon":
        return t("ManagementCategoryCommon");
      case "Customization":
        return t("Customization");
      case "StudioTimeLanguageSettings":
        return t("StudioTimeLanguageSettings");
      case "CustomTitles":
        return t("CustomTitles");
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
      case "Payments":
        return t("Payments");
      case "SingleSignOn":
        return t("SingleSignOn");
      default:
        throw new Error("Unexpected translation key");
    }
  };
  return data.map((item) => {
    if (item.children && item.children.length && !item.isCategory) {
      return (
        <TreeNode
          title={
            <Text className="inherit-title-link header">
              {maptKeys(item.tKey)}
            </Text>
          }
          key={item.key}
          icon={item.icon && <ReactSVG className="tree_icon" src={item.icon} />}
          disableSwitch={true}
        >
          {getTreeItems(item.children, path, t)}
        </TreeNode>
      );
    }
    const link = path + getSelectedLinkByKey(item.key, settingsTree);
    return (
      <TreeNode
        key={item.key}
        title={
          <Link className="inherit-title-link" href={link}>
            {maptKeys(item.tKey)}
          </Link>
        }
        icon={item.icon && <ReactSVG src={item.icon} className="tree_icon" />}
        disableSwitch={true}
      />
    );
  });
};

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
    const { tReady, setIsLoadedArticleBody } = this.props;

    if (tReady) setIsLoadedArticleBody(true);

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
      case "Payments":
        return t("Payments");
      case "ManagementCategoryDataManagement":
        return t("ManagementCategoryDataManagement");
      case "RestoreBackup":
        return t("RestoreBackup");
      default:
        throw new Error("Unexpected translation key");
    }
  };

  catalogItems = () => {
    const { selectedKeys } = this.state;
    const { showText, isNotPaidPeriod, t } = this.props;

    const items = [];
    let resultTree = [...settingsTree];

    if (isNotPaidPeriod) {
      resultTree = [...settingsTree].filter((e) => {
        return e.tKey === "Backup" || e.tKey === "Payments";
      });
    }
    resultTree.map((item) => {
      items.push(
        <CatalogItem
          key={item.key}
          id={item.key}
          icon={item.icon}
          showText={showText}
          text={this.mapKeys(item.tKey)}
          value={item.link}
          isActive={item.key === selectedKeys[0][0]}
          onClick={() => this.onSelect(item.key)}
        />
      );
    });

    const settingsHeader = (
      <CatalogItem
        key={"settings-header"}
        isHeader={true}
        isFirstHeader={true}
        showText={showText}
        text={`${t("Common:Settings")}`}
      />
    );

    items.unshift(settingsHeader);

    return items;
  };

  render() {
    const items = this.catalogItems();
    const { isLoadedArticleBody } = this.props;

    return !isLoadedArticleBody ? <LoaderArticleBody /> : <>{items}</>;
  }
}

export default inject(({ auth, common }) => {
  const { isLoadedArticleBody, setIsLoadedArticleBody } = common;
  const { currentTariffStatusStore } = auth;
  const { isNotPaidPeriod } = currentTariffStatusStore;
  return {
    showText: auth.settingsStore.showText,
    toggleArticleOpen: auth.settingsStore.toggleArticleOpen,
    isLoadedArticleBody,
    setIsLoadedArticleBody,
    isNotPaidPeriod,
  };
})(
  withLoading(
    withRouter(
      withTranslation(["Settings", "Common"])(observer(ArticleBodyContent))
    )
  )
);
