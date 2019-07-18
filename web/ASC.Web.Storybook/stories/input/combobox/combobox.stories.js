import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, boolean} from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Section from '../../../.storybook/decorators/section';
import { ComboBox } from 'asc-web-components'

let items = [
    {
        label: '25 per page',
        onClick: () => console.log('set paging 25 action')
    },
    {
        label: '50 per page',
        onClick: () => console.log('set paging 50 action')
    },
    {
        label: '100 per page',
        onClick: () => console.log('set paging 100 action')
    }
];

storiesOf('Components|Input', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('combo box', () => (
        <Section>
            <ComboBox 
                items={items}
                isDisabled={boolean('isDisabled', false)}
            />
        </Section>
    ));