import React from 'react';
import { mount, shallow } from 'enzyme';
import LinkWithDropdown from '.';

const data = [
  {
    key: 'key1',
    label: 'Button 1',
    onClick: () => console.log('Button1 action'),
  },
  {
    key: 'key2',
    label: 'Button 2',
    onClick: () => console.log('Button2 action'),
  },
  {
    key: 'key3',
    isSeparator: true
  },
  {
    key: 'key4',
    label: 'Button 3',
    onClick: () => console.log('Button3 action'),
  },
];

describe('<LinkWithDropdown />', () => {

  it('renders without error', () => {
    const wrapper = mount(<LinkWithDropdown color="#333333" isBold={true} data={[]}>Link with dropdown</LinkWithDropdown>);

    expect(wrapper).toExist();
  });

  it('re-render test', () => {
    const wrapper = mount(<LinkWithDropdown color="#333333" isBold={true} data={data}>Link with dropdown</LinkWithDropdown>);

    const instance = wrapper.instance();
    const shouldUpdate = instance.shouldComponentUpdate({
      isBold: false
    }, wrapper.state);

    expect(shouldUpdate).toBe(true);

  });

  it('re-render after changing color', () => {

    const wrapper = shallow(<LinkWithDropdown color="#333333" isBold={true} data={data}>Link with dropdown</LinkWithDropdown>);
    const instance = wrapper.instance();

    const shouldUpdate = instance.shouldComponentUpdate({
      color: "#999"
    }, instance.state);

    expect(shouldUpdate).toBe(true);
  });

  it('re-render after changing dropdownType and isOpen prop', () => {

    const wrapper = shallow(<LinkWithDropdown color="#333333" isBold={true} data={data}>Link with dropdown</LinkWithDropdown>);
    const instance = wrapper.instance();

    const shouldUpdate = instance.shouldComponentUpdate({
      isOpen: true,
      dropdownType: 'appearDashedAfterHover'
    }, instance.state);

    expect(shouldUpdate).toBe(true);
  });

  it('re-render after changing isOpen prop', () => {

    const wrapper = shallow(<LinkWithDropdown color="#333333" isBold={true} data={data}>Link with dropdown</LinkWithDropdown>);
    const instance = wrapper.instance();

    const shouldUpdate = instance.shouldComponentUpdate({
      isOpen: true
    }, instance.state);

    expect(shouldUpdate).toBe(true);
  });


  it('not re-render', () => {
    const wrapper = mount(<LinkWithDropdown color="#333333" isBold={true} data={data}>Link with dropdown</LinkWithDropdown>);

    const instance = wrapper.instance();
    const shouldUpdate = instance.shouldComponentUpdate(instance.props, instance.state);

    expect(shouldUpdate).toBe(false);

  });

});
