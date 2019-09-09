import React from 'react';
import { mount } from 'enzyme';
import Checkbox from '.';

describe('<Checkbox />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Checkbox value="text" onChange={event => alert(event.target.value)}/>
    );

    expect(wrapper).toExist();
  });
});
