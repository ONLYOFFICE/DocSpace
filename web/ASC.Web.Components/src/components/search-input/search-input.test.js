import React from 'react';
import { mount } from 'enzyme';
import SearchInput from '.';

const baseProps = {
  isNeedFilter: true,
  getFilterData: () => [{ key: 'filter-example', group: 'filter-example', label: 'example group', isHeader: true },
  { key: 'filter-example-test', group: 'filter-example', label: 'Test' }]
}

describe('<SearchInput />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <SearchInput {...baseProps} />
    );

    expect(wrapper).toExist();
  });

  it('middle size prop', () => {
    const wrapper = mount(
      <SearchInput {...baseProps} size="middle" />
    );

    expect(wrapper.prop('size')).toEqual('middle');
  });

  it('big size prop', () => {
    const wrapper = mount(
      <SearchInput {...baseProps} size="big" />
    );

    expect(wrapper.prop('size')).toEqual('big');
  });

  it('huge size prop', () => {
    const wrapper = mount(
      <SearchInput {...baseProps} size="huge" />
    );

    expect(wrapper.prop('size')).toEqual('huge');
  });

  it('accepts id', () => {
    const wrapper = mount(
      <SearchInput {...baseProps} id="testId" />
    );

    expect(wrapper.prop('id')).toEqual('testId');
  });

  it('accepts className', () => {
    const wrapper = mount(
      <SearchInput {...baseProps} className="test" />
    );

    expect(wrapper.prop('className')).toEqual('test');
  });

  it('accepts style', () => {
    const wrapper = mount(
      <SearchInput {...baseProps} style={{ color: 'red' }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty('color', 'red');
  });
});
