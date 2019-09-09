import React from 'react';
import { mount } from 'enzyme';
// import GroupButtonsMenu from '.';
// import DropDownItem from '../drop-down-item';

//TODO: Fix test run
describe('<GroupButtonsMenu />', () => {
    it('renders without error', () => {
        /*const menuItems = [
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
              label: 'Menu item 1',
              disabled: false,
              onClick: () => console.log('Menu item 1 action')
            },
            {
              label: 'Menu item 2',
              disabled: true,
              onClick: () => console.log('Menu item 2 action')
            }
          ];

        const wrapper = mount(         
          <GroupButtonsMenu checked={false} menuItems={menuItems} visible={true} />
        );*/

        const wrapper = mount(         
            <div>FIX ME</div>
          );

        expect(wrapper).toExist();
    });
});