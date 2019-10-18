import React from 'react';
import { storiesOf } from '@storybook/react';
import LinkWithDropdown from '.';
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

const data = [
  { key: 'key1', label: 'Base button1', onClick: () => console.log('Base button1 clicked') },
  { key: 'key2', label: 'Base button2', onClick: () => console.log('Base button2 clicked') },
  { key: 'key3', isSeparator: true },
  { key: 'key4', label: 'Base button3', onClick: () => console.log('Base button3 clicked') }
];

const dropdownType = ['alwaysDashed', 'appearDashedAfterHover'];

storiesOf('Components|LinkWithDropdown', module)
  .addParameters({ viewport: { defaultViewport: 'responsive' } })
  .addParameters({ options: { showAddonPanel: false } })
  .add('all', () => (
    <>
      <Container fluid>
        <Row style={headerStyle}>
          {dropdownType.map(linkType =>
            <Col key={linkType}>type - {linkType}:</Col>
          )}
        </Row>

        <Row style={rowStyle}>
          {dropdownType.map(linkType =>
            <Col key={linkType}>
              <LinkWithDropdown isBold={true} dropdownType={linkType} data={data}>Bold {linkType}</LinkWithDropdown>
            </Col>
          )}
        </Row>


        <Row style={rowStyle}>
          {dropdownType.map(linkType =>
            <Col key={linkType}>
              <LinkWithDropdown dropdownType={linkType} data={data}> {linkType}</LinkWithDropdown>
            </Col>
          )}
        </Row>


        <Row style={rowStyle}>
          {dropdownType.map(linkType =>
            <Col key={linkType}>
              <LinkWithDropdown isSemitransparent={true} dropdownType={linkType} data={data}>Semitransparent  {linkType}</LinkWithDropdown>
            </Col>
          )}
        </Row>

      </Container>
    </>
  ));
