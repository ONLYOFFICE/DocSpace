import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { withKnobs, text } from '@storybook/addon-knobs/react';
import { Text, ErrorContainer } from 'asc-web-components';

storiesOf('Components| ErrorContainer', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => (
        <ErrorContainer>
            <Text.Headline as="h2" size='medium' isInline>{text("Headline text", "Some error has happened")}</Text.Headline>
            <Text.Body as="span">{text("Body text", "Try again later")}</Text.Body>
        </ErrorContainer>
    ));