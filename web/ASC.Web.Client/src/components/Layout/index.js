import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { Layout, Toast } from "asc-web-components";
import { logout } from "../../store/auth/actions";
import { withTranslation, I18nextProvider } from 'react-i18next';
import i18n from "./i18n";

class PureStudioLayout extends React.Component {
    shouldComponentUpdate(nextProps) {
        if(this.props.hasChanges !== nextProps.hasChanges) {
            return true;
        }

        return false;
    }

    onProfileClick = () => {
        window.location.href = "/products/people/view/@self";
    }

    onAboutClick = () => {
        this.props.history.push("/about");
    }

    onLogoutClick = () => {
        this.props.logout();
        this.props.history.push("/");
    }

    onLogoClick = () => this.props.history.push("/");  

    render() {
        const { hasChanges, children, t } = this.props;

        const currentUserActions = [
            {
                key: 'ProfileBtn', label: t("Profile"), onClick: this.onProfileClick
            },
            {
                key: 'AboutBtn', label: t("AboutCompanyTitle"), onClick: this.onAboutClick
            },
            {
                key: 'LogoutBtn', label: t("LogoutButton"), onClick: this.onLogoutClick
            },
        ];

        const newProps = hasChanges
            ? {
                currentUserActions: currentUserActions,
                onLogoClick: this.onLogoClick,
                ...this.props
            }
            : {};

        console.log("StudioLayout render", newProps);
        return (
            <>
                <Toast />
                <Layout key="1" {...newProps}>{children}</Layout>
            </>
        );
    }
};


const getAvailableModules = modules => {
  const separator = { separator: true, id: "nav-separator-1" };
  const products =
    modules.map(product => {
      return {
        id: product.id,
        title: product.title,
        iconName: "PeopleIcon",
        notifications: 0,
        url: product.link,
        onClick: () => window.open(product.link, "_self"),
        onBadgeClick: e => console.log("PeopleIconBadge Clicked", e)
      };
    }) || [];

  return products.length ? [separator, ...products] : products;
};

function mapStateToProps(state) {
  let availableModules = getAvailableModules(state.auth.modules);
  return {
    hasChanges: state.auth.isAuthenticated && state.auth.isLoaded,
    availableModules: availableModules,
    currentUser: state.auth.user,
    currentModuleId: state.auth.settings.currentModuleId,
    language: state.auth.user.cultureName || state.auth.settings.culture,
  };
};
const StudioLayoutContainer = withTranslation()(PureStudioLayout);

const StudioLayout = (props) => { 
  const { language } = props;
  i18n.changeLanguage(language);
  return (<I18nextProvider i18n={i18n}><StudioLayoutContainer {...props} /></I18nextProvider>);
};

StudioLayout.propTypes = {
  logout: PropTypes.func.isRequired
};

export default connect(
  mapStateToProps,
  { logout }
)(withRouter(StudioLayout));
