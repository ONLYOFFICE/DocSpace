import React from 'react';
import { mount } from 'enzyme';
import Label from '.';

describe('<IconButton />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Label
        text="First name:"
        title={"first name"}
        htmlFor="firstNameField"
        display="block"
      />
    );

    expect(wrapper).toExist();
  });
});
