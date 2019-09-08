import React from 'react';
import { mount } from 'enzyme';
import { Text } from '.';

describe('<Text />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <Text.Body as='p' title='Some title'>
        Some text
    </Text.Body>
    );

    expect(wrapper).toExist();
  });
});
