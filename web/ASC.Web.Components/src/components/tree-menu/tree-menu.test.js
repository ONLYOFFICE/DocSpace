import React from 'react';
import { mount } from 'enzyme';
import TreeMenu from '.';

describe('<TreeMenu />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <TreeMenu data={[]} />
    );

    expect(wrapper).toExist();
  });
});
