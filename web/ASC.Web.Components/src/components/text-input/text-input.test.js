import React from 'react';
import { mount } from 'enzyme';
import TextInput from '.';

describe('<TextInput />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <TextInput value="text" onChange={event => alert(event.target.value)} />
    );

    expect(wrapper).toExist();
  });
});
