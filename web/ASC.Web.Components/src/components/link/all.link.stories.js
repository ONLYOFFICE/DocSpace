import React from 'react';
import { storiesOf } from '@storybook/react';
import Link from '.';
import { Container, Row, Col } from 'reactstrap';

const rowStyle = {
  marginTop: 8,
  paddingLeft: 20,
  fontSize: 12
};

const headerStyle = {
  marginTop: 8,
  paddingLeft: 20,
  fontSize: 16,
};


storiesOf('Components|Link', module)
  .addParameters({ viewport: { defaultViewport: 'responsive' } })
  .addParameters({ options: { showAddonPanel: false } })
  .add('all', () => (
    <>
      <Container fluid>
        <Row style={headerStyle}>
          <Col>Page links:</Col>
          <Col>Action links:</Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" href="https://github.com" isBold={true}>Bold black page link</Link>
          </Col>
          <Col>
            <Link type="action" isBold={true}>Bold black action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" href="https://github.com">Black page link</Link>
          </Col>
          <Col>
            <Link type="action">Black action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" href="https://github.com" isHovered={true}>Black hovered page link</Link>
          </Col>
          <Col>
            <Link type="action" isHovered={true}>Black hovered action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" href="https://github.com" isSemitransparent={true}>Semitransparent black page link</Link>
          </Col>
          <Col>
            <Link type="action" isSemitransparent={true}>Semitransparent black action link</Link>
          </Col>
        </Row>
      </Container>
    </>
  ));
