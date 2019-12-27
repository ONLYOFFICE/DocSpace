import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, select, number } from '@storybook/addon-knobs/react';
import { optionsKnob as options } from '@storybook/addon-knobs';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import ComboBox from '.';
import { Icons } from '../icons';
import Button from '../button';
import RadioButton from '../radio-button';
import DropDownItem from '../drop-down-item'
import Section from '../../../.storybook/decorators/section';

const iconNames = Object.keys(Icons);
const sizeOptions = ['base', 'middle', 'big', 'huge', 'content'];

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
        disabled: true,
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
      },
      {
        key: 6,
        label: 'Option 6'
      },
      {
        key: 7,
        label: 'Option 7'
      }
    ];

    const needScrollDropDown = boolean('Need scroll dropdown', false);
    const dropDownMaxHeight = needScrollDropDown ? number('dropDownMaxHeight', 200) : null;
    const optionsMultiSelect = options('children',
      {
        button: 'button',
        icon: 'icon'
      },
      [],
      {
        display: 'multi-select',
      });

    let children = [];
    optionsMultiSelect.forEach(function (item, i) {
      switch (item) {
        case "button":
          children.push(<Button label="button" key={i} />);
          break;
        case "icon":
          children.push(<Icons.NavLogoIcon size="medium" key={i} />);
          break;
        default:
          break;
      }
    });

    const advancedOptions =
      <>
        <DropDownItem key='1' noHover>
          <RadioButton value='asc' name='first' label='A-Z' isChecked={true} />
        </DropDownItem>
        <DropDownItem key='2' noHover>
          <RadioButton value='desc' name='first' label='Z-A' />
        </DropDownItem>
        <DropDownItem key='3' isSeparator />
        <DropDownItem key='4' noHover>
          <RadioButton value='first' name='second' label='First name' />
        </DropDownItem>
        <DropDownItem key='5' noHover>
          <RadioButton value='last' name='second' label='Last name' isChecked={true} />
        </DropDownItem>
      </>;

    const childrenItems = children.length > 0 ? children : null;

    return (
      <Section>
        <table style={{ width: 584, borderCollapse: "separate", textAlign: "left" }}>
          <thead>
            <tr>
              <th>Default</th>
              <th>Advanced</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td style={{ paddingBottom: 20 }}>
                <ComboBox
                  options={comboOptions}
                  onSelect={option => action("Selected option")(option)}
                  selectedOption={{
                    key: 0,
                    label: 'Select',
                    default: true
                  }}
                  isDisabled={boolean('isDisabled', false)}
                  noBorder={boolean('noBorder', false)}
                  dropDownMaxHeight={dropDownMaxHeight}
                  scaled={boolean('scaled', false)}
                  scaledOptions={boolean('scaledOptions', false)}
                  size={select('size', sizeOptions, 'content')}
                >
                  {childrenItems}
                </ComboBox>
              </td>
              <td style={{ paddingBottom: 20 }}>
                <ComboBox
                  options={[]}
                  advancedOptions={advancedOptions}
                  onSelect={option => action("Selected option")(option)}
                  selectedOption={{
                    key: 0,
                    label: 'Select',
                    default: true
                  }}
                  isDisabled={boolean('isDisabled', false)}
                  scaled={false}
                  size='content'
                  directionX='right'
                >
                  <Icons.NavLogoIcon size="medium" key='comboIcon' />
                </ComboBox>
              </td>
            </tr>
          </tbody>
        </table>
      </Section>
    );
  });