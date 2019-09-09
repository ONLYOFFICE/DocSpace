import React from 'react';
import { mount } from 'enzyme';
import DatePicker from '.';

describe('<DatePicker />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <DatePicker
        onChange={date => {
          console.log('Selected date', date);
        }}
        selectedDate={new Date()}
        minDate={new Date("1970/01/01")}
        maxDate={new Date(new Date().getFullYear() + 1 + "/01/01")}
      />
    );

    expect(wrapper).toExist();
  });
});
