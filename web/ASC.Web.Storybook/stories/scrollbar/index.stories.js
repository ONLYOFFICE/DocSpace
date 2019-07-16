import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import { withKnobs, select } from '@storybook/addon-knobs/react';
import Readme from './README.md';
import Section from '../../.storybook/decorators/section';
import { Scrollbar } from 'asc-web-components';

const stypes = ["smallWhite", "smallBlack", "mediumBlack"];

storiesOf('Components|Scrollbar', module)
  .addDecorator(withReadme(Readme))
  .addDecorator(withKnobs)
  .add('Scrollbar', () => (
    <Section>
        <Scrollbar
            stype={select('stype', stypes, 'smallBlack')}
            style={{ width: 100, height: 100, background: 'green' }}>
                ================================================================
                Lorem ipsum dolor sit amet, consectetur adipiscing elit,
                sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
                Ut enim ad minim veniam, quis nostrud exercitation ullamco 
                laboris nisi ut aliquip ex ea commodo consequat.
                Duis aute irure dolor in reprehenderit in voluptate velit esse cillum
                dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident,
                sunt in culpa qui officia deserunt mollit anim id est laborum.
                ================================================================
        </Scrollbar>
    </Section>
  ));