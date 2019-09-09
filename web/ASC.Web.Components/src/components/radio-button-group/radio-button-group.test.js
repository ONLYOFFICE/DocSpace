import React from 'react';
import { mount } from 'enzyme';
import RadioButtonGroup from '.';

describe('<RadioButtonGroup />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <RadioButtonGroup 
        name='fruits' 
        selected='banana'
        options={[
                    { value: 'apple', label: 'Sweet apple'},
                    { value: 'banana', label: 'Banana'},
                    { value: 'Mandarin'}
                ]} 
      />
    );

    expect(wrapper).toExist();
  });
});
