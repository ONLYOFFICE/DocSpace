import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { Value } from 'react-value';
import { withKnobs, boolean } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Section from '../../.storybook/decorators/section';
import { ModalDialog, Button } from 'asc-web-components';

const header = "Header text";

const body = <p>{"Body text"}</p>;

const footer = [
  <Button label="Ok" onClick={e => { action('onOk')(e)}} primary/>,
  <Button label="Cancel" onClick={e => { action('onCancel')(e)}} style={{marginLeft:"8px"}}/>
];

storiesOf('Components|ModalDialog', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('Modal dialog', () => (
    <Section>
      <Value
        defaultValue={true}
        render={(isOpen, onChange) => (
          <ModalDialog
            isOpen={boolean('isOpen', isOpen)}
            headerContent={header}
            bodyContent={body}
            footerContent={footer}
            onClose={event => {
              action('onClose')(event);
              onChange(false);
            }}
          />
        )}
      />  
    </Section>
  ));