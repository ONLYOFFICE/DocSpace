import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { withKnobs, text } from '@storybook/addon-knobs/react';
import Text from '../text';
import Heading from '../heading';
import ErrorContainer from '.';

storiesOf('Components| ErrorContainer', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => (
        <ErrorContainer>
            <Heading level={2} size='medium' isInline>{text("Headline text", "Some error has happened")}</Heading>
            <Text as="span">{text("Body text", "Try again later")}</Text>
        </ErrorContainer>
    ));