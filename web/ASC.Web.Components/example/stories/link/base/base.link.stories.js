import React from 'react';
import { storiesOf } from '@storybook/react';
import { Link, DropDown } from 'asc-web-components';
import Readme from './README.md';
import {text, boolean, withKnobs, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Section from '../../../.storybook/decorators/section';

const type = ['action', 'page'];
const colors = ['black', 'gray', 'blue'];
const target = ['_blank', '_self', '_parent', '_top'];
const dropdownType = ['alwaysDotted', 'appearDottedAfterHover', 'none'];


function clickActionLink(e) {
  console.log('Clicked action link', e);
}

storiesOf('Components|Link', module)
.addDecorator(withKnobs)
.addDecorator(withReadme(Readme))
.add('base', () => {
let linkType=`${select('type', type, 'page')}`;
const userProps = linkType === "action" ? {
  dropdownType: `${select('dropdownType', dropdownType, 'none')}`,
  onClick: clickActionLink
  } : {};
return (
  <Section>
    <Link
      type={linkType}
      color={select('color', colors, 'black')}
      fontSize={number('fontSize', 12)}
      href={text('href', undefined)}
      isBold={boolean('isBold', false)}
      title={text('title', undefined )}
      target={select('target', target, '_top')}
      isTextOverflow={boolean('isTextOverflow', false)}
      isHovered={boolean('isHovered', false)}
      isSemitransparent={boolean('isSemitransparent', false)}
      text={text('text', 'Simple link')}
      {...userProps}
    />
  </Section>
);
});

