import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, boolean} from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Section from '../../../.storybook/decorators/section';
import { ComboBox } from 'asc-web-components'

let options = [
    {
        key: '0',
        label: '25 per page',
        onClick: (e) => console.log(e.target)
    },
    {
        key: '1',
        label: '50 per page',
        onClick: (e) => console.log(e.target)
    },
    {
        key: '2',
        label: '100 per page',
        onClick: (e) => console.log(e.target)
    }
];

storiesOf('Components|Input', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('combo box', () => (
        <Section>
            <ComboBox 
                options={options}
                isDisabled={boolean('isDisabled', false)}
                withBorder={boolean('withBorder', true)}
            />
        </Section>
    ));