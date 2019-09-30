import React from 'react';
import { mount } from 'enzyme';
import SidePanel from '.';

describe('<SidePanel />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <SidePanel visible={false} scale={false}/>
    );

    expect(wrapper).toExist();
  });
});
