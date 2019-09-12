import React from 'react';
import { mount, shallow } from 'enzyme';
import ComboBox from '.';
import DropDownItem from '../drop-down-item';

const baseOptions = [
  {
    key: 0,
    label: "Select"
  },
  {
    key: 1,
    label: "Select"
  },
  {
    key: 2,
    label: "Select"
  }
];

const advancedOptions = (
  <>
    <DropDownItem>
      <span>Some text</span>
    </DropDownItem>
  </>
);

const baseProps = {
  noBorder: false,
  isDisabled: false,
  selectedOption: {
    key: 0,
    label: "Select"
  },
  options: baseOptions,
  opened: false,
  onSelect: () => jest.fn(),
  size: 'base',
  scaled: true
};

describe('<ComboBox />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <ComboBox {...baseProps} />
    );

    expect(wrapper).toExist();
  });

  it('render with advanced options', () => {
    const wrapper = mount(
      <ComboBox {...baseProps} options={[]} advancedOptions={advancedOptions} />
    );

    expect(wrapper).toExist();
  });

  it('disabled when isDisabled is passed', () => {
    const wrapper = mount(<ComboBox {...baseProps} isDisabled={true} />);

    expect(wrapper.prop('isDisabled')).toEqual(true);
  });

  it('not re-render test', () => {
    const wrapper = shallow(<ComboBox {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props, wrapper.state);

    expect(shouldUpdate).toBe(false);
  });

  it('re-render test', () => {
    const wrapper = shallow(<ComboBox {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({
      noBorder: true,
      isDisabled: false,
      selectedOption: {
        key: 0,
        label: "Select"
      },
      options: baseOptions,
      opened: false,
      onSelect: () => jest.fn(),
      size: 'base',
      scaled: true
    }, wrapper.state);

    expect(shouldUpdate).toBe(true);
  });
});
