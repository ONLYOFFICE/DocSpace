import React from 'react';
import { mount } from 'enzyme';
import Badge from '.';

describe('<Avatar />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Badge />
    );

    expect(wrapper).toExist();
  });
});
