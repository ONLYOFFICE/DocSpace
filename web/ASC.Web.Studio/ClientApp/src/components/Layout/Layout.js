import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Container, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem, Row } from 'reactstrap';
import { NavMenu, NavLogo } from 'asc-web-components';
import { logout } from '../../actions/authActions';

const Layout = props => {
    const { auth, logout, children, history } = props;

    return (
        <>
            <header>
                <NavMenu>
                    <NavLogo imageUrl="images/light_small_general.svg" onClick={() => history.push('/')} />
                    {auth.isAuthenticated && (
                        <UncontrolledDropdown inNavbar size="sm">
                            <DropdownToggle caret nav style={{ fontFamily: '"Open Sans", sans-serif', fontSize: '12px', color: '#c5c5c5',  }}>
                                {auth.user.displayName}
                            </DropdownToggle>
                            <DropdownMenu right>
                                <DropdownItem onClick={() => history.push("/products/people/profile")}>Profile</DropdownItem>
                                <DropdownItem onClick={() => {
                                    logout();
                                    history.push('/');
                                }}>Sign Out</DropdownItem>
                            </DropdownMenu>
                        </UncontrolledDropdown>
                        )}
                </NavMenu>
            </header>
            <main className="main">
                <Container fluid className="mainPageContent">
                    <Row>
                        {children}
                    </Row>
                </Container>
            </main>
        </>
    )
};

Layout.propTypes = {
    auth: PropTypes.object.isRequired,
    logout: PropTypes.func.isRequired
};

function mapStateToProps(state) {
    return {
        auth: state.auth
    };
}

export default connect(mapStateToProps, { logout })(withRouter(Layout));
