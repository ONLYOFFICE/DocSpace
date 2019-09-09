import React from 'react';
import { mount } from 'enzyme';
import RequestLoader from '.';

describe('<RequestLoader />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <RequestLoader label="Loading... Please wait..." />
    );

    expect(wrapper).toExist();
  });
});
