import React from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from 'asc-web-components';

const Layout = props => {
    const { children } = props;

    return (
        <>
            <header>
                <NavMenu logoUrl="images/light_small_general.svg" href="/" />
            </header>
            <Container>
                {children}
            </Container>
        </>
    )
};

export default Layout;
