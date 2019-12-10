import React from 'react';
import { mount, shallow } from 'enzyme';
import GroupButton from '.';

const baseProps = {
  label: 'test',
  disabled: false,
  opened: false,
  isDropdown: false,
}

describe('<GroupButton />', () => {
  it('renders without error', () => {
    const wrapper = mount(<GroupButton {...baseProps} />);

    expect(wrapper).toExist();
  });

  it('renders with child', () => {
    const wrapper = mount(
    <GroupButton {...baseProps} isDropdown={true} >
      <div key='demo' label='demo'>1</div>
    </GroupButton>);

    expect(wrapper).toExist();
  });

  it('applies disabled prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} disabled={true} />);

    expect(wrapper.prop('disabled')).toEqual(true);
  });

  it('applies opened prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} opened={true} />);

    expect(wrapper.prop('opened')).toEqual(true);
    expect(wrapper.state('isOpen')).toEqual(true);
  });

  it('applies isDropdown prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} isDropdown={true} />);

    expect(wrapper.prop('isDropdown')).toEqual(true);
  });

  it('applies activated prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} activated={true} />);

    expect(wrapper.prop('activated')).toEqual(true);
  });

  it('applies hovered prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} hovered={true} />);

    expect(wrapper.prop('hovered')).toEqual(true);
  });

  it('applies isSeparator prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} isSeparator={true} />);

    expect(wrapper.prop('isSeparator')).toEqual(true);
  });

  it('applies isSelect prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} isSelect={true} />);

    expect(wrapper.prop('isSelect')).toEqual(true);
  });

  it('applies checked prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} checked={true} />);

    expect(wrapper.prop('checked')).toEqual(true);
  });

  it('applies isIndeterminate prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} isIndeterminate={true} />);

    expect(wrapper.prop('isIndeterminate')).toEqual(true);
  });

  it('applies dropDownMaxHeight prop', () => {
    const wrapper = mount(<GroupButton {...baseProps} dropDownMaxHeight={100} />);

    expect(wrapper.prop('dropDownMaxHeight')).toEqual(100);
  });

  it('componentDidUpdate() state lifecycle test', () => {
    const wrapper = shallow(<GroupButton {...baseProps} />);
    const instance = wrapper.instance();

    wrapper.setState({ isOpen: true });

    instance.componentDidUpdate(wrapper.props(), wrapper.state());

    expect(wrapper.state()).toBe(wrapper.state());
  });

  it('componentDidUpdate() props lifecycle test', () => {
    const wrapper = shallow(<GroupButton {...baseProps} />);
    const instance = wrapper.instance();

    instance.componentDidUpdate({ opened: true }, wrapper.state());

    expect(wrapper.props()).toBe(wrapper.props());
  });

  it('componentWillUnmount() props lifecycle test', () => {
    const wrapper = shallow(<GroupButton {...baseProps} />);
    const instance = wrapper.instance();

    instance.componentWillUnmount();

    expect(wrapper).toExist(false);
  });

  it('causes function dropDownItemClick()', () => {
    const onClick = jest.fn();
    const onSelect = jest.fn()
    const child = (<div onClick={onClick}>1</div>);
    const wrapper = shallow(
      <GroupButton {...baseProps} opened={true} onSelect={onSelect} >
        {child}
      </GroupButton>);
    const instance = wrapper.instance();

    instance.dropDownItemClick(child);

    expect(wrapper.state('isOpen')).toBe(false);
    expect(onClick).toBeCalled();
  });

  it('causes function dropDownToggleClick()', () => {
    const wrapper = shallow(
      <GroupButton {...baseProps} opened={true} />);
    const instance = wrapper.instance();

    instance.dropDownToggleClick();

    expect(wrapper.state('isOpen')).toBe(false);
  });

  it('causes function checkboxChange()', () => {
    const wrapper = shallow(
      <GroupButton {...baseProps} selected='demo' />);
    const instance = wrapper.instance();

    instance.checkboxChange();

    expect(wrapper.state('selected')).toBe('demo');
  });

  it('causes function handleClick()', () => {
    const wrapper = mount(<GroupButton {...baseProps} />);
    const instance = wrapper.instance();

    instance.handleClick(new Event('click'));

    expect(wrapper.state('isOpen')).toBe(false);
  });

  it('accepts id', () => {
    const wrapper = mount(
      <GroupButton {...baseProps}  id="testId" />
    );

    expect(wrapper.prop('id')).toEqual('testId');
  });

  it('accepts className', () => {
    const wrapper = mount(
      <GroupButton {...baseProps}  className="test" />
    );

    expect(wrapper.prop('className')).toEqual('test');
  });

  it('accepts style', () => {
    const wrapper = mount(
      <GroupButton {...baseProps}  style={{ color: 'red' }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty('color', 'red');
  });

});


