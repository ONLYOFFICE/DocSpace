import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { Container, Row, Col } from 'reactstrap';
import { DropDown, DropDownItem } from 'asc-web-components'

storiesOf('Components | DropDown | DropDownItem', module)
    .addDecorator(withReadme(Readme))
    .add('base item', () => (
        <Container fluid={true}>
            <Row>
                <Col xs='1'></Col>
                <Col xs="4">Only dropdown</Col>
                <Col xs='1'></Col>
                <Col xs="4">Profile preview dropdown</Col>
            </Row>
            <Row>
                <Col xs="1"></Col>
                <Col xs="4">
                    <DropDown opened={true}>
                        <DropDownItem label='Button 1' onClick={() => console.log('Button 1 clicked')} />
                        <DropDownItem label='Button 2' onClick={() => console.log('Button 2 clicked')} />
                        <DropDownItem label='Button 3' onClick={() => console.log('Button 3 clicked')} />
                        <DropDownItem label='Button 4' onClick={() => console.log('Button 4 clicked')} />
                        <DropDownItem isSeparator />
                        <DropDownItem label='Button 5' onClick={() => console.log('Button 5 clicked')} />
                        <DropDownItem label='Button 6' onClick={() => console.log('Button 6 clicked')} />
                    </DropDown>
                </Col>
                <Col xs="1"></Col>
                <Col xs="4">
                    <DropDown isUserPreview withArrow direction='right' opened={true}>
                        <DropDownItem isUserPreview role='admin' source='https://static-www.onlyoffice.com/images/team/developers_photos/personal_44_2x.jpg' userName='Jane Doe' label='janedoe@gmail.com'/>
                        <DropDownItem label='Profile' onClick={() => console.log('Profile clicked')} />
                        <DropDownItem label='About this program' onClick={() => console.log('About this program clicked')} />
                        <DropDownItem label='Log out' onClick={() => console.log('Log out clicked')} />
                    </DropDown>
                </Col>
            </Row>
        </Container>
    )
);