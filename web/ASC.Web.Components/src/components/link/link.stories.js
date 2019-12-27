import React from 'react';
import { storiesOf } from '@storybook/react';
import Link from '.';
import Readme from './README.md';
import { text, boolean, withKnobs, select, color } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Section from '../../../.storybook/decorators/section';
import { action } from '@storybook/addon-actions';

const type = ['action', 'page'];
const target = ['_blank', '_self', '_parent', '_top'];

function clickActionLink(e) {
  console.log("clickActionLink", e);
  action('actionClick')(e);
}

storiesOf('Components|Link', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => {

    const href = text('href', "http://github.com");

    const actionProps = (href && href.length > 0) ? { href } : { onClick: clickActionLink };

    const label = text('text', 'Simple link');

    const isTextOverflow=boolean('isTextOverflow', false);

   return (
   <Section>
        <Link
          type={select('type', type, 'page')}
          color={color('color', '#333333')}
          fontSize={text('fontSize', '13px')}
          fontWeight={text('fontWeight', '400')}
          isBold={boolean('isBold', false)}
          title={text('title', undefined)}
          target={select('target', target, '_blank')}
          isTextOverflow={isTextOverflow}
          isHovered={boolean('isHovered', false)}
          noHover={boolean('noHover', false)}
          isSemitransparent={boolean('isSemitransparent', false)}
          {...actionProps}
        >
          {label}
        </Link>
    </Section>
   )}
  );
