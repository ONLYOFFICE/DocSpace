import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { BooleanValue } from 'react-values'
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Section from '../../.storybook/decorators/section';
import { ModalDialog, Button } from 'asc-web-components';


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
              isOpen={value}
              headerContent={"Header text"}
              bodyContent={
                <p>{"Body text"}</p>
              }
              footerContent={[
                <Button
                  key="OkBtn"
                  label="Ok"
                  primary={true}
                  onClick={(e) => {
                    action('onOk')(e);
                    toggle(false);
                  }}
                />,
                <Button
                  key="CancelBtn"
                  label="Cancel"
                  onClick={(e) => {
                    action('onCancel')(e);
                    toggle(false);
                  }}
                  style={{marginLeft:"8px"}}
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