import React from "react";
import PropTypes from "prop-types";
import { Toast } from "asc-web-components";
import Layout from "../Layout";

class PureStudioLayout extends React.Component {
  shouldComponentUpdate(nextProps) {
    return this.props.hasChanges !== nextProps.hasChanges ||
    this.props.currentModuleId !== nextProps.currentModuleId;
  }

  onProfileClick = () => {
    const { history, settings } = this.props;
    if (settings.homepage == "/products/people") {
      history.push("/products/people/view/@self");
    } else {
      window.open("/products/people/view/@self", "_self")
    }
  };

  onAboutClick = () => {
    window.open("/about", "_self")
  };

  onLogoutClick = () => {
    this.props.logout();
  };

  onLogoClick = () => {
    window.open("/", "_self")
  };

  render() {
    const { hasChanges, children, t } = this.props;

    const currentUserActions = [
      {
        key: "ProfileBtn",
        label: t("Profile"),
        onClick: this.onProfileClick,
        url: '/products/people/view/@self'
      },
      {
        key: "AboutBtn",
        label: t("AboutCompanyTitle"),
        onClick: this.onAboutClick,
        url: '/about'
      },
      {
        key: "LogoutBtn",
        label: t("LogoutButton"),
        onClick: this.onLogoutClick
      }
    ];

    const newProps = hasChanges
      ? {
          currentUserActions: currentUserActions,
          onLogoClick: this.onLogoClick,
          ...this.props
        }
      : {};

    console.log("PureStudioLayout render", newProps);

    return (
      <>
        <Toast />
        <Layout key="1" {...newProps}>
          {children}
        </Layout>
      </>
    );
  }
}

PureStudioLayout.propTypes = {
  logout: PropTypes.func.isRequired,
  language: PropTypes.string,
  hasChanges: PropTypes.bool,
  currentModuleId: PropTypes.string,
  history: PropTypes.object,
  settings: PropTypes.object,
  children: PropTypes.any,
  t: PropTypes.func
};

export default PureStudioLayout;
