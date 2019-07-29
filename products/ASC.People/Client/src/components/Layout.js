import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Layout } from 'asc-web-components';
import { logout } from '../store/auth/actions';
import { getAvailableModules } from '../store/auth/selectors';

var config = require('../../package.json');

const PeopleLayout = props => {
    const { auth, logout, children, history } = props;
    const currentUserActions = [
        {
            key: 'ProfileBtn', label: 'Profile', onClick: () => {
                console.log('ProfileBtn')
                history.push(`${config.homepage}/view/@self`);
            }
        },
        {
            key: 'AboutBtn', label: 'About', onClick: () => {
                console.log('AboutBtn');
            }
        },
        {
            key: 'LogoutBtn', label: 'Log out', onClick: () => {
                logout();
            }
        },
    ];

    const newProps = auth.isAuthenticated && auth.isLoaded 
        ? { currentUserActions: currentUserActions, 
            onLogoClick: () => window.open("/", '_self'), 
            ...props } 
        : {};

    return (<Layout key="1" {...newProps}>{children}</Layout>);
};

PeopleLayout.propTypes = {
    auth: PropTypes.object.isRequired,
    logout: PropTypes.func.isRequired
};

function mapStateToProps(state) {
    return {
        auth: state.auth,
        availableModules: getAvailableModules(state.auth.modules),
        currentUser: state.auth.user,
        currentModuleId: state.auth.currentModuleId
    };
}

export default connect(mapStateToProps, { logout })(withRouter(PeopleLayout));
