import React from 'react';
import { storiesOf } from '@storybook/react';
import { Link } from 'asc-web-components';
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
            <Link type="page" color="black" href="https://github.com" isBold={true}>Bold black page link</Link>
          </Col>
          <Col>
            <Link type="action" color="black" isBold={true}>Bold black action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" color="black" href="https://github.com">Black page link</Link>
          </Col>
          <Col>
            <Link type="action" color="black">Black action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" color="black" href="https://github.com" isHovered={true}>Black hovered page link</Link>
          </Col>
          <Col>
            <Link type="action" color="black" isHovered={true}>Black hovered action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" color="gray" href="https://github.com">Gray page link</Link>
          </Col>
          <Col>
            <Link type="action" color="gray">Gray action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" color="gray" href="https://github.com" isHovered={true}>Gray hovered page link</Link>
          </Col>
          <Col>
            <Link type="action" color="gray" isHovered={true}>Gray hovered action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" color="blue" href="https://github.com">Blue page link</Link>
          </Col>
          <Col>
            <Link type="action" color="blue">Blue action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" color="blue" href="https://github.com" isHovered={true}>Blue hovered page link</Link>
          </Col>
          <Col>
            <Link type="action" color="blue" isHovered={true}>Blue hovered action link</Link>
          </Col>
        </Row>
        <Row style={rowStyle}>
          <Col>
            <Link type="page" color="black" href="https://github.com" isSemitransparent={true}>Semitransparent black page link</Link>
          </Col>
          <Col>
            <Link type="action" color="black" isSemitransparent={true}>Semitransparent black action link</Link>
          </Col>
        </Row>
      </Container>
    </>
  ));
