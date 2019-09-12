import React from 'react';
import { mount, shallow } from 'enzyme';
import Avatar from '.';

const baseProps = {
  size: 'max',
  role: 'user',
  source: '',
  editLabel: 'Edit',
  userName: 'Demo User',
  editing: false,
  editAction: () => jest.fn()
}

describe('<Avatar />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Avatar {...baseProps} />
    );

    expect(wrapper).toExist();
  });

  it('not re-render test', () => {
    const wrapper = shallow(<Avatar {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props, wrapper.state);

    expect(shouldUpdate).toBe(false);
  });

  it('re-render test', () => {
    const wrapper = shallow(<Avatar {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({
      size: 'max',
      role: 'admin',
      source: '',
      editLabel: 'Edit',
      userName: 'Demo User',
      editing: false,
      editAction: () => jest.fn()
    }, wrapper.state);

    expect(shouldUpdate).toBe(true);
  });
});
