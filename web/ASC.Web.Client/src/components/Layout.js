import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Layout } from 'asc-web-components';
import { logout } from '../store/auth/actions';
import { withTranslation } from 'react-i18next';

const StudioLayout = props => {
    const { auth, logout, children, history, t } = props;
    console.log("StudioLayout", props);
    const currentUserActions = [
        {
            key: 'ProfileBtn', label: t('Profile'), onClick: () => {
                window.location.href = '/products/people/view/@self';
            }
        },
        {
            key: 'AboutBtn', label: t('AboutCompanyTitle'), onClick: () => {
                history.push('/about');
            }
        },
        {
            key: 'LogoutBtn', label: t('LogoutButton'), onClick: () => {
                logout();
                history.push('/');
            }
        },
    ];

    const layoutProps = { currentUserActions: currentUserActions, ...props };

    return (
        auth.isAuthenticated && auth.isLoaded
                ? <Layout key="1" {...layoutProps}>{children}</Layout>
                : <Layout key="2">{children}</Layout>
    );
};

StudioLayout.propTypes = {
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
    let availableModules = getAvailableModules(state.auth.modules);
    return {
        auth: state.auth,
        availableModules: availableModules,
        currentUser: state.auth.user,
        currentModuleId: ''
    };
}


export default connect(mapStateToProps, { logout })(withRouter(withTranslation()(StudioLayout)));
