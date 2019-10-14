import React from 'react';
import { mount, shallow } from 'enzyme';
import EmailInput from '.';


const baseProps = {
  id: 'emailInputId',
  name: 'emailInputName',
  value: '',
  size: 'base',
  scale: false,
  isDisabled: false,
  isReadOnly: false,
  maxLength: 255,
  placeholder: 'email',
  onChange: () => jest.fn(),
  onValidateInput: () => jest.fn()
}

describe('<EmailInput />', () => {
  it('renders without error', () => {
    const wrapper = mount(<EmailInput {...baseProps} />);

    expect(wrapper).toExist();
  });

  it('re-render test', () => {
    const wrapper = shallow(<EmailInput {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({
      id: 'newEmailInputId',
      name: 'emailInputName',
      value: '',
      size: 'base',
      scale: false,
      isDisabled: false,
      isReadOnly: false,
      maxLength: 255,
      placeholder: 'email',
      onChange: () => jest.fn(),
      onValidateInput: () => jest.fn()
    }, wrapper.state);

    expect(shouldUpdate).toBe(true);
  });

});
