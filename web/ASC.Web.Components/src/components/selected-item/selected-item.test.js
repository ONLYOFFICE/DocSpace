import React from 'react';
import { mount } from 'enzyme';
import SelectedItem from '.';

describe('<SelectedItem />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <SelectedItem text="sample text" onClick={()=>console.log("onClick")} onClose={()=>console.log("onClose")}></SelectedItem>
    );

    expect(wrapper).toExist();
  });
});
