import React from 'react';
import { mount } from 'enzyme';
import RadioButton from '.';

describe('<RadioButton />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <RadioButton
        name='fruits'
        value= 'apple'
        label= 'Sweet apple'
      />
    );

    expect(wrapper).toExist();
  });
});
