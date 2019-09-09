import React from 'react';
import { mount } from 'enzyme';
import Textarea from '.';

describe('<Textarea />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <Textarea placeholder="Add comment" onChange={event => alert(event.target.value)} value='value' />
    );

    expect(wrapper).toExist();
  });
});
