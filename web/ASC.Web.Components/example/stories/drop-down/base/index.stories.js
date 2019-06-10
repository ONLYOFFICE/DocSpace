import React from 'react'
import { action } from '@storybook/addon-actions';
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { Container, Row, Col } from 'reactstrap';
import { GroupButton, Button, DropDown } from 'asc-web-components'

const rowStyle = { marginTop: 8 };

storiesOf('Components| DropDown', module)
    .addDecorator(withReadme(Readme))
    .add('base', () => (
        <Container>
            <Row style={rowStyle}>
                <Col>Only dropdown</Col>
                <Col>With Button</Col>
            </Row>
            <Row style={rowStyle}>
                <Col>
                    Used for demonstration without active button
                    <DropDown opened={true}>
                        <GroupButton text='Button 1' />
                        <GroupButton text='Button 2' />
                        <GroupButton text='Button 3' />
                        <GroupButton text='Button 4' />
                        <GroupButton text='Button 5' />
                        <GroupButton text='Button 6' />
                        <GroupButton text='Button 7' />
                        <GroupButton text='Button 8' />
                    </DropDown>
                </Col>
                <Col>
                    <GroupButton text='Dropdown demo' isDropdown={true}>
                        <GroupButton 
                            text='Group button' 
                            clickAction={() => console.log('Group button clicked')} 
                        />
                        <Button 
                            label='Base button' 
                            size='base' 
                            onClick={action('Base button clicked')} 
                        />
                    </GroupButton>
                </Col>
            </Row>
        </Container>
    )
);