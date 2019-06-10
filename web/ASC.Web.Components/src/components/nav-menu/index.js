import React from 'react'
import { Navbar } from 'reactstrap'
import styled from 'styled-components'
import PropTypes from 'prop-types'

const StyledNav = styled(Navbar)`
    background: #0f4071;
    color: #c5c5c5;
    height: 48px;
    padding-top: 4px;
    z-index: 1;
`;

const NavMenu = props => {
    const { children } = props;

    return (
        <StyledNav dark>
            {children}
        </StyledNav>
    );
}

NavMenu.propTypes = {
    children: PropTypes.oneOfType([
        PropTypes.arrayOf(PropTypes.node),
        PropTypes.node
    ])
}

export default NavMenu