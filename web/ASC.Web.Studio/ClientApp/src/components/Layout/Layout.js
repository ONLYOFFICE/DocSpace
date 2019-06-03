import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Container, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { NavMenu } from 'asc-web-components';
import setAuthorizationToken from '../../utils/setAuthorizationToken';

const Layout = props => {
    const { auth, children, history } = props;

    const logout = () => {
        setAuthorizationToken();
        history.push('/');
    };

    return (
        <>
            <header>
                <NavMenu logoUrl="images/light_small_general.svg" href="/">
                    {auth.isAuthenticated && (
                        <UncontrolledDropdown inNavbar size="sm">
                            <DropdownToggle caret nav style={{ fontFamily: '"Open Sans", sans-serif', fontSize: '12px', color: '#c5c5c5',  }}>
                                {auth.user.displayName}
                            </DropdownToggle>
                            <DropdownMenu right>
                                <DropdownItem onClick={() => history.push("/products/people/profile")}>Profile</DropdownItem>
                                <DropdownItem onClick={logout}>Sign Out</DropdownItem>
                            </DropdownMenu>
                        </UncontrolledDropdown>
                        )}
                </NavMenu>
            </header>
            <main className="main">
                <Container fluid>
                    {children}
                </Container>
            </main>
        </>
    )
};

Layout.propTypes = {
    auth: PropTypes.object.isRequired
};

function mapStateToProps(state) {
    return {
        auth: state.auth
    };
}

export default connect(mapStateToProps)(withRouter(Layout));
