import React from 'react';
import { mount } from 'enzyme';
import ContextMenu from '.';

const baseProps = {
  options: []
}

describe('<ContextMenu />', () => {
  it('renders without error', () => {
    const wrapper = mount(<ContextMenu {...baseProps} />);

    expect(wrapper).toExist();
  });

  it('componentWillUnmount() test unmount', () => {
    const wrapper = mount(<ContextMenu {...baseProps} />);

    wrapper.unmount();

    expect(wrapper).toEqual(wrapper);
  });

  it('simulate handleClick() with change state to close context menu', () => {
    const wrapper = mount(<ContextMenu {...baseProps} />);
    const instance = wrapper.instance();

    wrapper.setState({ visible: true });
    instance.handleClick(new Event('click', { target: null }));

    expect(wrapper.state('visible')).toEqual(false);
  });

  it('simulate handleClick() to close context menu', () => {
    const wrapper = mount(<ContextMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.handleClick(new Event('click', { target: null }));
    expect(wrapper.state('visible')).toEqual(false);
  });

  it('simulate handleScroll() with change state to close context menu', () => {
    const wrapper = mount(<ContextMenu {...baseProps} />);
    const instance = wrapper.instance();

    wrapper.setState({ visible: true });
    instance.handleScroll();

    expect(wrapper.state('visible')).toEqual(false);
  });

  it('simulate handleScroll() to close context menu', () => {
    const wrapper = mount(<ContextMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.handleScroll();
    expect(wrapper.state('visible')).toEqual(false);
  });

  it('render with options', () => {
    const options = [
      { label: 'test' },
      { key: 2, label: 'test' },
      false,
      { key: 4, label: 'test' }
    ];

    const wrapper = mount(<ContextMenu options={options} />);
    wrapper.setState({ visible: true });

    expect(wrapper.props().options).toEqual(options);
  });

  it('simulate handleContextMenu() to close context menu', () => {
    const wrapper = mount(<ContextMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.handleContextMenu();

    expect(wrapper.state('visible')).toEqual(true);
  });

  it('simulate handleContextMenu(e) to close context menu', () => {
    const wrapper = mount(<ContextMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.handleContextMenu(new Event('click', { target: null }));

    expect(wrapper.state('visible')).toEqual(true);
  });

  it('accepts id', () => {
    const wrapper = mount(<ContextMenu {...baseProps} id="testId" />);

    expect(wrapper.prop('id')).toEqual('testId');
  });

  it('accepts className', () => {
    const wrapper = mount(<ContextMenu {...baseProps} className="test" />);

    expect(wrapper.prop('className')).toEqual('test');
  });

  it('accepts style', () => {
    const wrapper = mount(<ContextMenu {...baseProps} style={{ color: 'red' }} />);

    wrapper.setState({visible: true});

    expect(wrapper.getDOMNode().style).toHaveProperty('color', 'red');
  });
});
