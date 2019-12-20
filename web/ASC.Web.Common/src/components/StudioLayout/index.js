import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { withTranslation, I18nextProvider } from "react-i18next";
import i18n from "./i18n";
import { logout } from "../../store/auth/actions";
import PureStudioLayout from "./pureStudioLayout";

const getSeparator = id => {
  return {
    separator: true,
    id: id
  };
};

const toModuleWrapper = (item, iconName) => {
  return {
    id: item.id,
    title: item.title,
    iconName: iconName || "PeopleIcon",
    notifications: 0,
    url: item.link,
    onClick: () => window.open(item.link, "_self"),
    onBadgeClick: e => console.log(iconName + " Badge Clicked", e)
  };
};

const getCustomModules = isAdmin => {
  if (!isAdmin) {
    return [];
  }

  const separator = getSeparator("nav-modules-separator");
  const settingsModuleWrapper = toModuleWrapper(
    {
      id: "settings",
      title: "Settings",
      link: "/settings"
    },
    "SettingsIcon"
  );

  return [separator, settingsModuleWrapper];
};

const getAvailableModules = (modules, currentUser) => {
  if (!modules.length) {
    return [];
  }

  const isUserAdmin = currentUser.isAdmin;
  const customModules = getCustomModules(isUserAdmin);
  const separator = getSeparator("nav-products-separator");
  const products = modules.map(toModuleWrapper);

  return [separator, ...products, ...customModules];
};

function mapStateToProps(state) {
  return {
    hasChanges: state.auth.isAuthenticated && state.auth.isLoaded,
    availableModules: getAvailableModules(state.auth.modules, state.auth.user),
    currentUser: state.auth.user,
    currentModuleId: state.auth.settings.currentProductId,
    settings: state.auth.settings,
    language: state.auth.user.cultureName || state.auth.settings.culture
  };
}

const StudioLayoutContainer = withTranslation()(PureStudioLayout);

const StudioLayout = props => {
  const { language } = props;
  i18n.changeLanguage(language);

  return (
    <I18nextProvider i18n={i18n}>
      <StudioLayoutContainer {...props} />
    </I18nextProvider>
  );
};

StudioLayout.propTypes = {
  logout: PropTypes.func.isRequired,
  language: PropTypes.string
};

export default connect(mapStateToProps, { logout })(withRouter(StudioLayout));
