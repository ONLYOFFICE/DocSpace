import React from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import Avatar from "@appserver/components/avatar";
import Text from "@appserver/components/text";
import ContextMenuButton from "@appserver/components/context-menu-button";
import { isDesktop, isMobile, isMobileOnly } from "react-device-detect";
import { isTablet } from "@appserver/components/utils/device";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const { proxyURL } = AppServerConfig;

const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, "/");
const PROFILE_SELF_URL = combineUrl(
  PROXY_HOMEPAGE_URL,
  "/products/people/view/@self"
);
const PROFILE_MY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/my");

const StyledProfile = styled.div`
  position: fixed;
  bottom: 0;
  padding: 16px 0;
  display: flex;
  align-items: center;
  flex-flow: row wrap;
  gap: 16px;
  width: ${(props) =>
    !props.tablet
      ? "211px"
      : props.tablet && props.showText
      ? "211px"
      : "44px"};

  ${(props) =>
    props.tablet &&
    css`
      padding: 14px 6px;
    `}

  .option-button {
    margin-left: auto;
  }
`;

const Profile = (props) => {
  const {
    history,
    user,
    isPersonal,
    modules,
    settingsModule,
    currentProductId,
    debugInfo,
    peopleAvailable,
    showText,
    setHotkeyPanelVisible,
    logout,
  } = props;
  const { t } = useTranslation("Common");

  const getUserRole = (user) => {
    let isModuleAdmin = user.listAdminModules && user.listAdminModules.length;

    if (user.isOwner) return "owner";
    if (user.isAdmin || isModuleAdmin) return "admin";
    if (user.isVisitor) return "guest";
    return "user";
  };

  const onProfileClick = () => {
    peopleAvailable
      ? history.push(PROFILE_SELF_URL)
      : history.push(PROFILE_MY_URL);
  };

  const onSettingsClick = () => {
    const settingsUrl =
      settingsModule && combineUrl(PROXY_HOMEPAGE_URL, settingsModule.link);
    history.push(settingsUrl);
  };

  const onHotkeysClick = () => {
    setHotkeyPanelVisible(true);
  };

  const onAboutClick = () => {
    console.log("onAboutClick");
  };

  const onLogoutClick = () => {
    logout && logout();
  };

  const onDebugClick = () => {
    console.log("onDebugClick");
  };

  const getContextOptions = () => {
    const settings =
      settingsModule && !isPersonal
        ? {
            key: "SettingsBtn",
            icon: "/static/images/catalog.settings.react.svg",
            label: t("Common:Settings"),
            onClick: onSettingsClick,
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
          icon: "/static/images/hotkeys.react.svg",
          label: t("Common:Hotkeys"),
          onClick: onHotkeysClick,
        };
      }
    }
    const actions = [
      {
        key: "ProfileBtn",
        icon: "/static/images/profile.react.svg",
        label: t("Common:Profile"),
        onClick: onProfileClick,
      },
      settings,
      hotkeys,
      {
        key: "AboutBtn",
        icon: "/static/images/info.react.svg",
        label: t("AboutCompanyTitle"),
        onClick: onAboutClick,
      },
      {
        isSeparator: true,
        key: "separator",
      },
      {
        key: "LogoutBtn",
        icon: "/static/images/logout.react.svg",
        label: t("LogoutButton"),
        onClick: onLogoutClick,
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
  };

  const tablet = isTablet();
  const avatarSize = tablet ? "min" : "base";
  const userRole = getUserRole(user);

  return (
    <StyledProfile showText={showText} tablet={tablet}>
      <Avatar
        size={avatarSize}
        role={userRole}
        source={user.avatar}
        userName={user.displayName}
      />
      {(!tablet || showText) && (
        <>
          <Text fontSize="12px" fontWeight={600}>
            {user.displayName}
          </Text>
          <div className="option-button">
            <ContextMenuButton
              zIndex={402}
              directionX="left"
              directionY="top"
              iconName="images/vertical-dots.react.svg"
              size={15}
              isFill
              getData={getContextOptions}
              isDisabled={false}
              usePortal={true}
            />
          </div>
        </>
      )}
    </StyledProfile>
  );
};

export default withRouter(
  inject(({ auth }) => {
    const { userStore, settingsStore, logout } = auth;
    const { user } = userStore;
    const {
      personal: isPersonal,
      currentProductId,
      debugInfo,
      setHotkeyPanelVisible,
    } = settingsStore;
    const modules = auth.availableModules;
    const settingsModule = modules.find((module) => module.id === "settings");

    return {
      user,
      isPersonal,
      modules,
      settingsModule,
      currentProductId,
      peopleAvailable: modules.some((m) => m.appName === "people"),
      debugInfo,
      setHotkeyPanelVisible,
      logout,
    };
  })(observer(Profile))
);
