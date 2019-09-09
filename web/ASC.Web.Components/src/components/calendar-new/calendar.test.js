import React from 'react';
import { mount } from 'enzyme';
import NewCalendar from '.';

describe('<NewCalendar />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <NewCalendar
          onChange={date => {
          console.log('Selected date:', date);
          }}
          disabled={false}
          themeColor='#ED7309'
          selectedDate={new Date()}
          openToDate={new Date()}
          minDate={new Date("1970/01/01")}
          maxDate={new Date("3000/01/01")}
          locale='ru'
          scaled={false}
      />
    );

    expect(wrapper).toExist();
  });
});
