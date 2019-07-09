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
            <Text.Headline tag="h2">{text("Headline text", "Some error has happened")}</Text.Headline>
            <Text.Body tag="span">{text("Body text", "Try again later")}</Text.Body>
        </ErrorContainer>
    ));