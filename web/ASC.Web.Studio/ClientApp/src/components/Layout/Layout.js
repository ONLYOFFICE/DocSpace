import React from 'react';
import { Container } from 'reactstrap';
import NavMenu from '../ui/NavMenu/NavMenu';

const Layout = props => (
    <>
        <header>
            <NavMenu />
        </header>
        <Container>
            {props.children}
        </Container>
    </>
);

export default Layout;
