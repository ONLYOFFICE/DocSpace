import React from 'react';
import { mount } from 'enzyme';
import ToggleContent from '.';

describe('<ToggleContent />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <ToggleContent>
        <span>Some text</span>
      </ToggleContent>
    );

    expect(wrapper).toExist();
  });
});
