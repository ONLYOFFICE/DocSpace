import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs } from '@storybook/addon-knobs/react';
import { SelectedItem } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

function onCloseButtonClick() {
  console.log("onCloseButtonClick");
}

storiesOf('Components|SelectedItem', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('selected item', () => {

    return (
      <Section>
        <SelectedItem  
          text={text('text', 'Selected item')}
          isInline={boolean('isInline', true)}
          clickAction={onCloseButtonClick}
          isDisabled={boolean('isDisabled', false)}
        />
      </Section>
    )
  });
