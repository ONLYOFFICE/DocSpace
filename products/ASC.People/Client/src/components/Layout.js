import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Layout } from 'asc-web-components';
// import { logout } from '../../actions/authActions';

const PeopleLayout = props => {
    const { auth, children, history } = props;
    const currentUserActions = [
        {
            key: 'ProfileBtn', label: 'Profile', onClick: () => {
                console.log('ProfileBtn')
            }
        },
        {
            key: 'AboutBtn', label: 'About', onClick: () => {
                console.log('AboutBtn');
            }
        },
        {
            key: 'LogoutBtn', label: 'Log out', onClick: () => {
                //logout();
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

PeopleLayout.propTypes = {
    auth: PropTypes.object.isRequired
};

function mapStateToProps(state) {
    return {
        auth: state.auth,
        availableModules: state.auth.modules,
        currentUser: state.auth.user,
        currentModuleId: state.auth.currentModuleId
    };
}

export default connect(mapStateToProps)(withRouter(PeopleLayout));
