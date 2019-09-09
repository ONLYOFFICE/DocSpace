import React from 'react';
import { mount } from 'enzyme';
import DropDown from '.';

describe('<DropDown />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <DropDown opened={false}></DropDown>
    );

    expect(wrapper).toExist();
  });
});
