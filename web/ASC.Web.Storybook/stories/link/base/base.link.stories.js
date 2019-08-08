import React from 'react';
import { storiesOf } from '@storybook/react';
import { Link } from 'asc-web-components';
import Readme from './README.md';
import { text, boolean, withKnobs, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Section from '../../../.storybook/decorators/section';
import { Col } from 'reactstrap';
import { action } from '@storybook/addon-actions';

const type = ['action', 'page'];
const colors = ['black', 'gray', 'blue'];
const target = ['_blank', '_self', '_parent', '_top'];

function clickActionLink(e) {
  action('actionClick')(e);
}

storiesOf('Components|Link', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Section>
      <Col>
        <Link
          type={select('type', type, 'page')}
          color={select('color', colors, 'black')}
          fontSize={number('fontSize', 13)}
          href={text('href', undefined)}
          isBold={boolean('isBold', false)}
          title={text('title', undefined)}
          target={select('target', target, '_top')}
          isTextOverflow={boolean('isTextOverflow', false)}
          isHovered={boolean('isHovered', false)}
          isSemitransparent={boolean('isSemitransparent', false)}
          onClick={clickActionLink}
        >
          {text('text', 'Simple link')}
        </Link>
      </Col>
    </Section>
  )
  );
