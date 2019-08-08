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
        key: 1,
        icon: 'CatalogEmployeeIcon',
        label: 'Option 1'
    },
    {
        key: 2,
        icon: 'CatalogGuestIcon',
        label: 'Option 2',
    },
    {
        key: 3,
        label: 'Option 3'
    },
    {
        key: 4,
        label: 'Option 4'
    },
    {
        key: 5,
        icon: 'CopyIcon',
        label: 'Option 5'
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