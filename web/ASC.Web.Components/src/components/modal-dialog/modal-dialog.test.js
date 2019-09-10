import React from 'react';
import { mount } from 'enzyme';
import ModalDialog from '.';

describe('<ModalDialog />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <ModalDialog visible={false} />
    );

    expect(wrapper).toExist();
  });
});
