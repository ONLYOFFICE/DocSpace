import React from 'react';
import { mount, shallow } from 'enzyme';
import Button from '.';

describe('<Button />', () => {
  it('renders without error', () => {
    const wrapper = mount(
      <Button size='base' isDisabled={false} onClick={() => alert('Button clicked')} label="OK" />
    );

    expect(wrapper).toExist();
  });

  it('not re-render test', () => {
    const onClick= () => alert('Button clicked');

    const wrapper = shallow(<Button size='base' isDisabled={false} onClick={onClick} label="OK" />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props);

    expect(shouldUpdate).toBe(false);
  });

  it('re-render test by value', () => {
    const onClick= () => alert('Button clicked');

    const wrapper = shallow(<Button size='base' isDisabled={false} onClick={onClick} label="OK" />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({
      ...wrapper.props,
      label: "Cancel"
    });

    expect(shouldUpdate).toBe(true);
  });
});
