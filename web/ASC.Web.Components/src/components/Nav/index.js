import React from 'react'
import { Navbar } from 'reactstrap'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import './styles.scss'

const StyledNav = styled(Navbar)`
    background: #0f4071;
    color: #c5c5c5;
    height: 48px;
    padding-top: 4px;
    z-index: 1;
`;

const Nav = props => {
    const { children } = props;

    return (
        <StyledNav dark>{children}</StyledNav>
    );
}

Nav.propTypes = {
    text: PropTypes.string
}

export default Nav