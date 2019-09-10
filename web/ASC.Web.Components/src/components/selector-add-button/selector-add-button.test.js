import React from 'react';
import { mount } from 'enzyme';
import SelectorAddButton from '.';

describe('<SelectorAddButton />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <SelectorAddButton title="Add item" isDisabled={false} onClick={()=>console.log("onClick")}></SelectorAddButton>
    );

    expect(wrapper).toExist();
  });
});
