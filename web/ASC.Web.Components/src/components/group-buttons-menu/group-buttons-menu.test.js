import React from 'react';
import { mount, shallow } from 'enzyme';
import GroupButtonsMenu from '.';
import DropDownItem from '../drop-down-item';

const defaultMenuItems = [
  {
    label: 'Select',
    isDropdown: true,
    isSeparator: true,
    isSelect: true,
    fontWeight: 'bold',
    children: [
      <DropDownItem key='aaa' label='aaa' />,
      <DropDownItem key='bbb' label='bbb' />,
      <DropDownItem key='ccc' label='ccc' />,
    ],
    onSelect: () => { }
  },
  {
    label: 'Menu item 1',
    disabled: false,
    onClick: () => { }
  },
  {
    label: 'Menu item 2',
    disabled: true,
    onClick: () => { }
  }
];

const baseProps = {
  checked: false,
  menuItems: defaultMenuItems,
  visible: true,
  moreLabel: 'More',
  closeTitle: 'Close'
}

describe('<GroupButtonsMenu />', () => {
  it('renders without error', () => {
    const wrapper = mount(<GroupButtonsMenu {...baseProps} />);

    expect(wrapper).toExist();
  });

  it('applies checked prop', () => {
    const wrapper = mount(<GroupButtonsMenu {...baseProps} checked={true} />);

    expect(wrapper.prop('checked')).toEqual(true);
  });

  it('applies visible prop', () => {
    const wrapper = mount(<GroupButtonsMenu {...baseProps} visible={true} />);

    expect(wrapper.prop('visible')).toEqual(true);
  });

  it('causes function closeMenu()', () => {
    const onClose = jest.fn();
    const wrapper = mount(<GroupButtonsMenu {...baseProps} onClose={onClose} />);
    const instance = wrapper.instance();

    instance.closeMenu(new Event('click'));

    expect(wrapper.state('visible')).toBe(false);
    expect(onClose).toBeCalled();
  });

  it('causes function groupButtonClick()', () => {
    const onClick = jest.fn();
    const item = {
      label: 'Menu item 1',
      disabled: false,
      onClick: onClick
    };
    const wrapper = mount(<GroupButtonsMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.groupButtonClick(item);

    expect(wrapper.state('visible')).toBe(false);
    expect(onClick).toBeCalled();
  });

  it('causes function countMenuItems()', () => {
    const wrapper = mount(<GroupButtonsMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.countMenuItems([], 2, 1);
    instance.countMenuItems([1, 2], 200, 2);
    instance.countMenuItems([1,2,3,4], 1, 2);

    expect(wrapper.state('visible')).toBe(true);
  });

  it('causes function groupButtonClick() if disabled', () => {
    const onClick = jest.fn();
    const item = {
      label: 'Menu item 1',
      disabled: true,
      onClick: onClick
    };
    const wrapper = mount(<GroupButtonsMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.groupButtonClick(item);

    expect(wrapper.state('visible')).toBe(true);
    expect(onClick).toBeCalledTimes(0);
  });

  it('componentDidUpdate() props lifecycle test', () => {
    const wrapper = shallow(<GroupButtonsMenu {...baseProps} visible={false} />);
    const instance = wrapper.instance();

    instance.componentDidUpdate({ visible: true, menuItems: defaultMenuItems }, wrapper.state());

    expect(wrapper.props()).toBe(wrapper.props());
  });

  it('componentWillUnmount() props lifecycle test', () => {
    const wrapper = shallow(<GroupButtonsMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.componentWillUnmount();

    expect(wrapper).toExist(false);
  });

  it('filled state moreItems', () => {
    const wrapper = shallow(<GroupButtonsMenu {...baseProps} />);

    wrapper.setState({
      moreItems: [{
        label: 'Menu item 1',
        disabled: false,
        onClick: jest.fn()
      }]
    });

    expect(wrapper).toExist(false);
  });

});