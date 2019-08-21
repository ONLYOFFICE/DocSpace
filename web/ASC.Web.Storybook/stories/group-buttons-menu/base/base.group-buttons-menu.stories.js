import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, text } from '@storybook/addon-knobs/react';
import { BooleanValue } from 'react-values'
import withReadme from 'storybook-readme/with-readme'
import styled from '@emotion/styled';
import Readme from './README.md'
import { GroupButtonsMenu, DropDownItem, Button } from 'asc-web-components'

const GroupButtonsMenuContainer = styled.div`
  height: 2000px;
`;

const groupItems = [
  {
    label: 'Select',
    isDropdown: true,
    isSeparator: true,
    isSelect: true,
    fontWeight: 'bold',
    children: [
      <DropDownItem key='aaa' label='aaa' />,
      <DropDownItem key='bbb' label='bbb' />,
      <DropDownItem key='ccc' label='ccc' />,
    ],
    onSelect: (a) => console.log(a)
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    disabled: true,
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    disabled: true,
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  },
  {
    label: 'Menu item',
    onClick: () => console.log('Menu item action')
  }
];

storiesOf('Components|GroupButtonsMenu', module)
  .addDecorator(withReadme(Readme))
  .addDecorator(withKnobs)
  .add('base', () => {
    return (
      <BooleanValue>
        {({ value: visible, toggle }) => (
          <>
            <Button
              label="Show menu"
              onClick={(e) => {
                toggle(visible);
              }}
            />
            <GroupButtonsMenuContainer>
              <BooleanValue>
                {({ value: checked, toggle }) => (
                  <GroupButtonsMenu
                    checked={checked}
                    menuItems={groupItems}
                    visible={visible}
                    moreLabel={text('moreLabel', 'More')}
                    closeTitle={text('closeTitle', 'Close')}
                    onClose={() => console.log('Close action')}
                    onChange={(e) => toggle(checked)}
                    selected={groupItems[0].label}
                  />
                )}
              </BooleanValue>
            </GroupButtonsMenuContainer>
          </>
        )}
      </BooleanValue>
    );
  });