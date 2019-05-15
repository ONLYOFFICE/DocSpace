import * as React from 'react';
import { Card, Row, Col, CardBody, CardImg, CardText, CardTitle } from 'reactstrap';
import { Link } from 'react-router-dom';
import './ModuleTile.scss';

const ModuleTile = (props) => {
    const { title, link, image, description, isPrimary } = props;
    const baseImagePath = "images/"
    const imageSrc = baseImagePath + image;

    return (
        <Card>
            {isPrimary ? (
                <Row className="justify-content-md-center">
                    <Col md="auto">
                        <Link to={link}>
                            <CardImg src={imageSrc} style={{ border: 'none', height: '241px', width: '240px' }} />
                        </Link>
                    </Col>
                    <Col md="6" className="align-middle">
                        <CardBody>
                            <Link to={link}>
                                <CardTitle style={{ fontSize: '36px', color: 'black' }}>{title}</CardTitle>
                            </Link>
                            <CardText style={{ fontSize: '18px', color: 'black', textDecoration: 'none', clear: 'both' }}>{description}</CardText>
                        </CardBody>
                    </Col>
                </Row>
            ) : (
                    <Link to={link}>
                        <CardBody className="text-center">
                            <Row>
                                <Col>
                                    <CardImg src={imageSrc} style={{ border: 'none', height: '100px', width: '100px' }} />
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <CardTitle className="align-middle">{title}</CardTitle>
                                </Col>
                            </Row>
                        </CardBody>
                    </Link>
                )}
        </Card>
    );
};

ModuleTile.defaultProps = {
    isPrimary: false,
    description: ''
}

export default ModuleTile;