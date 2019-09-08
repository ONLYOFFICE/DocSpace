import React from 'react';
import { mount } from 'enzyme';
import GroupButton from '.';

describe('<GroupButton />', () => {
    it('renders without error', () => {
        const wrapper = mount(
            <GroupButton label='Group button' disabled={false} isDropdown={false} opened={false} ></GroupButton>
        );

        expect(wrapper).toExist();
    });
});


