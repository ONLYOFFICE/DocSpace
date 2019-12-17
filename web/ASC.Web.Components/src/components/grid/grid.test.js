import React from 'react';
import { mount } from 'enzyme';
import Grid from '.';

describe('<Grid />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Grid />
    );

    expect(wrapper).toExist();
  });
});
