import React from 'react';
import { mount } from 'enzyme';
import Loader from '.';

describe('<Loader />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Loader type="base" color="black" size={18} label="Loading" />
    );

    expect(wrapper).toExist();
  });
});
