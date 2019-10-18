import React from 'react';
import { mount } from 'enzyme';
import HelpButton from '.';

describe('<HelpButton />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <HelpButton tooltipContent="You tooltip content" />
    );
    expect(wrapper).toExist();
  });
});
