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

  it('accepts id', () => {
    const wrapper = mount(
      <ErrorContainer id="testId" />
    );

    expect(wrapper.prop('id')).toEqual('testId');
  });

  it('accepts className', () => {
    const wrapper = mount(
      <ErrorContainer className="test" />
    );

    expect(wrapper.prop('className')).toEqual('test');
  });

  it('accepts style', () => {
    const wrapper = mount(
      <ErrorContainer style={{ color: 'red' }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty('color', 'red');
  });
});
