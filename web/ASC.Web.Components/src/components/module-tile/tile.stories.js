import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { Container, Row, Col } from 'reactstrap';
import ModuleTile from '../module-tile';
import withReadme from 'storybook-readme/with-readme';
import { withKnobs, boolean, text } from '@storybook/addon-knobs/react';
import Readme from './README.md';

const rowStyle = { marginTop: 8 };

storiesOf('Components|ModuleTile', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => (
        <Container>
            <Row style={rowStyle}>
                <Col>
                    <ModuleTile
                        title={text("title", "Documents")}
                        imageUrl="./modules/documents240.png"
                        link="/products/files/"
                        description={text("description", "Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed.")}
                        isPrimary={boolean("isPrimary", true)}
                        onClick={action("onClick")}
                    />
                </Col>
            </Row>
        </Container>
    ));
