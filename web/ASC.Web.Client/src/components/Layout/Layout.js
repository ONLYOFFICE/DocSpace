import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Layout } from 'asc-web-components';
import { logout } from '../../actions/authActions';

const StudioLayout = props => {
    const { auth, logout, children, history } = props;
    const currentUserActions = [
        {
            key: 'ProfileBtn', label: 'Profile', onClick: () => {
                window.location.href = '/products/people/view/@self';
            }
        },
        {
            key: 'AboutBtn', label: 'About', onClick: () => {
                history.push('/about');
            }
        },
        {
            key: 'LogoutBtn', label: 'Log out', onClick: () => {
                logout();
                history.push('/');
            }
        },
    ];

    console.log(props);

    const layoutProps = { currentUserActions: currentUserActions, ...props };
    console.log(auth.isAuthenticated, auth.isLoaded);

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

function convertModules(modules) {
    const separator = { seporator: true, id: 'nav-seporator-1' };
    const chat = {
        id: '22222222-2222-2222-2222-222222222222',
        title: 'Chat',
        iconName: 'ChatIcon',
        notifications: 3,
        url: '/products/chat/',
        onClick: () => window.open('/products/chat/', '_blank'),
        onBadgeClick: e => console.log('ChatIconBadge Clicked')(e),
        isolateMode: true
    };

    let items = modules.map(item => {
        return {
            id: '11111111-1111-1111-1111-111111111111',
            title: item.title,
            iconName: 'PeopleIcon',
            notifications: 0,
            url: item.link,
            onClick: () => window.open(item.link, '_self'),
            onBadgeClick: e => console.log('DocumentsIconBadge Clicked')(e)
        };
    }) || [];

    return items.length ? [separator, ...items, chat] : items;
}

function mapStateToProps(state) {
    let availableModules = convertModules(state.auth.modules);
    return {
        auth: state.auth,
        availableModules: availableModules,
        currentUser: state.auth.user,
        currentModuleId: ''
    };
}


export default connect(mapStateToProps, { logout })(withRouter(StudioLayout));
