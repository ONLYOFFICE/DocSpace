import React from 'react'
import { NavbarBrand } from 'reactstrap'
import styled from 'styled-components'
import PropTypes from 'prop-types'

const StyledNavLogo = styled(NavbarBrand)`
    cursor: pointer;
    padding: 4px 0 5px;
    margin: 0 24px 0 0;
`;

const NavLogo = props => {
    const { logoUrl, href } = props;
    return (
        <StyledNavLogo href={href}>
            <svg
                height={23}
                width={142}
                xmlns="http://www.w3.org/2000/svg"
                xmlnsXlink="http://www.w3.org/1999/xlink">
                
                <image xlinkHref={logoUrl} />
            </svg>
        </StyledNavLogo>
    );
}

NavLogo.propTypes = {
    logoUrl: PropTypes.string.isRequired
}

export default NavLogo