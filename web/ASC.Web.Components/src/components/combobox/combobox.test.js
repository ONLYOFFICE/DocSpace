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
    icon: 'CatalogFolderIcon',
    label: "Select"
  },
  options: baseOptions,
  opened: false,
  onSelect: () => jest.fn(),
  size: 'base',
  dropDownMaxHeight: 200,
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

  it('no border when noBorder is passed', () => {
    const wrapper = mount(<ComboBox {...baseProps} noBorder={true} />);

    expect(wrapper.prop('noBorder')).toEqual(true);
  });

  it('opened when opened is passed', () => {
    const wrapper = mount(<ComboBox {...baseProps} opened={true} />);

    expect(wrapper.prop('opened')).toEqual(true);
  });

  it('not scaled button', () => {
    const wrapper = mount(<ComboBox {...baseProps} scaled={false} />);

    expect(wrapper.prop('scaled')).toEqual(false);
  });

  it('scaled button', () => {
    const wrapper = mount(<ComboBox {...baseProps} scaled={true} />);

    expect(wrapper.prop('scaled')).toEqual(true);
  });

  it('scaled options', () => {
    const wrapper = mount(<ComboBox {...baseProps} scaledOptions={true} />);

    expect(wrapper.prop('scaledOptions')).toEqual(true);
  });

  it('middle size options', () => {
    const wrapper = mount(<ComboBox {...baseProps} size='middle' />);

    expect(wrapper.prop('size')).toEqual('middle');
  });

  it('big size options', () => {
    const wrapper = mount(<ComboBox {...baseProps} size='big' />);

    expect(wrapper.prop('size')).toEqual('big');
  });

  it('huge size options', () => {
    const wrapper = mount(<ComboBox {...baseProps} size='huge' />);

    expect(wrapper.prop('size')).toEqual('huge');
  });

  it('content size options', () => {
    const wrapper = mount(<ComboBox {...baseProps} size='content' />);

    expect(wrapper.prop('size')).toEqual('content');
  });

  it('with children node', () => {
    const wrapper = mount(<ComboBox {...baseProps} ><div>demo</div></ComboBox>);

    expect(wrapper.contains(<div>demo</div>)).toBe(true)
  });

  it('not re-render test', () => {
    const wrapper = shallow(<ComboBox {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props, wrapper.state);

    expect(shouldUpdate).toBe(false);
  });

  it('re-render test', () => {
    const wrapper = shallow(<ComboBox {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({ noBorder: true }, wrapper.state);

    expect(shouldUpdate).toBe(true);
  });
});
