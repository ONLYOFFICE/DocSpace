import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, select, number } from '@storybook/addon-knobs/react';
import { optionsKnob as options } from '@storybook/addon-knobs';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { ComboBox, Icons, Button } from 'asc-web-components'
import Section from '../../../.storybook/decorators/section';

const iconNames = Object.keys(Icons);
const sizeOptions = ['base', 'middle', 'big', 'huge', 'content'];

const appendOptions = (comboOptions, optionsCount) => {
  let newOptions = comboOptions;
  for (let i = 0; i <= optionsCount; i++) {
    newOptions.push({
      key: (i + 6),
      label: 'Option ' + (i + 6)
    })
  }
  return newOptions;
};

iconNames.push("NONE");

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('combo box', () => {

    const comboOptions = [
      {
        key: 1,
        icon: 'CatalogEmployeeIcon',
        label: 'Option 1'
      },
      {
        key: 2,
        icon: 'CatalogGuestIcon',
        label: 'Option 2',
      },
      {
        key: 3,
        label: 'Option 3'
      },
      {
        key: 4,
        label: 'Option 4'
      },
      {
        key: 5,
        icon: 'CopyIcon',
        label: 'Option 5'
      }
    ];

    const optionsCount = number('Add options', 1,
      {
        range: true,
        min: 1,
        max: 100,
        step: 1
      }
    );

    const needScrollDropDown = boolean('Need scroll dropdown', false);
    const dropDownMaxHeight = needScrollDropDown && number('dropDownMaxHeight', 200);
    const optionsMultiSelect = options('Children',
      {
        button: 'button',
        icon: 'icon'
      },
      ['icon'],
      {
        display: 'multi-select',
      });

    let children = [];
    optionsMultiSelect.forEach(function (item, i) {
      switch (item) {
        case "button":
          children.push(<Button label="OK" key={i} />);
          break;
        case "icon":
          children.push(<Icons.SettingsIcon size="medium" key={i} />);
          break;
        default:
          break;
      }
    });

    return (
      <Section>
        <ComboBox
          options={appendOptions(comboOptions, optionsCount)}
          onSelect={option => action("Selected option")(option)}
          isDisabled={boolean('isDisabled', false)}
          dropDownMaxHeight={dropDownMaxHeight + 'px'}
          scaled={boolean('scaled', false)}
          size={select('size', sizeOptions, 'content')}
        >
          {children}
        </ComboBox>
      </Section>
    );
  });