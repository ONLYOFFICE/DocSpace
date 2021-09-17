import React, { useCallback, useState } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import NavItem from "./nav-item";
import ProfileActions from "./profile-actions";
import { useTranslation } from "react-i18next";
import { tablet } from "@appserver/components/utils/device";
import { combineUrl, deleteCookie } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../package.json";
import { isDesktop } from "react-device-detect";
import AboutDialog from "../../pages/About/AboutDialog";

const { proxyURL } = AppServerConfig;
const homepage = config.homepage;

const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, homepage);
const ABOUT_URL = combineUrl(PROXY_HOMEPAGE_URL, "/about");
const PROFILE_SELF_URL = combineUrl(
  PROXY_HOMEPAGE_URL,
  "/products/people/view/@self"
);
const PROFILE_MY_URL = combineUrl(PROXY_HOMEPAGE_URL, "/my");

const StyledNav = styled.nav`
  display: flex;
  padding: 0 20px 0 16px;
  align-items: center;
  position: absolute;
  right: 0;
  height: 56px;
  z-index: 190 !important;

  .profile-menu {
    right: 12px;
    top: 66px;

    @media ${tablet} {
      right: 6px;
    }
  }

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
`;
const HeaderNav = ({
  history,
  modules,
  user,
  logout,
  isAuthenticated,
  peopleAvailable,
  isPersonal,
  versionAppServer,
  userIsUpdate,
  setUserIsUpdate,
}) => {
  const { t } = useTranslation(["NavMenu", "Common", "About"]);
  const [visibleDialog, setVisibleDialog] = useState(false);

  const onProfileClick = useCallback(() => {
    peopleAvailable
      ? history.push(PROFILE_SELF_URL)
      : history.push(PROFILE_MY_URL);
  }, []);

  const onAboutClick = useCallback(() => {
    if (isDesktop) {
      setVisibleDialog(true);
    } else {
      history.push(ABOUT_URL);
    }
  }, []);

  const onCloseDialog = () => setVisibleDialog(false);

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

  const getCurrentUserActions = useCallback(() => {
    return [
      {
        key: "ProfileBtn",
        label: t("Common:Profile"),
        onClick: onProfileClick,
        url: peopleAvailable ? PROFILE_SELF_URL : PROFILE_MY_URL,
      },
      {
        key: "SwitchToBtn",
        ...(!isPersonal && {
          label: t("TurnOnDesktopVersion"),
          onClick: onSwitchToDesktopClick,
          url: `${window.location.origin}?desktop_view=true`,
          target: "_self",
        }),
      },
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
      },
    ];
  }, [onProfileClick, onAboutClick, onLogoutClick]);

  //console.log("HeaderNav render");
  return (
    <StyledNav className="profileMenuIcon hidingHeader">
      {modules
        .filter((m) => m.isolateMode)
        .map((m) => (
          <NavItem
            key={m.id}
            iconName={m.iconName}
            iconUrl={m.iconUrl}
            badgeNumber={m.notifications}
            url={m.link}
            onClick={(e) => {
              history.push(m.link);
              e.preventDefault();
            }}
            onBadgeClick={(e) => console.log(m.iconName + "Badge Clicked", e)}
            noHover={true}
          />
        ))}
      {isAuthenticated && user ? (
        <ProfileActions
          userActions={getCurrentUserActions()}
          user={user}
          userIsUpdate={userIsUpdate}
          setUserIsUpdate={setUserIsUpdate}
        />
      ) : (
        <></>
      )}

      <AboutDialog
        t={t}
        visible={visibleDialog}
        onClose={onCloseDialog}
        personal={isPersonal}
        versionAppServer={versionAppServer}
      />
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
    } = settingsStore;
    const { user, userIsUpdate, setUserIsUpdate } = userStore;
    const modules = auth.availableModules;
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
    };
  })(observer(HeaderNav))
);
