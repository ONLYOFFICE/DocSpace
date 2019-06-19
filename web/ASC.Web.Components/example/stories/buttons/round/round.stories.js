import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { RoundButton } from 'asc-web-components'
import Section from '../../../.storybook/decorators/section';

const data = [
    { key: 'key_1', text: 'Action 1' },
    { key: 'key_2', text: 'Action 2' },
    { key: 'key_3', text: 'Action 3' }
];

storiesOf('Components|Buttons', module)
    .addDecorator(withReadme(Readme))
    .add('round', () => (
        <Section>
            <RoundButton title="Actions" data={data}></RoundButton>
        </Section>
    )
);