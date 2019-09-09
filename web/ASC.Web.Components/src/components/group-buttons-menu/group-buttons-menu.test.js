import React from 'react';
import { mount } from 'enzyme';
import GroupButtonsMenu from '.';
import DropDownItem from '../drop-down-item';

describe('<GroupButtonsMenu />', () => {
    it('renders without error', () => {
        const menuItems = [
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
              onSelect: () => {}
            },
            {
              label: 'Menu item 1',
              disabled: false,
              onClick: () => {}
            },
            {
              label: 'Menu item 2',
              disabled: true,
              onClick: () => {}
            }
          ];

        const wrapper = mount(         
          <GroupButtonsMenu checked={false} menuItems={menuItems} visible={true} moreLabel='More' closeTitle='Close' />
        );

        expect(wrapper).toExist();
    });
});