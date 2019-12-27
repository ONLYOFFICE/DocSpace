import React from 'react';
import { storiesOf } from '@storybook/react';
import LinkWithDropdown from '.';
import Readme from './README.md';
import { text, boolean, withKnobs, select, color } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Section from '../../../.storybook/decorators/section';

const dropdownType = ["alwaysDashed", "appearDashedAfterHover"];

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
      <div>
        <LinkWithDropdown
          dropdownType={select('dropdownType', dropdownType, 'alwaysDashed')}
          color={color('color', '#333333')}
          fontSize={text('fontSize', '13px')}
          fontWeight={text('fontWeight', '400')}
          isBold={boolean('isBold', false)}
          title={text('title', undefined)}
          isTextOverflow={boolean('isTextOverflow', false)}
          isSemitransparent={boolean('isSemitransparent', false)}
          data={dropdownItems}
        >
          {text('text', 'Simple link with dropdown')}
        </LinkWithDropdown>
      </div>
    </Section>
  )
  );
