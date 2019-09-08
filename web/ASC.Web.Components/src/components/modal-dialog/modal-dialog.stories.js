import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { BooleanValue } from 'react-values'
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Section from '../../.storybook/decorators/section';
import ModalDialog from '.';
import Button from '../button';


storiesOf('Components|ModalDialog', module)
  .addDecorator(withReadme(Readme))
  .add('Modal dialog', () => (
    <Section>
      <BooleanValue>
        {({ value, toggle }) => (
          <div>
            <Button
              label="Show"
              primary={true}
              onClick={(e) => {
                action('onShow')(e);
                toggle(true);
              }}
            />
            <ModalDialog
              visible={value}
              headerContent="Change password"
              bodyContent={
                <div>Send the password change instruction to the <a href="mailto:asc@story.book">asc@story.book</a> email address</div>
              }
              footerContent={[
                <Button
                  key="SendBtn"
                  label="Send"
                  primary={true}
                  size="medium"
                  onClick={(e) => {
                    action('onOk')(e);
                    toggle(false);
                  }}
                />
              ]}
              onClose={e => {
                action('onClose')(e);
                toggle(false);
              }}
            />
          </div>
        )}
      </BooleanValue>  
    </Section>
  ));