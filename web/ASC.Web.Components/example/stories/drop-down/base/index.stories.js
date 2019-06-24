import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { Container, Row, Col } from 'reactstrap';
import { GroupButton, DropDown, DropDownItem} from 'asc-web-components'

const rowStyle = { marginTop: 8 };

storiesOf('Components| DropDown', module)
    .addDecorator(withReadme(Readme))
    .add('base', () => (
        <Container>
            <Row style={rowStyle}>
                <Col xs="4">Only dropdown</Col>
                <Col xs="2"/>
                <Col>With Button</Col>
            </Row>
            <Row style={rowStyle}>
                <Col xs="4">
                    Without active button
                    <DropDown opened={true}>
                        <DropDownItem
                            label='Button 1'
                            onClick={() => console.log('Button 1 clicked')}
                        />
                        <DropDownItem
                            label='Button 2'
                            onClick={() => console.log('Button 2 clicked')}
                        />
                        <DropDownItem
                            label='Button 3'
                            onClick={() => console.log('Button 3 clicked')}
                        />
                        <DropDownItem
                            label='Button 4'
                            onClick={() => console.log('Button 4 clicked')}
                        />
                        <DropDownItem isSeparator />
                        <DropDownItem
                            label='Button 5'
                            onClick={() => console.log('Button 5 clicked')}
                        />
                        <DropDownItem
                            label='Button 6'
                            onClick={() => console.log('Button 6 clicked')}
                        />
                    </DropDown>
                </Col>
                <Col xs="2"/>
                <Col>
                    <GroupButton text='Dropdown demo' isDropdown={true}>
                        <DropDownItem
                            label='Button 1'
                        />
                        <DropDownItem
                            label='Button 2'
                        />
                        <DropDownItem
                            label='Button 3'
                        />
                    </GroupButton>
                </Col>
            </Row>
        </Container>
    )
);