import React from 'react';
import { mount } from 'enzyme';
import ToggleButton from '.';

describe('<ToggleButton />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <ToggleButton label="text" onChange={event => console.log(event.target.value)} isChecked={false} />
    );

    expect(wrapper).toExist();
  });
});
