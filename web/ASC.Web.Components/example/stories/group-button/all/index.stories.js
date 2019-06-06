import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { Container, Row, Col } from 'reactstrap';
import { GroupButton } from 'asc-web-components'

const rowStyle = { marginTop: 8 };

storiesOf('Components|GroupButton', module)
    .addDecorator(withReadme(Readme))
    .addParameters({ options: { showAddonPanel: false }})
    .add('all', () => (
        <Container>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col>Active</Col>
                <Col>Hover</Col>
                <Col>Click*(Press)</Col>
                <Col>Disable</Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1">Default</Col>
                <Col><GroupButton text='' isCheckbox /></Col>
                <Col><GroupButton text='' isCheckbox hovered /></Col>
                <Col><GroupButton text='' isCheckbox activated/></Col>
                <Col><GroupButton text='' isCheckbox disabled /></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton text='' isCheckbox isDropdown ><GroupButton /></GroupButton></Col>
                <Col><GroupButton text='' isCheckbox isDropdown hovered ><GroupButton /></GroupButton></Col>
                <Col><GroupButton text='' isCheckbox isDropdown activated ><GroupButton /></GroupButton></Col>
                <Col><GroupButton text='' isCheckbox isDropdown disabled ><GroupButton /></GroupButton></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton text='' isCheckbox isDropdown splitted ><GroupButton /></GroupButton></Col>
                <Col><GroupButton text='' isCheckbox isDropdown splitted hovered ><GroupButton /></GroupButton></Col>
                <Col><GroupButton text='' isCheckbox isDropdown splitted activated ><GroupButton /></GroupButton></Col>
                <Col><GroupButton text='' isCheckbox isDropdown splitted disabled ><GroupButton /></GroupButton></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton /></Col>
                <Col><GroupButton hovered /></Col>
                <Col><GroupButton activated /></Col>
                <Col><GroupButton disabled /></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton isDropdown ><GroupButton /></GroupButton></Col>
                <Col><GroupButton isDropdown hovered ><GroupButton /></GroupButton></Col>
                <Col><GroupButton isDropdown activated ><GroupButton /></GroupButton></Col>
                <Col><GroupButton isDropdown disabled ><GroupButton /></GroupButton></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton isDropdown splitted ><GroupButton /></GroupButton></Col>
                <Col><GroupButton isDropdown splitted hovered ><GroupButton /></GroupButton></Col>
                <Col><GroupButton isDropdown splitted activated ><GroupButton /></GroupButton></Col>
                <Col><GroupButton isDropdown splitted disabled ><GroupButton /></GroupButton></Col>
            </Row>
            <Row style={rowStyle}>
                <Col></Col>
                <Col></Col>
                <Col></Col>
                <Col></Col>
                <Col></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1">Primary</Col>
                <Col><GroupButton primary text='' isCheckbox /></Col>
                <Col><GroupButton primary text='' isCheckbox hovered /></Col>
                <Col><GroupButton primary text='' isCheckbox activated /></Col>
                <Col><GroupButton primary text='' isCheckbox disabled /></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton primary text='' isCheckbox isDropdown ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary text='' isCheckbox isDropdown hovered ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary text='' isCheckbox isDropdown activated ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary text='' isCheckbox isDropdown disabled ><GroupButton /></GroupButton></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton primary text='' isCheckbox isDropdown splitted ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary text='' isCheckbox isDropdown splitted hovered ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary text='' isCheckbox isDropdown splitted activated ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary text='' isCheckbox isDropdown splitted disabled ><GroupButton /></GroupButton></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton primary /></Col>
                <Col><GroupButton primary hovered /></Col>
                <Col><GroupButton primary activated /></Col>
                <Col><GroupButton primary disabled /></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton primary isDropdown ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary isDropdown hovered ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary isDropdown activated ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary isDropdown disabled ><GroupButton /></GroupButton></Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="1"></Col>
                <Col><GroupButton primary isDropdown splitted ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary isDropdown splitted hovered ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary isDropdown splitted activated ><GroupButton /></GroupButton></Col>
                <Col><GroupButton primary isDropdown splitted disabled ><GroupButton /></GroupButton></Col>
            </Row>
        </Container>
    ));