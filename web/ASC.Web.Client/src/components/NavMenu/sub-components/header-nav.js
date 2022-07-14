import React, { useCallback, useState } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import NavItem from "./nav-item";
import ProfileActions from "./profile-actions";
import { useTranslation } from "react-i18next";
import { tablet, mobile } from "@appserver/components/utils/device";
import { combineUrl, deleteCookie } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../package.json";
import { isDesktop, isMobile, isMobileOnly } from "react-device-detect";
import AboutDialog from "../../pages/About/AboutDialog";
import DebugInfoDialog from "../../pages/DebugInfo";
import HeaderCatalogBurger from "./header-catalog-burger";

const { proxyURL } = AppServerConfig;
const homepage = config.homepage;

const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, homepage);
const ABOUT_URL = combineUrl(PROXY_HOMEPAGE_URL, "/about");
const SETTINGS_URL = combineUrl(PROXY_HOMEPAGE_URL, "/settings");
const PROFILE_SELF_URL = combineUrl(
  PROXY_HOMEPAGE_URL,
  "/products/people/view/@self"
);
const PROFILE_MY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/my");
const COMMUNITY_URL = "https://forum.onlyoffice.com/c/personal/43";
const HELP_URL =
  "https://helpcenter.onlyoffice.com/userguides/workspace-personal.aspx";

const StyledNav = styled.nav`
  display: flex;
  padding: 0 20px 0 16px;
  align-items: center;
  position: absolute;
  right: 0;
  height: 48px;
  z-index: 180 !important;

  & > div {
    margin: 0 0 0 16px;
    padding: 0;
    min-width: 24px;
  }

  @media ${tablet} {
    padding: 0 16px;
  }
  .icon-profile-menu {
    cursor: pointer;
  }

  ${isMobile &&
  css`
    padding: 0 16px 0 16px !important;
  `}

  @media ${mobile} {
    padding: 0 0 0 16px;
  }

  ${isMobileOnly &&
  css`
    padding: 0 0 0 16px !important;
  `}
`;
const HeaderNav = ({
  history,
  modules,
  user,
  logout,
  isAuthenticated,
  peopleAvailable,
  isPersonal,
  userIsUpdate,
  setUserIsUpdate,
  buildVersionInfo,
  debugInfo,
  settingsModule,
  setHotkeyPanelVisible,
  currentProductId,
}) => {
  const { t } = useTranslation(["NavMenu", "Common", "About"]);
  const [visibleAboutDialog, setVisibleAboutDialog] = useState(false);
  const [visibleDebugDialog, setVisibleDebugDialog] = useState(false);

  const onProfileClick = useCallback(() => {
    peopleAvailable
      ? history.push(PROFILE_SELF_URL)
      : history.push(PROFILE_MY_URL);
  }, []);

  const onAboutClick = useCallback(() => {
    if (isDesktop) {
      setVisibleAboutDialog(true);
    } else {
      history.push(ABOUT_URL);
    }
  }, []);

  const onHotkeysClick = useCallback(() => {
    setHotkeyPanelVisible(true);
  }, []);

  const onCommunityClick = useCallback(() => {
    window.open(COMMUNITY_URL, "_blank");
  }, []);

  const onHelpClick = useCallback(() => {
    window.open(HELP_URL, "_blank");
  }, []);

  const onCloseDialog = () => setVisibleDialog(false);
  const onDebugClick = useCallback(() => {
    setVisibleDebugDialog(true);
  }, []);

  const onCloseAboutDialog = () => setVisibleAboutDialog(false);
  const onCloseDebugDialog = () => setVisibleDebugDialog(false);

  const onSwitchToDesktopClick = useCallback(() => {
    deleteCookie("desktop_view");
    window.open(
      `${window.location.origin}?desktop_view=true`,
      "_self",
      "",
      true
    );
  }, []);

  const onLogoutClick = useCallback(() => logout && logout(), [logout]);

  const settingsUrl =
    settingsModule && combineUrl(PROXY_HOMEPAGE_URL, settingsModule.link);
  const onSettingsClick =
    settingsModule && useCallback(() => history.push(settingsUrl), []);

  const getCurrentUserActions = useCallback(() => {
    const settings =
      settingsModule && !isPersonal
        ? {
            key: "SettingsBtn",
            label: t("Common:Settings"),
            onClick: onSettingsClick,
            url: settingsUrl,
          }
        : null;

    let hotkeys = null;
    if (modules) {
      const moduleIndex = modules.findIndex((m) => m.appName === "files");

      if (
        moduleIndex !== -1 &&
        modules[moduleIndex].id === currentProductId &&
        !isMobile
      ) {
        hotkeys = {
          key: "HotkeysBtn",
          label: t("Common:Hotkeys"),
          onClick: onHotkeysClick,
        };
      }
    }

    const communityForum = isPersonal
      ? {
          key: "CommunityBtn",
          label: t("CommunityForum"),
          onClick: onCommunityClick,
        }
      : null;

    const helpCenter = isPersonal
      ? {
          key: "HelpBtn",
          label: t("HelpCenter"),
          onClick: onHelpClick,
        }
      : null;
    const actions = [
      {
        key: "ProfileBtn",
        label: t("Common:Profile"),
        onClick: onProfileClick,
        url: peopleAvailable ? PROFILE_SELF_URL : PROFILE_MY_URL,
      },
      settings,
      {
        key: "SwitchToBtn",
        ...(!isPersonal && {
          label: t("TurnOnDesktopVersion"),
          onClick: onSwitchToDesktopClick,
          url: `${window.location.origin}?desktop_view=true`,
          target: "_self",
        }),
      },
      hotkeys,
      communityForum,
      helpCenter,
      {
        key: "AboutBtn",
        label: t("AboutCompanyTitle"),
        onClick: onAboutClick,
        url: ABOUT_URL,
      },
      {
        key: "LogoutBtn",
        label: t("LogoutButton"),
        onClick: onLogoutClick,
        isButton: true,
      },
    ];

    if (debugInfo) {
      actions.splice(3, 0, {
        key: "DebugBtn",
        label: "Debug Info",
        onClick: onDebugClick,
      });
    }

    return actions;
  }, [onProfileClick, onAboutClick, onLogoutClick, currentProductId]);
  //console.log("HeaderNav render");
  return (
    <StyledNav className="profileMenuIcon hidingHeader">
      {isAuthenticated && user ? (
        <>
          <ProfileActions
            userActions={getCurrentUserActions()}
            user={user}
            userIsUpdate={userIsUpdate}
            setUserIsUpdate={setUserIsUpdate}
          />
          {/* <HeaderCatalogBurger
            isProduct={currentProductId !== "home"}
            onClick={toggleArticleOpen}
          /> */}
        </>
      ) : (
        <></>
      )}

      <AboutDialog
        t={t}
        visible={visibleAboutDialog}
        onClose={onCloseAboutDialog}
        personal={isPersonal}
        buildVersionInfo={buildVersionInfo}
      />

      {debugInfo && (
        <DebugInfoDialog
          visible={visibleDebugDialog}
          onClose={onCloseDebugDialog}
        />
      )}
    </StyledNav>
  );
};

HeaderNav.displayName = "HeaderNav";

HeaderNav.propTypes = {
  history: PropTypes.object,
  modules: PropTypes.array,
  user: PropTypes.object,
  logout: PropTypes.func,
  isAuthenticated: PropTypes.bool,
  isLoaded: PropTypes.bool,
  currentProductId: PropTypes.string,
  toggleArticleOpen: PropTypes.func,
};

export default withRouter(
  inject(({ auth }) => {
    const {
      settingsStore,
      userStore,
      isAuthenticated,
      isLoaded,
      language,
      logout,
    } = auth;
    const {
      defaultPage,
      personal: isPersonal,
      version: versionAppServer,
      currentProductId,
      toggleArticleOpen,
      buildVersionInfo,
      debugInfo,
      setHotkeyPanelVisible,
    } = settingsStore;
    const { user, userIsUpdate, setUserIsUpdate } = userStore;
    const modules = auth.availableModules;
    const settingsModule = modules.find((module) => module.id === "settings");

    return {
      isPersonal,
      user,
      isAuthenticated,
      isLoaded,
      language,
      defaultPage: defaultPage || "/",
      modules,
      logout,
      peopleAvailable: modules.some((m) => m.appName === "people"),
      versionAppServer,
      userIsUpdate,
      setUserIsUpdate,
      currentProductId,
      toggleArticleOpen,
      buildVersionInfo,
      debugInfo,
      settingsModule,
      setHotkeyPanelVisible,
    };
  })(observer(HeaderNav))
);
