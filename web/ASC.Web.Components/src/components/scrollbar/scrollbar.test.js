import React from 'react';
import { mount } from 'enzyme';
import Scrollbar from '.';

describe('<Scrollbar />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <Scrollbar>Some content</Scrollbar>
    );

    expect(wrapper).toExist();
  });
});
