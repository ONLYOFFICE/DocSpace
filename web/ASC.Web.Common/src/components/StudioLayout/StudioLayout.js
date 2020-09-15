import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { logout } from "../../store/auth/actions";
import PureStudioLayout from "./PureStudioLayout";
import { changeLanguage } from "../../utils";
import isEqual from "lodash/isEqual";

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
    iconName: item.iconName || iconName || "PeopleIcon", //TODO: Change to URL
    iconUrl: item.iconUrl,
    notifications: 0,
    url: item.link,
    onClick: e => {
      if (e) {
        window.open(item.link, "_self");
        e.preventDefault();
      }
    },
    onBadgeClick: e => console.log(iconName + " Badge Clicked", e)
  };
};

const getCustomModules = isAdmin => {
  if (!isAdmin) {
    return [];
  }

  /*  const separator = getSeparator("nav-modules-separator");
    const settingsModuleWrapper = toModuleWrapper(
      {
        id: "settings",
        title: i18n.t('Settings'),
        link: "/settings"
      },
      "SettingsIcon"
    );
  const paymentsModuleWrapper = toModuleWrapper(
    {
      id: "payments",
      title: i18n.t("Payments"),
      link: "/payments"
    },
    "PaymentsIcon"
  );

  return [separator, settingsModuleWrapper, paymentsModuleWrapper];*/   // Temporarily hiding the settings module
  return [];
};

const getAvailableModules = (modules, currentUser) => {
  if (!modules.length) {
    return [];
  }

  const isUserAdmin = currentUser.isAdmin;
  const customModules = getCustomModules(isUserAdmin);
  const separator = getSeparator("nav-products-separator");
  const products = modules.map(m => toModuleWrapper(m));

  return [separator, ...products, ...customModules];
};

const StudioLayoutContainer = withTranslation()(PureStudioLayout);

class StudioLayout extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);
  }

  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    return <StudioLayoutContainer i18n={i18n} {...this.props} />;
  }
}

StudioLayout.displayName = "StudioLayout";

StudioLayout.propTypes = {
  logout: PropTypes.func.isRequired,
  language: PropTypes.string
};

function mapStateToProps(state) {
  return {
    hasChanges: state.auth.isAuthenticated && state.auth.isLoaded,
    availableModules: getAvailableModules(state.auth.modules, state.auth.user),
    currentUser: state.auth.user,
    currentModuleId: state.auth.settings.currentProductId,
    settings: state.auth.settings,
    modules: state.auth.modules
  };
}

export default connect(mapStateToProps, { logout })(withRouter(StudioLayout));
