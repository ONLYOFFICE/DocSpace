import React from 'react';
import { mount } from 'enzyme';
import Header from '.';

describe('<Header />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <Header as='span' title='Some title'>
        Some text
    </Header>
    );

    expect(wrapper).toExist();
  });
});
