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
            <Link type = "page" color = "black" href="https://github.com" isBold = {true}>Bold black page link</Link>
          </Col>
          <Col>
            <Link type = "action" color = "black" isBold = {true}>Bold black action link</Link>
          </Col>
          <Col>Sorting:&nbsp;<Link type = "action" color = "black" isDotted = {true} isDropdown = {true} dropdownColor = 'sorting' isHoverDotted = {true}>Last name</Link></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "black" href="https://github.com">Black page link</Link>
          </Col>
          <Col>
            <Link type = "action" color = "black">Black action link</Link>
          </Col>
          <Col> <Link type = "action" color = "profile" isDropdown = {true} dropdownColor = 'profile'>My profile color(deprecated)</Link></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "black" href="https://github.com" isHovered = {true}>Black hovered page link</Link>
          </Col>
          <Col>
            <Link type = "action" color = "black" isHovered = {true}>Black hovered action link</Link>
          </Col>
          <Col> Show on page:&nbsp;<Link type = "action" color = "black" isDotted = {true} isDropdown = {true} dropdownColor = 'number' isHoverDotted = {true}>25</Link></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "gray" href="https://github.com">Gray page link</Link>
          </Col>
          <Col>
            <Link type = "action" color = "gray">Gray action link</Link>
          </Col>
          <Col><Link type = "action" color = "black" isDropdown = {true} dropdownColor = 'email' displayDropdownAfterHover = {true} isHoverDotted = {true}>Dropdown appear after hover</Link></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "gray" href="https://github.com" isHovered = {true}>Gray hovered page link</Link>
          </Col>
          <Col>
            <Link type = "action" color = "gray" isHovered = {true}>Gray hovered action link</Link>
          </Col>
          <Col><Link type = "action" color = "filter" isDropdown = {true} dropdownColor = 'filter' isDropdown = {true} isDotted = {true} isHoverDotted = {true}>Some group filter color(deprecated)</Link></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "blue" href="https://github.com">Blue page link</Link>
          </Col>
          <Col>
            <Link type = "action" color = "blue">Blue action link</Link>
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col>
            <Link type = "page" color = "blue" href="https://github.com" isHovered = {true}>Blue hovered page link</Link>
          </Col>
          <Col>
            <Link type = "action" color = "blue" isHovered = {true}>Blue hovered action link</Link>
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col></Col>
          <Col>
            <Link type = "action" color = "black" isDotted = {true}>Black dotted action link</Link>
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col></Col>
          <Col>
            <Link type = "action" color = "black" isHoverDotted = {true}>Black action link, dotted after hover</Link>
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col></Col>
          <Col>
            <Link type = "action" color = "gray" isDotted = {true}>Gray dotted action link</Link>
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col></Col>
          <Col>
            <Link type = "action" color = "gray" isHoverDotted = {true}>Gray action link, dotted after hover</Link>
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col></Col>
          <Col>
            <Link type = "action" color = "blue" isDotted = {true}>Blue dotted action link</Link>
          </Col>
          <Col></Col>
      </Row>
      <Row style={rowStyle}>
          <Col></Col>
          <Col>
            <Link type = "action" color = "blue" isHoverDotted = {true}>Blue action link, dotted after hover</Link>
          </Col>
          <Col></Col>
      </Row>
      
    </Container>
   </>
  ));