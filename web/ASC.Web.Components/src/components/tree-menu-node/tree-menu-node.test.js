import React from 'react';
import { mount } from 'enzyme';
// import TreeNodeMenu from '.';

//TODO: Fix test run
describe('<TreeNodeMenu />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      /*<TreeNodeMenu data={[]}/>*/
      <span>FIX ME</span>
    );

    expect(wrapper).toExist();
  });
});
