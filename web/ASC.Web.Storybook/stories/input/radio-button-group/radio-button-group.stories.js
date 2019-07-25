import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import { RadioButtonGroup } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import {text, boolean, withKnobs, select, number } from '@storybook/addon-knobs/react';
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
          <RadioButtonGroup spaceBtwnElems='33' selected='apple' name="test" options={
            [
              { value: 'apple', label: 'Яблоко' },
              { value: 'mandarin', label: 'Мандарин'}]} />
          <div> Second list</div>
          <RadioButtonGroup spaceBtwnElems='55' selected='veh' name="test2" options={
            [
              { value: 'Car' },
              { value: 'veh', label: 'Vehicle', disabled: true },
              { value: 'veh1', label: 'Vehicle1' },
              { value: 'veh2', label: 'Vehicle2', disabled: true }
            ]} />
        </>
      </Section>
    );
  });
