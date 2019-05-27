import React from 'react'
import { Navbar } from 'reactstrap'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import NavLogo from '../nav-logo'

const StyledNav = styled(Navbar)`
    background: #0f4071;
    color: #c5c5c5;
    height: 48px;
    padding-top: 4px;
    z-index: 1;
`;

const NavMenu = props => {
    const { href, logoUrl, children } = props;

    return (
        <StyledNav dark>
            <NavLogo logoUrl={logoUrl} href={href} />
            {children}
        </StyledNav>
    );
}

NavMenu.propTypes = {
    href: PropTypes.string.isRequired,
    logoUrl: PropTypes.string.isRequired,
    children: PropTypes.oneOfType([
        PropTypes.arrayOf(PropTypes.node),
        PropTypes.node
    ])
}

export default NavMenu