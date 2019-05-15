import React from 'react';
import { Container } from 'reactstrap';
import './Error404.scss'

const Error404 = () => {
    return (
        <Container fluid id="errorNotFound">
            <Container fluid id="wrapper">
                <Container id="container">
                    Sorry, the resource
                    cannot be found.
                </Container>
            </Container>
        </Container>
    );
};

export default Error404;