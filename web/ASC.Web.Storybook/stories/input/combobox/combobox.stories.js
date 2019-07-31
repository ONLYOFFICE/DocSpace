import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, boolean} from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Section from '../../../.storybook/decorators/section';
import { ComboBox } from 'asc-web-components'
import { action } from '@storybook/addon-actions';

const options = [
    {
        key: '0',
        label: '25 per page'
    },
    {
        key: '1',
        label: '50 per page',
    },
    {
        key: '2',
        label: '100 per page'
    }
];

storiesOf('Components|Input', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('combo box', () => (
        <Section>
            <ComboBox 
                options={options}
                onSelect={option => action("Selected option")(option)}
                isDisabled={boolean('isDisabled', false)}
                withBorder={boolean('withBorder', true)}
            />
        </Section>
    ));