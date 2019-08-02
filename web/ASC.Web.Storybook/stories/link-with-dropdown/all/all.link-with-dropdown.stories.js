import React from 'react';
import { storiesOf } from '@storybook/react';
import { LinkWithDropdown } from 'asc-web-components';
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

const dropdownType = ['alwaysDotted', 'appearDottedAfterHover'];
const colors = ['black', 'gray', 'blue'];

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
        {colors.map(color =>
          <Row key={color} style={rowStyle}>
            {dropdownType.map(linkType =>
              <Col key={linkType}>
                <LinkWithDropdown color={color} isBold={true} dropdownType={linkType} data={data}>Bold {color} {linkType}</LinkWithDropdown>
              </Col>
            )}
          </Row>
        )}
        {colors.map(color =>
          <Row key={color} style={rowStyle}>
            {dropdownType.map(linkType =>
              <Col key={linkType}>
                <LinkWithDropdown color={color} dropdownType={linkType} data={data}>{color} {linkType}</LinkWithDropdown>
              </Col>
            )}
          </Row>
        )}
        {colors.map(color =>
          <Row key={color} style={rowStyle}>
            {dropdownType.map(linkType =>
              <Col key={linkType}>
                <LinkWithDropdown color={color} isSemitransparent={true} dropdownType={linkType} data={data}>Semitransparent {color} {linkType}</LinkWithDropdown>
              </Col>
            )}
          </Row>
        )}
      </Container>
    </>
  ));
