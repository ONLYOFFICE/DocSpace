import React, { useCallback } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import styled from "styled-components";
import NavItem from "./nav-item";
import ProfileActions from "./profile-actions";

import { useTranslation } from "react-i18next";
import { utils } from "asc-web-components";
const { tablet } = utils.device;
import { logout } from "../../../store/auth/actions";

import {
  getCurrentUser,
  getLanguage,
  getIsolateModules,
  getIsLoaded,
} from "../../../store/auth/selectors";

const StyledNav = styled.nav`
  display: flex;
  padding: 0 24px 0 16px;
  align-items: center;
  position: absolute;
  right: 0;
  height: 56px;
  z-index: 190;

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
const HeaderNav = React.memo(
  ({ history, homepage, modules, user, logout, isAuthenticated }) => {
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

    //console.log("HeaderNav render");
    return (
      <StyledNav>
        {modules.map((module) => (
          <NavItem
            key={module.id}
            iconName={module.iconName}
            iconUrl={module.iconUrl}
            badgeNumber={module.notifications}
            onClick={module.onClick}
            onBadgeClick={module.onBadgeClick}
            noHover={true}
          />
        ))}

        {isAuthenticated && user ? (
          <ProfileActions userActions={getCurrentUserActions()} user={user} />
        ) : (
          <></>
        )}
      </StyledNav>
    );
  }
);

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

function mapStateToProps(state) {
  const { settings, isAuthenticated } = state.auth;
  const { defaultPage, homepage } = settings;

  return {
    homepage,
    defaultPage: defaultPage || "/",
    user: getCurrentUser(state),
    isAuthenticated,
    isLoaded: getIsLoaded(state),
    modules: getIsolateModules(state),
    language: getLanguage(state),
  };
}

export default connect(mapStateToProps, { logout })(HeaderNav);
