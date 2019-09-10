import React from 'react';
import { mount } from 'enzyme';
import ErrorContainer from '.';

describe('<ErrorContainer />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <ErrorContainer>Some error has happened</ErrorContainer>
    );

    expect(wrapper).toExist();
  });
});
