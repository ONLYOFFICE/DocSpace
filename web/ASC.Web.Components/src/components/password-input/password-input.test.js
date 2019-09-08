import React from 'react';
import { mount } from 'enzyme';
import PasswordInput from '.';

describe('<PasswordInput />', () => {
  it('renders without error', () => {
    const settings = {
      minLength: 6,
      upperCase: false,
      digits: false,
      specSymbols: false
    };

    const wrapper = mount(     
      <PasswordInput
        inputName="demoPasswordInput"
        emailInputName="demoEmailInput"
        inputValue={""}
        onChange={e => {
          console.log(e.target.value);
        }}
        clipActionResource="Copy e-mail and password"
        clipEmailResource="E-mail: "
        clipPasswordResource="Password: "
        tooltipPasswordTitle="Password must contain:"
        tooltipPasswordLength="from 6 to 30 characters"
        tooltipPasswordDigits="digits"
        tooltipPasswordCapital="capital letters"
        tooltipPasswordSpecial="special characters (!@#$%^&*)"
        generatorSpecial="!@#$%^&*"
        passwordSettings={settings}
        isDisabled={false}
        placeholder="password"
        onValidateInput={a => console.log(a)}
        onCopyToClipboard={b => console.log("Data " + b + " copied to clipboard")}
      />
    );

    expect(wrapper).toExist();
  });
});
