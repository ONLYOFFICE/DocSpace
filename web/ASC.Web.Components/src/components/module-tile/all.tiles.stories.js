import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { Container, Row, Col } from 'reactstrap';
import ModuleTile from '../module-tile';

const rowStyle = { marginTop: 8 };

storiesOf('Components|ModuleTile', module)
    // To set a default viewport for all the stories for this component
    .addParameters({ viewport: { defaultViewport: 'responsive' } })
    .addParameters({ options: { showAddonPanel: false }})
    .add('all', () => (
        <Container>
            <Row style={rowStyle}>
                <Col>
                    <ModuleTile
                        title="Documents"
                        imageUrl="./modules/documents240.png"
                        link="/products/files/"
                        description="Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed."
                        isPrimary={true}
                        onClick={action("onClick")}
                    />
                </Col>
            </Row>
            <Row style={rowStyle}>
                <Col>
                    <ModuleTile
                        title="Projects"
                        imageUrl="./modules/projects_logolarge.png"
                        link="products/projects/"
                        isPrimary={false}
                        onClick={action("onClick")}
                    />
                </Col>
                <Col>
                    <ModuleTile
                        title="Crm"
                        imageUrl="./modules/crm_logolarge.png"
                        link="/products/crm/"
                        isPrimary={false}
                        onClick={action("onClick")}
                    />
                </Col>
                <Col>
                    <ModuleTile
                        title="Mail"
                        imageUrl="./modules/mail_logolarge.png"
                        link="/products/mail/"
                        isPrimary={false}
                        onClick={action("onClick")}
                    />
                </Col>
                <Col>
                    <ModuleTile
                        title="People"
                        imageUrl="./modules/people_logolarge.png"
                        link="/products/people/"
                        isPrimary={false}
                        onClick={action("onClick")}
                    />
                </Col>
                <Col>
                    <ModuleTile
                        title="Community"
                        imageUrl="./modules/community_logolarge.png"
                        link="products/community/"
                        isPrimary={false}
                        onClick={action("onClick")}
                    />
                </Col>
            </Row>
        </Container>
    ));
