import React from 'react';
import { mount } from 'enzyme';
import Badge from '.';

describe('<Badge />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Badge />
    );

    expect(wrapper).toExist();
  });
});
