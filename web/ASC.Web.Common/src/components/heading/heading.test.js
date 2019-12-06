import React from 'react';
import { mount } from 'enzyme';
import Heading from '.';

describe('<Heading />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <Heading as='span' title='Some title'>
        Some text
    </Heading>
    );

    expect(wrapper).toExist();
  });
});
