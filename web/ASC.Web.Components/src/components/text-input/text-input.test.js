import React from 'react';
import { mount, shallow } from 'enzyme';
import TextInput from '.';

describe('<TextInput />', () => {
  it('renders without error', () => {
    const wrapper = mount(     
      <TextInput value="text" onChange={event => alert(event.target.value)} />
    );

    expect(wrapper).toExist();
  });

  it('not re-render test', () => {
    const onChange= event => alert(event.target.value);

    const wrapper = shallow(<TextInput value="text" onChange={onChange} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props);

    expect(shouldUpdate).toBe(false);
  });

  it('re-render test by value', () => {
    const onChange= event => alert(event.target.value);

    const wrapper = shallow(<TextInput value="text" onChange={onChange} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({
      ...wrapper.props,
      value: "another text"
    });

    expect(shouldUpdate).toBe(true);
  });
});
