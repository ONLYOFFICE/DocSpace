import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { Container, Row, Col } from 'reactstrap';
import { GroupButton, DropDownItem } from 'asc-web-components'

const rowStyle = { marginTop: 8 };

storiesOf('Components|GroupButton', module)
    .addDecorator(withReadme(Readme))
    .addParameters({ options: { showAddonPanel: false }})
    .add('all', () => (
        <Container>
            <Row style={rowStyle}>
                <Col>Active</Col>
                <Col>Hover</Col>
                <Col>Click*(Press)</Col>
                <Col>Disable</Col>
            </Row>
            <Row style={rowStyle}>
                <Col><GroupButton /></Col>
                <Col><GroupButton hovered /></Col>
                <Col><GroupButton activated/></Col>
                <Col><GroupButton disabled /></Col>
            </Row>
            <Row style={rowStyle}>
                <Col><GroupButton isDropdown ><DropDownItem /></GroupButton></Col>
                <Col><GroupButton isDropdown hovered ><DropDownItem /></GroupButton></Col>
                <Col><GroupButton isDropdown activated ><DropDownItem /></GroupButton></Col>
                <Col><GroupButton isDropdown disabled ><DropDownItem /></GroupButton></Col>
            </Row>
        </Container>
    ));