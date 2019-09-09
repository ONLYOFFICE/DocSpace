import React from 'react';
import { mount } from 'enzyme';
import LinkWithDropdown from '.';

describe('<LinkWithDropdown />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <LinkWithDropdown color = "black" isBold = {true} data={[]}>Link with dropdown</LinkWithDropdown>
    );

    expect(wrapper).toExist();
  });
});
