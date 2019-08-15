import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Layout, Toast } from 'asc-web-components';
import { logout } from '../store/auth/actions';
import { withTranslation } from 'react-i18next';

class PeopleLayout extends React.PureComponent {
    onProfileClick = () => {
        console.log('ProfileBtn');
        const { history, settings } = this.props;
        history.push(`${settings.homepage}/view/@self`);
    }

    onAboutClick = () => {
        console.log('About clicked');
    }

    onLogoutClick = () => {
        this.props.logout();
    }

    onLogoClick = () => window.open("/", '_self');

    render() {
        const { auth, children, t } = this.props;

        const currentUserActions = [
            {
                key: 'ProfileBtn', label: t('Resource:Profile'), onClick: this.onProfileClick
            },
            {
                key: 'AboutBtn', label: t('Resource:AboutCompanyTitle'), onClick: this.onAboutClick
            },
            {
                key: 'LogoutBtn', label: t('Resource:LogoutButton'), onClick: this.onLogoutClick
            },
        ];

        const newProps = auth.isAuthenticated && auth.isLoaded
            ? {
                currentUserActions: currentUserActions,
                onLogoClick: this.onLogoClick,
                ...this.props
            }
            : {};

        console.log("PeopleLayout render");
        return (
            <>
                <Toast />
                <Layout key="1" {...newProps}>{children}</Layout>
            </>
        );
    }
};

PeopleLayout.propTypes = {
    auth: PropTypes.object.isRequired,
    logout: PropTypes.func.isRequired
};

const getAvailableModules = (modules) => {
    const separator = { seporator: true, id: 'nav-seporator-1' };
    const products = modules.map(product => {
        return {
            id: product.id,
            title: product.title,
            iconName: 'PeopleIcon',
            notifications: 0,
            url: product.link,
            onClick: () => window.open(product.link, '_self'),
            onBadgeClick: e => console.log('PeopleIconBadge Clicked', e)
        };
    }) || [];

    return products.length ? [separator, ...products] : products;
};

function mapStateToProps(state) {
    return {
        auth: state.auth,
        availableModules: getAvailableModules(state.auth.modules),
        currentUser: state.auth.user,
        currentModuleId: state.auth.settings.currentProductId,
        settings: state.auth.settings
    };
}

export default connect(mapStateToProps, { logout })(withRouter(withTranslation()(PeopleLayout)));
