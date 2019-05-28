import React from 'react';
import PropTypes from 'prop-types';
import { Card, Row, Col, CardBody, CardImg, CardTitle } from 'reactstrap';
import styled from 'styled-components';

const PrimaryImage = styled(CardImg)`
    border: none;
    height: 241px;
    width: 240px;
    cursor: pointer;
`;

const NotPrimaryImage = styled(CardImg)`
    border: none;
    height: 100px;
    width: 100px;
    cursor: pointer;
`;

const DescriptionText = styled(CardTitle)`
    font-size: 12px; 
    
    text-decoration: none;
    line-height: 20px;
    clear: both;
`;

const PrimaryTitle = styled(CardTitle)`
    font-size: 36px;
    margin: 46px 0 11px 0;
    cursor: pointer;
`;

const NotPrimaryTitle = styled(CardTitle)`
    font-size: 18px;
    cursor: pointer;
    margin: 14px 0 14px 0;
`;

const TileCard = styled(Card)`
    font-family: 'Open Sans', sans-serif;
    color: #333;
    border: none;

    &:hover {
        .selectable {
            text-decoration: underline;
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
                            <DescriptionText className="description">{description}</DescriptionText>
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