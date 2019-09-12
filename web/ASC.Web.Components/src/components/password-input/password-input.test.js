import React from 'react';
import { mount } from 'enzyme';
import PasswordInput from '.';

const basePasswordSettings = {
  minLength: 6,
  upperCase: false,
  digits: false,
  specSymbols: false
};

const baseProps = {
  inputName: 'demoPasswordInput',
  emailInputName: 'demoEmailInput',
  inputValue: '',
  clipActionResource: 'Copy e-mail and password',
  clipEmailResource: 'E-mail: ',
  clipPasswordResource: 'Password: ',
  tooltipPasswordTitle: 'Password must contain:',
  tooltipPasswordLength: 'from 6 to 30 characters',
  tooltipPasswordDigits: 'digits',
  tooltipPasswordCapital: 'capital letters',
  tooltipPasswordSpecial: 'special characters (!@#$%^&*)',
  generatorSpecial: '!@#$%^&*',
  passwordSettings: basePasswordSettings,
  isDisabled: false,
  placeholder: 'password',
  onChange: () => jest.fn(),
  onValidateInput: () => jest.fn(),
  onCopyToClipboard: () => jest.fn()
}

describe('<PasswordInput />', () => {
  it('renders without error', () => {
    const wrapper = mount(<PasswordInput {...baseProps} />);

    expect(wrapper).toExist();
  });

  it('render password input', () => {
    const wrapper = mount(<PasswordInput {...baseProps} />);

    expect(wrapper.find('input').prop('type')).toEqual('password');
  });

  it('have an HTML name', () => {
    const wrapper = mount(<PasswordInput {...baseProps} />);

    expect(wrapper.find('input').prop('name')).toEqual('demoPasswordInput');
  });

  it('forward passed value', () => {
    const wrapper = mount(<PasswordInput {...baseProps} inputValue='demo' />);

    expect(wrapper.props().inputValue).toEqual('demo');
  });

  it('call onChange when changing value', () => {
    const onChange = jest.fn(event => {
      expect(event.target.id).toEqual('demoPasswordInput');
      expect(event.target.name).toEqual('demoPasswordInput');
      expect(event.target.value).toEqual('demo');
    });

    const wrapper = mount(<PasswordInput {...baseProps} id="demoPasswordInput" name="demoPasswordInput" onChange={onChange} />);

    const event = { target: { value: "demo" } };

    wrapper.simulate('change', event);
  });

  it('call onFocus when input is focused', () => {
    const onFocus = jest.fn(event => {
      expect(event.target.id).toEqual('demoPasswordInput');
      expect(event.target.name).toEqual('demoPasswordInput');
    });

    const wrapper = mount(<PasswordInput {...baseProps} id="demoPasswordInput" name="demoPasswordInput" onFocus={onFocus} />);

    wrapper.simulate('focus');
  });

  it('call onBlur when input loses focus', () => {
    const onBlur = jest.fn(event => {
      expect(event.target.id).toEqual('demoPasswordInput');
      expect(event.target.name).toEqual('demoPasswordInput');
    });

    const wrapper = mount(<PasswordInput {...baseProps} id="demoPasswordInput" name="demoPasswordInput" onBlur={onBlur} />);

    wrapper.simulate('blur');
  });

  it('disabled when isDisabled is passed', () => {
    const wrapper = mount(<PasswordInput {...baseProps} isDisabled={true} />);

    expect(wrapper.prop('isDisabled')).toEqual(true);
  });
});
