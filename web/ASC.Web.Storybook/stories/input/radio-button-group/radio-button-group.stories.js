import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import { RadioButtonGroup } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import {withKnobs} from '@storybook/addon-knobs/react';
import Readme from './README.md';

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('radio-button-group', () => {
    console.log('radio box rendering')
    return (
      <Section>
        <>
        <div> First list</div>
          <RadioButtonGroup
           selected='apple' 
           name="test" 
           options={
                    [
                      { value: 'apple', label: 'Apple' },
                      { value: 'mandarin', label: 'Mandarin'}
                    ]
                  } 
            />
          <div> Second list</div>
          <RadioButtonGroup
            selected='veh' 
            name="test2" 
            options={
                      [
                        { value: 'Car' },
                        { value: 'veh', label: 'Vehicle', disabled: true },
                        { value: 'veh1', label: 'Vehicle1' },
                        { value: 'veh2', label: 'Vehicle2', disabled: true }
                      ]
                    } 
            />
        </>
      </Section>
    );
  });
