import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Layout, Toast } from 'asc-web-components';
import { logout } from '../store/auth/actions';
import { getAvailableModules } from '../store/auth/selectors';

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
        const { auth, children } = this.props;

        const currentUserActions = [
            {
                key: 'ProfileBtn', label: 'Profile', onClick: this.onProfileClick
            },
            {
                key: 'AboutBtn', label: 'About', onClick: this.onAboutClick
            },
            {
                key: 'LogoutBtn', label: 'Log out', onClick: this.onLogoutClick
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

function mapStateToProps(state) {
    return {
        auth: state.auth,
        availableModules: getAvailableModules(state.auth.modules),
        currentUser: state.auth.user,
        currentModuleId: state.settings.currentProductId,
        settings: state.settings
    };
}

export default connect(mapStateToProps, { logout })(withRouter(PeopleLayout));
