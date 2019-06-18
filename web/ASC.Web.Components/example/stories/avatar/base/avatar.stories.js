import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, boolean, text, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Avatar } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

const roleOptions = ['owner', 'admin','guest','user'];
const sizeOptions = ['retina', 'max', 'big', 'medium', 'small'];

storiesOf('Components|Avatar', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Section>
        <Avatar
            size={select('size', sizeOptions, 'max')}
            role={select('role', roleOptions, 'admin')}
            source={text('source', '')}
            pending={boolean('pending', false)}
            disabled={boolean('disabled', false)}
        />
    </Section>
  ));