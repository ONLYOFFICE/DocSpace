import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Layout, Toast } from 'asc-web-components';
import { logout } from '../store/auth/actions';
import { withTranslation } from 'react-i18next';

class PeopleLayout extends React.Component {
    shouldComponentUpdate(nextProps) {
        if(this.props.hasChanges !== nextProps.hasChanges) {
            return true;
        }

        return false;
    }

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
        const { hasChanges, children, t } = this.props;

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

        const newProps = hasChanges
            ? {
                currentUserActions: currentUserActions,
                onLogoClick: this.onLogoClick,
                ...this.props
            }
            : {};

        console.log("PeopleLayout render", newProps);
        return (
            <>
                <Toast />
                <Layout key="1" {...newProps}>{children}</Layout>
            </>
        );
    }
};

PeopleLayout.propTypes = {
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
        hasChanges: state.auth.isAuthenticated && state.auth.isLoaded,
        availableModules: getAvailableModules(state.auth.modules),
        currentUser: state.auth.user,
        currentModuleId: state.auth.settings.currentProductId,
        settings: state.auth.settings
    };
}

export default connect(mapStateToProps, { logout })(withRouter(withTranslation()(PeopleLayout)));
