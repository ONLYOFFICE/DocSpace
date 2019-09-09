import React from 'react';
import { mount } from 'enzyme';
import Avatar from '.';

describe('<Avatar />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Avatar size='max' role='admin' source='' userName='' editing={false} />
    );

    expect(wrapper).toExist();
  });
});
