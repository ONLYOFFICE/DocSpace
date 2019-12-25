import React from 'react';
import { mount } from 'enzyme';

import ModuleTile from './';

const baseProps = {
  title: "Documents",
  imageUrl: "./modules/documents240.png",
  link: "/products/files/",
  description: "Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed.",
  isPrimary: true
}

describe('<ModuleTile />', () => {
  it('renders without error', () => {
    const wrapper = mount(<ModuleTile {...baseProps} />);

    expect(wrapper).toExist();
  });

  it('accepts id', () => {
    const wrapper = mount(
      <ModuleTile {...baseProps} id="testId" />
    );

    expect(wrapper.prop('id')).toEqual('testId');
  });

  it('accepts className', () => {
    const wrapper = mount(
      <ModuleTile {...baseProps} className="test" />
    );

    expect(wrapper.prop('className')).toEqual('test');
  });

  it('accepts style', () => {
    const wrapper = mount(
      <ModuleTile {...baseProps} style={{ color: 'red' }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty('color', 'red');
  });

  it('added onClick prop', () => {
    const onClick = jest.fn();
    const wrapper = mount(
      <ModuleTile {...baseProps} onClick={onClick} />
    );

    expect(wrapper.prop('onClick')).toEqual(onClick);
  });

  it('not isPrimary prop', () => {
    const wrapper = mount(
      <ModuleTile {...baseProps} isPrimary={false} />
    );

    expect(wrapper.prop('isPrimary')).toEqual(false);
  });
});
