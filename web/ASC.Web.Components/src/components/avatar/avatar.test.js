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

  it('render owner avatar', () => {
    const wrapper = mount(
      <Avatar {...baseProps} role='owner' />
    );

    expect(wrapper.prop('role')).toEqual('owner');
  });

  it('render guest avatar', () => {
    const wrapper = mount(
      <Avatar {...baseProps} role='guest' />
    );

    expect(wrapper.prop('role')).toEqual('guest');
  });

  it('render big avatar', () => {
    const wrapper = mount(
      <Avatar {...baseProps} size='big' />
    );

    expect(wrapper.prop('size')).toEqual('big');
  });

  it('render medium avatar', () => {
    const wrapper = mount(
      <Avatar {...baseProps} size='medium' />
    );

    expect(wrapper.prop('size')).toEqual('medium');
  });

  it('render small avatar', () => {
    const wrapper = mount(
      <Avatar {...baseProps} size='small' />
    );

    expect(wrapper.prop('size')).toEqual('small');
  });

  it('render empty avatar', () => {
    const wrapper = mount(
      <Avatar {...baseProps} userName='' source='' />
    );

    expect(wrapper.prop('userName')).toEqual('');
    expect(wrapper.prop('source')).toEqual('');
  });

  it('render source avatar', () => {
    const wrapper = mount(
      <Avatar {...baseProps} userName='Demo User' source='demo' />
    );

    expect(wrapper.prop('userName')).toEqual('Demo User');
    expect(wrapper.prop('source')).toEqual('demo');
  });

  it('render initials avatar', () => {
    const wrapper = mount(
      <Avatar {...baseProps} userName='Demo User' source='' />
    );

    expect(wrapper.prop('userName')).toEqual('Demo User');
    expect(wrapper.prop('source')).toEqual('');
  });

  it('render editing avatar', () => {
    const wrapper = mount(
      <Avatar {...baseProps} editing />
    );

    expect(wrapper.prop('editing')).toEqual(true);
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
