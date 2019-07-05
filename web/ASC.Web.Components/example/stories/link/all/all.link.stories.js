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
            <Col>Another using of action links:</Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "black" href="https://github.com" isBold = {true} text = 'Bold black page link' />
          </Col>
          <Col>
            <Link type = "action" color = "black" isBold = {true} text = 'Bold black action link' />
          </Col>
          <Col><Link type = "action" color = "black" dropdownType = 'alwaysDotted' text = 'Simple dropdown' /></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "black" href="https://github.com" text = 'Black page link' />
          </Col>
          <Col>
            <Link type = "action" color = "black" text = 'Black action link' />
          </Col>
          <Col> <Link type = "action" color = "gray" dropdownType = 'appearDottedAfterHover' text = 'Gray dropdown and dotted appear after hover' /></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "black" href="https://github.com" isHovered = {true} text = 'Black hovered page link' />
          </Col>
          <Col>
            <Link type = "action" color = "black" isHovered = {true} text = 'Black hovered action link' />
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "gray" href="https://github.com" text = 'Gray page link' />
          </Col>
          <Col>
            <Link type = "action" color = "gray" text = 'Gray action link' />
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "gray" href="https://github.com" isHovered = {true} text = 'Gray hovered page link' />
          </Col>
          <Col>
            <Link type = "action" color = "gray" isHovered = {true} text = 'Gray hovered action link' />
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "blue" href="https://github.com" text = 'Blue page link' />
          </Col>
          <Col>
            <Link type = "action" color = "blue" text = 'Blue action link' />
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "blue" href="https://github.com" isHovered = {true} text = 'Blue hovered page link' />
          </Col>
          <Col>
            <Link type = "action" color = "blue" isHovered = {true} text = 'Blue hovered action link' />
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "black" href="https://github.com" isSemitransparent = {true} text = 'Semitransparent black page link' />
          </Col>
          <Col>
            <Link type = "action" color = "black" isSemitransparent = {true} text = 'Semitransparent black action link' />
          </Col>
          <Col></Col>
      </Row>  
    </Container>
   </>
  ));