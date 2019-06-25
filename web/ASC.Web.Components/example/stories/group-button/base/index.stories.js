import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, boolean, text } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { GroupButton, DropDownItem } from 'asc-web-components'

storiesOf('Components|GroupButton', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => (
        <GroupButton
            label = {text('Label', 'Base group button')}
            disabled = {boolean('disabled', false)}
            isDropdown = {boolean('isDropdown', false)}
            opened = {boolean('opened', false)}
        >
            <DropDownItem/>
        </GroupButton>
    ));
