import React from 'react';
import { mount } from 'enzyme';
import Link from '.';

describe('<Link />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Link type = "page" color = "black" href="https://github.com" isBold = {true}>Bold page link</Link>
    );

    expect(wrapper).toExist();
  });
});
