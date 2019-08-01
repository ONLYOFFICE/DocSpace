import React from 'react';
import { storiesOf } from '@storybook/react';
import { Link } from 'asc-web-components';
import Readme from './README.md';
import {text, boolean, withKnobs, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Section from '../../../.storybook/decorators/section';
import {Col} from 'reactstrap';
import { action } from '@storybook/addon-actions';

const type = ['action', 'page'];
const colors = ['black', 'gray', 'blue'];
const target = ['_blank', '_self', '_parent', '_top'];
const dropdownType = ['alwaysDotted', 'appearDottedAfterHover', 'none'];


function clickActionLink(e) {
  action('actionClick')(e);
}

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
    <Col>
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
      data={dropdownItems}
      {...userProps}
      >
      {text('text', 'Simple link')}
      </Link>
    </Col>
  </Section>
);
});
