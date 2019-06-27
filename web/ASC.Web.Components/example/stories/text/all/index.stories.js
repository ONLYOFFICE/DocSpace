import React from 'react';
import { storiesOf } from '@storybook/react';
import { Text } from 'asc-web-components';
import { Container, Row, Col } from 'reactstrap';

const rowStyle = { marginTop: 8 };

storiesOf('Components|Text', module)
    .addParameters({ options: { showAddonPanel: false }})
    .add('all', () => (
        <Container>
            <Row style={rowStyle}>
                <Text elementType="moduleName">Module name</Text>
            </Row>
            <Row style={rowStyle}>
                <Text elementType="mainTitle">Main title</Text>
            </Row>
            <Row style={rowStyle}>
                <Text elementType="h1">H1 - Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus</Text>
            </Row>
            <Row style={rowStyle}>
                <Text elementType="h2">H2 - Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus</Text>
            </Row>
            <Row style={rowStyle}>
                <Text elementType="h3">H3 - Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus</Text>
            </Row>
            <Row style={rowStyle}>
                <Text elementType="p">PARAGRAPH - Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus</Text>
            </Row>
            <Row style={rowStyle}>
                <Text elementType="p">Default black text</Text>
            </Row>
            <Row style={rowStyle}>
                <Text elementType="p" styleType="grayBackground">Gray text on gray bg</Text>
            </Row>
            <Row style={rowStyle}>
                <Text elementType="p" styleType="metaInfo">Meta info text</Text>
            </Row>
            <Row style={rowStyle}>
                <Text elementType="p" isDisabled>Disabled</Text>
            </Row>
        </Container>
    ));