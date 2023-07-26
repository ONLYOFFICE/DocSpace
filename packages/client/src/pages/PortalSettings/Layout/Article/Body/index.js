import React from "react";
import { useNavigate, useLocation, useParams } from "react-router-dom";
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
        return t("Customization");
      case "SettingsGeneral":
        return t("SettingsGeneral");
      case "StudioTimeLanguageSettings":
        return t("StudioTimeLanguageSettings");
      case "CustomTitles":
        return t("CustomTitles");
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
      case "PortalDeletion":
        return t("PortalDeletion");
      case "Common:PaymentsTitle":
        return t("Common:PaymentsTitle");
      case "SingleSignOn":
        return t("SingleSignOn");
      case "SMTPSettings":
        return t("SMTPSettings");
      case "DeveloperTools":
        return t("DeveloperTools");
      case "Bonus":
        return t("Common:Bonus");
      case "FreeProFeatures":
        return "Common:FreeProFeatures";
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

const ArticleBodyContent = (props) => {
  const {
    t,
    tReady,
    setIsLoadedArticleBody,
    toggleArticleOpen,
    showText,
    isNotPaidPeriod,
    isOwner,
    isLoadedArticleBody,
    standalone,
    isEnterprise,
    isCommunity,
  } = props;

  const [selectedKeys, setSelectedKeys] = React.useState([]);

  const prevLocation = React.useRef(null);

  const navigate = useNavigate();
  const location = useLocation();

  React.useEffect(() => {
    prevLocation.current = location;
  }, [location]);

  React.useEffect(() => {
    const locationPathname = location.pathname;

    const resultPath = locationPathname;
    const arrayOfParams = resultPath.split("/");
    arrayOfParams.splice(0, 2);

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

    setSelectedKeys([CurrentSettingsCategoryKey]);
  }, []);

  React.useEffect(() => {
    if (tReady) setIsLoadedArticleBody(true);

    if (prevLocation.current.pathname !== location.pathname) {
      if (location.pathname.includes("common")) {
        setSelectedKeys(["0-0"]);
      }

      if (location.pathname.includes("security")) {
        setSelectedKeys(["1-0"]);
      }

      if (location.pathname.includes("backup")) {
        setSelectedKeys(["2-0"]);
      }

      if (location.pathname.includes("restore")) {
        setSelectedKeys(["3-0"]);
      }

      if (location.pathname.includes("integration")) {
        setSelectedKeys(["4-0"]);
      }

      if (location.pathname.includes("developer")) {
        setSelectedKeys(["5-0"]);
      }

      if (location.pathname.includes("delete-data")) {
        setSelectedKeys(["6-0"]);
      }

      if (location.pathname.includes("payments")) {
        setSelectedKeys(["7-0"]);
      }
      if (this.props.location.pathname.includes("bonus")) {
        this.setState({ selectedKeys: ["8-0"] });
      }
    }
  }, [tReady, setIsLoadedArticleBody, location.pathname, selectedKeys]);

  const onSelect = (value) => {
    if (isArrayEqual([value], selectedKeys)) {
      return;
    }

    setSelectedKeys([value + "-0"]);

    if (isMobileOnly || isMobile()) {
      toggleArticleOpen();
    }

    const settingsPath = `/portal-settings${getSelectedLinkByKey(
      value + "-0",
      settingsTree
    )}`;

    if (settingsPath === location.pathname) return;

    navigate(`${settingsPath}`);
  };

  const mapKeys = (tKey) => {
    switch (tKey) {
      case "AccessRights":
        return t("Common:AccessRights");
      case "Customization":
        return t("Customization");
      case "SettingsGeneral":
        return t("SettingsGeneral");
      case "StudioTimeLanguageSettings":
        return t("StudioTimeLanguageSettings");
      case "CustomTitlesWelcome":
        return t("CustomTitlesWelcome");
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
      case "Common:PaymentsTitle":
        return t("Common:PaymentsTitle");
      case "ManagementCategoryDataManagement":
        return t("ManagementCategoryDataManagement");
      case "RestoreBackup":
        return t("RestoreBackup");
      case "PortalDeletion":
        return t("PortalDeletion");
      case "DeveloperTools":
        return t("DeveloperTools");
      case "Common:Bonus":
        return t("Common:Bonus");
      case "Common:FreeProFeatures":
        return "Common:FreeProFeatures";
      default:
        throw new Error("Unexpected translation key");
    }
  };

  const catalogItems = () => {
    const items = [];

    let resultTree = [...settingsTree];

    if (isNotPaidPeriod) {
      resultTree = [...settingsTree].filter((e) => {
        return (
          e.tKey === "Backup" ||
          e.tKey === "Common:PaymentsTitle" ||
          (isOwner && e.tKey === "PortalDeletion")
        );
      });
    }

    if (standalone) {
      const deletionTKey = isCommunity
        ? "Common:PaymentsTitle"
        : "Common:Bonus";

      const index = resultTree.findIndex((el) => el.tKey === deletionTKey);

      if (index !== -1) {
        resultTree.splice(index, 1);
      }
    } else {
      const index = resultTree.findIndex((n) => n.tKey === "Common:Bonus");
      if (index !== -1) {
        resultTree.splice(index, 1);
      }
    }

    if (!isOwner || standalone) {
      const index = resultTree.findIndex((n) => n.tKey === "PortalDeletion");
      if (index !== -1) {
        resultTree.splice(index, 1);
      }
    }

    if (selectedKeys.length === 0) return <></>;

    resultTree.map((item) => {
      items.push(
        <CatalogItem
          key={item.key}
          id={item.key}
          icon={item.icon}
          showText={showText}
          text={mapKeys(item.tKey)}
          value={item.link}
          isActive={item.key === selectedKeys[0][0]}
          onClick={() => onSelect(item.key)}
          folderId={item.id}
          style={{
            marginTop: `${
              item.key.includes(7) || item.key.includes(8) ? "16px" : "0"
            }`,
          }}
        />
      );
    });

    return items;
  };

  const items = catalogItems();

  return !isLoadedArticleBody ? <LoaderArticleBody /> : <>{items}</>;
};

export default inject(({ auth, common }) => {
  const { isLoadedArticleBody, setIsLoadedArticleBody } = common;
  const {
    currentTariffStatusStore,
    userStore,
    isEnterprise,
    settingsStore,
    isCommunity,
  } = auth;
  const { isNotPaidPeriod } = currentTariffStatusStore;
  const { user } = userStore;
  const { isOwner } = user;
  const { standalone, showText, toggleArticleOpen } = settingsStore;

  return {
    standalone,
    isEnterprise,
    showText,
    toggleArticleOpen,
    isLoadedArticleBody,
    setIsLoadedArticleBody,
    isNotPaidPeriod,
    isOwner,
    isCommunity,
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(ArticleBodyContent))
  )
);
