import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { Container, Row, Col } from 'reactstrap';
import { DropDown, DropDownItem } from 'asc-web-components'

storiesOf('Components | DropDown | DropDownItem', module)
    .addDecorator(withReadme(Readme))
    .add('base item', () => (
        <Container>
            <Row>
                <Col xs="2">Only dropdown</Col>
            </Row>
            <Row>
                <Col xs="2">
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
            </Row>
        </Container>
    )
);