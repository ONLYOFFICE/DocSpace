import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, boolean, number, text, color } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Section from '../../.storybook/decorators/section';
import { ContextMenuButton } from 'asc-web-components';


function getData() {
  console.log('getData');
  return [
    {key: 'key1', label: 'label1', onClick: () => console.log('label1')},
    {key: 'key2', label: 'label2', onClick: () => console.log('label2')}
  ]
}

storiesOf('Components|ContextMenuButton', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('context-menu-button', () => (
    <Section>
      <ContextMenuButton getData={getData} />
    </Section>
  ));