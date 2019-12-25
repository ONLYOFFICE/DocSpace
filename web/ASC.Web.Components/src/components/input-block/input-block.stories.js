import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, text, select, number, color } from '@storybook/addon-knobs/react';
import { optionsKnob as options } from '@storybook/addon-knobs';
import withReadme from 'storybook-readme/with-readme';
import { StringValue } from 'react-values';
import Readme from './README.md';
import InputBlock from '.';
import { Icons } from '../icons';
import Button from '../button';
import Section from '../../../.storybook/decorators/section';

const iconNames = Object.keys(Icons);
iconNames.push("NONE");

const sizeOptions = ['base', 'middle', 'big', 'huge'];

const IconClick = function(){
  action('iconClick')();
};

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('input block', () => {
    const valuesMultiSelect = {
      button: "button",
      icon: "icon"
    };
    const optionsMultiSelect = options('Children', valuesMultiSelect, ["icon"], {
      display: 'multi-select',
    });
    
    var children = []; 
    optionsMultiSelect.forEach(function (item, i) {
      switch (item) {
        case "button":
          children.push(<Button label="OK" key={i}/>); 
          break;
        case "icon":
          children.push(<Icons.SettingsIcon size="medium" key={i}/>); 
          break;
        default:
          break;
      }
      
    });

    return(
      <div>
        <StringValue
          onChange={e => {
              action('onChange')(e);
            }
          }
        >
          {({ value, set }) => (
            <Section>
              <InputBlock 
                id={text('id', '')}
                name={text('name', '')}
                placeholder={text('placeholder', 'This is placeholder')}
                maxLength={number('maxLength', 255)}
                size={select('size', sizeOptions, 'base')}
                onBlur={action('onBlur')}
                onFocus={action('onFocus')}
                isAutoFocussed={boolean('isAutoFocussed', false)}
                isReadOnly={boolean('isReadOnly', false)}
                hasError={boolean('hasError', false)}
                hasWarning={boolean('hasWarning', false)}
                scale={boolean('scale', false)}
                autoComplete={text('autoComplete', 'off')}
                tabIndex={number('tabIndex', 1)}
                iconSize={number('iconSize', 0)}

                mask={text("mask", null)}

                isDisabled={boolean('isDisabled', false)}
                iconName={select('iconName', iconNames, 'SearchIcon')}
                iconColor={color('iconColor', "#D0D5DA")}
                isIconFill={boolean('isIconFill', false)}
                value={value}
                onIconClick={IconClick}
                onChange={e => { 
                  set(e.target.value);
                }}>
                  {children}
                </InputBlock>
            </Section>
          )}
        </StringValue>
      </div> 
    );
                      });
