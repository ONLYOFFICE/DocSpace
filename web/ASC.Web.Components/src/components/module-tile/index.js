import React from 'react';
import PropTypes from 'prop-types';
import { Card, Row, Col, CardBody, CardImg, CardTitle } from 'reactstrap';
import styled from 'styled-components';

const PrimaryImage = styled(CardImg)`
    border: none;
    height: 241px;
    width: 240px;
`;

const NotPrimaryImage = styled(CardImg)`
    border: none;
    height: 100px;
    width: 100px;
`;

const DescriptionText = styled(CardTitle)`
    font-size: 18px; 
    color: black;
    text-decoration: none;
    clear: both;
`;

const PrimaryTitle = styled(CardTitle)`
    font-size: 36px; 
    color: black;
`;

const NotPrimaryTitle = styled(CardTitle)`
    font-size: 13px; 
    color: black;
`;

const TileCard = styled(Card)`
    border: none;

    &:hover {
        .selectable {
            text-decoration: underline;
            cursor: pointer;
        }
    }
`;

const ModuleTile = (props) => {
    const { title, imageUrl, link, description, isPrimary, onClick } = props;

    return (
        <TileCard>
            {isPrimary ? (
                <Row className="justify-content-md-center">
                    <Col md="auto">
                        <PrimaryImage className="selectable" src={imageUrl} onClick={event => onClick(event, link) } />
                    </Col>
                    <Col md="6" className="align-middle">
                        <CardBody>
                            <PrimaryTitle className="selectable" onClick={event => onClick(event, link) }>{title}</PrimaryTitle>
                            <DescriptionText>{description}</DescriptionText>
                        </CardBody>
                    </Col>
                </Row>
            ) : (
                    <CardBody className="text-center selectable">
                        <Row>
                            <Col>
                                <NotPrimaryImage src={imageUrl} onClick={event => onClick(event, link) } />
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <NotPrimaryTitle onClick={event => onClick(event, link) }>{title}</NotPrimaryTitle>
                            </Col>
                        </Row>
                    </CardBody>
                )}
        </TileCard>
    );
};

ModuleTile.propTypes = {
    title: PropTypes.string.isRequired,
    imageUrl: PropTypes.string.isRequired,
    link: PropTypes.string.isRequired,
    description: PropTypes.string,
    isPrimary: PropTypes.bool
}

ModuleTile.defaultProps = {
    isPrimary: false,
    description: ''
}

export default ModuleTile;