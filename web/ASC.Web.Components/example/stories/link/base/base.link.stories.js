import React from 'react';
import { storiesOf } from '@storybook/react';
import { Link, DropDown } from 'asc-web-components';
import Readme from './README.md';
import {text, boolean, withKnobs, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Section from '../../../.storybook/decorators/section';

const type = ['action', 'page'];
const colors = ['black', 'gray', 'blue', 'filter', 'profile'];
const target = ['_blank', '_self', '_parent', '_top'];
const dropdownType = ['filter', 'menu', 'none'];
const dropdownColor = ['filter', 'profile', 'sorting','number','email', 'group'];

storiesOf('Components|Link', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Section>
      <Link 
        type={select('type', type, 'page')}
        color={select('color', colors, 'black')}
        fontSize={text('fontSize', '12px')}
        href={text('href', 'https://github.com')}
        isBold={boolean('isBold', false)}
        title={text('title', '')}
        target={select('target', target, '_top')}
        rel={text('rel', '')}
        isTextOverflow={boolean('isTextOverflow', false)}
        isHovered={boolean('isHovered', false)}
        isDotted={boolean('isDotted', false)}
        isHoverDotted={boolean('isHoverDotted', false)}
        isDropdown={boolean('isDropdown', false)}
        dropdownType={select('dropdownType', dropdownType, 'none')}
        dropdownColor={select('dropdownColor', dropdownColor, 'filter')}
        dropdownRightIndent={text('dropdownRightIndent', '-10px')}
        displayDropdownAfterHover={boolean('displayDropdownAfterHover', false)}
        >
          Simple link
      </Link>
    </Section>
  ));
