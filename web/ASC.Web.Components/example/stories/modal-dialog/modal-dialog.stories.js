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
      <BooleanValue
        onChange={e => {
            action('onShow')(e);
        }}
      >
        {({ value, toggle }) => (
          <div>
            <Button label="Show" primary={true} onClick={() => { toggle(true); }}/>
            <ModalDialog
              isOpen={value}
              headerContent={"Header text"}
              bodyContent={<p>{"Body text"}</p>}
              footerContent={[
                <Button key="OkBtn" label="Ok" onClick={() => { toggle(false);}} primary={true}/>,
                <Button key="CancelBtn" label="Cancel" onClick={() => { toggle(false);}} style={{marginLeft:"8px"}}/>
              ]}
              onClose={e => { toggle(false); }}
            />
          </div>
        )}
      </BooleanValue>  
    </Section>
  ));