import React from 'react';
import { storiesOf } from '@storybook/react';
import { LinkWithDropdown } from 'asc-web-components';
import Readme from './README.md';
import { text, boolean, withKnobs, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Section from '../../../.storybook/decorators/section';
import { Col } from 'reactstrap';

const colors = ['black', 'gray', 'blue'];
const dropdownType = ['alwaysDotted', 'appearDottedAfterHover'];

const dropdownItems = [
  {
    key: 'key1',
    label: 'Button 1',
    onClick: () => console.log('Button1 action'),
  },
  {
    key: 'key2',
    label: 'Button 2',
    onClick: () => console.log('Button2 action'),
  },
  {
    key: 'key3',
    isSeparator: true
  },
  {
    key: 'key4',
    label: 'Button 3',
    onClick: () => console.log('Button3 action'),
  },
];

storiesOf('Components|LinkWithDropdown', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Section>
      <Col>
        <LinkWithDropdown
          dropdownType={select('dropdownType', dropdownType, 'alwaysDotted')}
          color={select('color', colors, 'black')}
          fontSize={number('fontSize', 13)}
          isBold={boolean('isBold', false)}
          title={text('title', undefined)}
          isTextOverflow={boolean('isTextOverflow', false)}
          isSemitransparent={boolean('isSemitransparent', false)}
          data={dropdownItems}
        >
          {text('text', 'Simple link with dropdown')}
        </LinkWithDropdown>
      </Col>
    </Section>
  )
  );
