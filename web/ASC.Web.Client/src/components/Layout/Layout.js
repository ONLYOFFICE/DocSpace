import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Container, Row } from 'reactstrap';
import { Layout } from 'asc-web-components';
import { logout } from '../../actions/authActions';

const MainPageContent = ({ children }) => (
    <main className="main">
        <Container className="mainPageContent">
            <Row>
                {children}
            </Row>
        </Container>
    </main>
);

const StudioLayout = props => {
    const { auth, logout, children, history } = props;
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
                logout();
                history.push('/');
            }
        },
    ];

    console.log(props);

    const layoutProps = { currentUserActions: currentUserActions, ...props };

    return (
        <>
            {auth.isAuthenticated && auth.isLoaded
                ? (
                    <Layout {...layoutProps}>
                        <MainPageContent>{children}</MainPageContent>
                    </Layout>
                )
                : (<MainPageContent>{children}</MainPageContent>)
            }
        </>
    )
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
            onClick: () => window.open(item.link, '_blank'),
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
