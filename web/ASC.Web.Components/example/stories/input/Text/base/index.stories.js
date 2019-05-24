import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { Value } from 'react-value';
import { withKnobs, boolean, text, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';


import { TextInput } from 'asc-web-components';

const sizeOptions = ['base', 'middle', 'big', 'huge', 'scale'];

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('text', () => (
      <Value
        defaultValue=""
        render={(value, onChange) => (
            <TextInput 
              id={text('id', '')}
              name={text('name', '')}
              value={text('value', value)} 
              placeholder={text('placeholder', 'This is placeholder')}
              maxLength={number('maxLength', 255)}
              size={select('size', sizeOptions, 'middle')}
              onChange={event => {
                action('onChange')(event);
                onChange(event.target.value);
              }}
              onBlur={action('onBlur')}
              onFocus={action('onFocus')}
              isAutoFocussed={boolean('isAutoFocussed', false)}
              isDisabled={boolean('isDisabled', false)}
              isReadOnly={boolean('isReadOnly', false)}
              hasError={boolean('hasError', false)}
              hasWarning={boolean('hasWarning', false)} 
              autoComplete={text('autoComplete', 'off')}
              tabIndex={number('tabIndex', 1)}

            />    
          )}
        />
  ));
