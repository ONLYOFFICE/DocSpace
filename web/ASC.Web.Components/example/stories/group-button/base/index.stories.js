import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, boolean, text } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { GroupButton } from 'asc-web-components'

storiesOf('Components|GroupButton', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => (
        <GroupButton
            text = {text('Label', 'Base group button')}
            primary = {boolean('primary', false)}
            disabled = {boolean('disabled', false)}
            isCheckbox = {boolean('isCheckbox', false)}
            isDropdown = {boolean('isDropdown', false)}
            opened = {boolean('opened', false)}
            splitted = {boolean('splitted', false)}
        >
            <GroupButton/>
        </GroupButton>
    ));
