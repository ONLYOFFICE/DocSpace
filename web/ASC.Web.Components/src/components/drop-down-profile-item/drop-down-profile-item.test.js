import React from 'react';
import { mount } from 'enzyme';
import DropDownProfileItem from '.';

const baseProps = {
  avatarRole: 'admin',
  avatarSource: '',
  displayName: 'Jane Doe',
  email: 'janedoe@gmail.com'
}

describe('<DropDownProfileItem />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <DropDownProfileItem {...baseProps} />
    );

    expect(wrapper).toExist();
  });

  it('accepts id', () => {
    const wrapper = mount(
      <DropDownProfileItem {...baseProps} id="testId" />
    );

    expect(wrapper.prop('id')).toEqual('testId');
  });

  it('accepts className', () => {
    const wrapper = mount(
      <DropDownProfileItem {...baseProps} className="test" />
    );

    expect(wrapper.prop('className')).toEqual('test');
  });

  it('accepts style', () => {
    const wrapper = mount(
      <DropDownProfileItem {...baseProps} style={{ color: 'red' }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty('color', 'red');
  });
});
