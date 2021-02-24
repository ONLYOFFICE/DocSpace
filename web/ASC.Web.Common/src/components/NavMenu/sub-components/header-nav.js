import React, { useCallback, useState } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import NavItem from "./nav-item";
import ProfileActions from "./profile-actions";
import { useTranslation } from "react-i18next";
import { utils } from "asc-web-components";

import { inject, observer } from "mobx-react";
const { tablet } = utils.device;

const StyledNav = styled.nav`
  display: flex;
  padding: 0 24px 0 16px;
  align-items: center;
  position: absolute;
  right: 0;
  height: 56px;
  z-index: ${(props) => (props.isOpen ? "191" : "190")} !important;

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
`;
const HeaderNav = ({
  history,
  homepage,
  modules,
  user,
  logout,
  isAuthenticated,
}) => {
  const { t } = useTranslation();
  const onProfileClick = useCallback(() => {
    if (homepage == "/products/people") {
      history.push("/products/people/view/@self");
    } else {
      window.open("/products/people/view/@self", "_self");
    }
  }, []);

  const onAboutClick = useCallback(() => window.open("/about", "_self"), []);

  const onLogoutClick = useCallback(() => logout && logout(), [logout]);

  const getCurrentUserActions = useCallback(() => {
    const currentUserActions = [
      {
        key: "ProfileBtn",
        label: t("Profile"),
        onClick: onProfileClick,
        url: "/products/people/view/@self",
      },
      {
        key: "AboutBtn",
        label: t("AboutCompanyTitle"),
        onClick: onAboutClick,
        url: "/about",
      },
      {
        key: "LogoutBtn",
        label: t("LogoutButton"),
        onClick: onLogoutClick,
      },
    ];

    return currentUserActions;
  }, [onProfileClick, onAboutClick, onLogoutClick]);

  const [isOpen, setIsOpen] = useState(false); //TODO: Need to refactoring
  const isOpenProfileMenu = (value) => {
    setIsOpen(value);
  };
  //console.log("HeaderNav render");
  return (
    <StyledNav isOpen={isOpen} className="profileMenuIcon hidingHeader">
      {modules
        .filter((m) => m.isolateMode)
        .map((m) => (
          <NavItem
            key={m.id}
            iconName={m.iconName}
            iconUrl={m.iconUrl}
            badgeNumber={m.notifications}
            onClick={(e) => {
              window.open(m.link, "_self");
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
          isOpenProfileMenu={isOpenProfileMenu}
        />
      ) : (
        <></>
      )}
    </StyledNav>
  );
};

HeaderNav.displayName = "HeaderNav";

HeaderNav.propTypes = {
  history: PropTypes.object,
  homepage: PropTypes.string,
  modules: PropTypes.array,
  user: PropTypes.object,
  logout: PropTypes.func,
  isAuthenticated: PropTypes.bool,
  isLoaded: PropTypes.bool,
};

export default inject(({ auth }) => {
  const {
    settingsStore,
    userStore,
    isAuthenticated,
    isLoaded,
    language,
    logout,
  } = auth;
  const { homepage, defaultPage } = settingsStore;
  const { user } = userStore;

  return {
    user,
    homepage,
    isAuthenticated,
    isLoaded,
    language,
    defaultPage: defaultPage || "/",
    modules: auth.availableModules,
    logout,
  };
})(observer(HeaderNav));
