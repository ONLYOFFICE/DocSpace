import React from 'react';
import { mount } from 'enzyme';
import DateInput from '.';

describe('<DateInput />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <DateInput selected={new Date()} dateFormat="dd.MM.yyyy" onChange={date => {}}/>
    );

    expect(wrapper).toExist();
  });
});
