import React from 'react';
import { mount, shallow } from 'enzyme';
import AdvancedSelector from '.';

const baseProps = {
  placeholder: "Search users",
  options: [],
  isMultiSelect: false,
  buttonLabel: "Add members",
  onSearchChanged: jest.fn,
  onSelect: jest.fn,
  onCancel: jest.fn
};

describe('<AdvancedSelector />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <AdvancedSelector {...baseProps} />
    );

    expect(wrapper).toExist();
  });

  it('not re-render test', () => {
    const wrapper = shallow(<AdvancedSelector {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props, wrapper.state);

    expect(shouldUpdate).toBe(false);
  });

  it('re-render test by options', () => {
    const wrapper = shallow(<AdvancedSelector {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({
      ...baseProps,
      options: [{key: 1, label: "Example"}]
    }, wrapper.state);

    expect(shouldUpdate).toBe(true);
  });

  it('re-render test by groups', () => {
    const wrapper = shallow(<AdvancedSelector {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({
      ...baseProps,
      groups: [{key: 1, label: "Example"}]
    }, wrapper.state);

    expect(shouldUpdate).toBe(true);
  });

  it('re-render test by selectedOptions', () => {
    const wrapper = shallow(<AdvancedSelector {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({
      ...baseProps,
      selectedOptions: [{key: 1, label: "Example"}]
    }, wrapper.state);

    expect(shouldUpdate).toBe(true);
  });
});
