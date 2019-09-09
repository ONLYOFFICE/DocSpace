import React from 'react';
import { mount } from 'enzyme';
import Layout from '.';

describe('<Layout />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Layout />
    );

    expect(wrapper).toExist();
  });
});
