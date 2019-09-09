import React from 'react';
import { mount } from 'enzyme';
import DropDownProfileItem from '.';

describe('<DropDownProfileItem />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <DropDownProfileItem
        avatarRole='admin'
        avatarSource=''
        displayName='Jane Doe'
        email='janedoe@gmail.com' />
    );

    expect(wrapper).toExist();
  });
});
