import React from 'react';
import { mount } from 'enzyme';
import Backdrop from '.';

describe('<Avatar />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Backdrop visible={false} />
    );

    expect(wrapper).toExist();
  });
});
