import React from "react";
import { mount, shallow } from "enzyme";
import PasswordInput from ".";

const basePasswordSettings = {
  minLength: 6,
  upperCase: false,
  digits: false,
  specSymbols: false,
};

const baseProps = {
  inputName: "demoPasswordInput",
  emailInputName: "demoEmailInput",
  inputValue: "",
  tooltipPasswordTitle: "Password must contain:",
  tooltipPasswordLength: "from 6 to 30 characters",
  tooltipPasswordDigits: "digits",
  tooltipPasswordCapital: "capital letters",
  tooltipPasswordSpecial: "special characters (!@#$%^&*)",
  generatorSpecial: "!@#$%^&*",
  passwordSettings: basePasswordSettings,
  isDisabled: false,
  placeholder: "password",
  onChange: () => jest.fn(),
  onValidateInput: () => jest.fn(),
};

describe("<PasswordInput />", () => {
  it("renders without error", () => {
    const wrapper = mount(<PasswordInput {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("render password input", () => {
    const wrapper = mount(<PasswordInput {...baseProps} />);

    expect(wrapper.find("input").prop("type")).toEqual("password");
  });

  it("have an HTML name", () => {
    const wrapper = mount(<PasswordInput {...baseProps} />);

    expect(wrapper.find("input").prop("name")).toEqual("demoPasswordInput");
  });

  it("forward passed value", () => {
    const wrapper = mount(<PasswordInput {...baseProps} inputValue="demo" />);

    expect(wrapper.props().inputValue).toEqual("demo");
  });

  it("call onChange when changing value", () => {
    const onChange = jest.fn((event) => {
      expect(event.target.id).toEqual("demoPasswordInput");
      expect(event.target.name).toEqual("demoPasswordInput");
      expect(event.target.value).toEqual("demo");
    });

    const wrapper = mount(
      <PasswordInput
        {...baseProps}
        id="demoPasswordInput"
        name="demoPasswordInput"
        onChange={onChange}
      />
    );

    const event = { target: { value: "demo" } };

    wrapper.simulate("change", event);
  });

  it("call onFocus when input is focused", () => {
    const onFocus = jest.fn((event) => {
      expect(event.target.id).toEqual("demoPasswordInput");
      expect(event.target.name).toEqual("demoPasswordInput");
    });

    const wrapper = mount(
      <PasswordInput
        {...baseProps}
        id="demoPasswordInput"
        name="demoPasswordInput"
        onFocus={onFocus}
      />
    );

    wrapper.simulate("focus");
  });

  it("call onBlur when input loses focus", () => {
    const onBlur = jest.fn((event) => {
      expect(event.target.id).toEqual("demoPasswordInput");
      expect(event.target.name).toEqual("demoPasswordInput");
    });

    const wrapper = mount(
      <PasswordInput
        {...baseProps}
        id="demoPasswordInput"
        name="demoPasswordInput"
        onBlur={onBlur}
      />
    );

    wrapper.simulate("blur");
  });

  it("disabled when isDisabled is passed", () => {
    const wrapper = mount(<PasswordInput {...baseProps} isDisabled={true} />);

    expect(wrapper.prop("isDisabled")).toEqual(true);
  });

  it("not re-render test", () => {
    const wrapper = shallow(<PasswordInput {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(
      wrapper.props,
      wrapper.state
    );

    expect(shouldUpdate).toBe(false);
  });

  it("re-render test", () => {
    const wrapper = shallow(<PasswordInput {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(
      {
        inputName: "demoPasswordInput",
        emailInputName: "demoEmailInput",
        inputValue: "",
        tooltipPasswordTitle: "Password must contain:",
        tooltipPasswordLength: "from 6 to 30 characters",
        tooltipPasswordDigits: "digits",
        tooltipPasswordCapital: "capital letters",
        tooltipPasswordSpecial: "special characters (!@#$%^&*)",
        generatorSpecial: "!@#$%^&*",
        passwordSettings: {
          minLength: 8,
          upperCase: false,
          digits: false,
          specSymbols: false,
        },
        isDisabled: false,
        placeholder: "password",
        onChange: () => jest.fn(),
        onValidateInput: () => jest.fn(),
      },
      wrapper.state
    );

    expect(shouldUpdate).toBe(true);
  });

  it("generate password with props: 10 , false , false , false", () => {
    const newPasswordSettings = {
      minLength: 10,
      upperCase: false,
      digits: false,
      specSymbols: false,
    };

    const wrapper = shallow(
      <PasswordInput {...baseProps} passwordSettings={newPasswordSettings} />
    );
    const instance = wrapper.instance();

    instance.onGeneratePassword();

    expect(wrapper.state("type")).toBe("text");
  });

  it("generate password with props: 10 , true , false , false", () => {
    const newPasswordSettings = {
      minLength: 10,
      upperCase: true,
      digits: false,
      specSymbols: false,
    };

    const wrapper = shallow(
      <PasswordInput {...baseProps} passwordSettings={newPasswordSettings} />
    );
    const instance = wrapper.instance();

    instance.onGeneratePassword();

    expect(wrapper.state("type")).toBe("text");
  });

  it("generate password with props: 10 , true , true , false", () => {
    const newPasswordSettings = {
      minLength: 10,
      upperCase: true,
      digits: true,
      specSymbols: false,
    };

    const wrapper = shallow(
      <PasswordInput {...baseProps} passwordSettings={newPasswordSettings} />
    );
    const instance = wrapper.instance();

    instance.onGeneratePassword();

    expect(wrapper.state("type")).toBe("text");
  });

  it("generate password with props: 10 , true , true , true", () => {
    const newPasswordSettings = {
      minLength: 10,
      upperCase: true,
      digits: true,
      specSymbols: true,
    };

    const wrapper = shallow(
      <PasswordInput {...baseProps} passwordSettings={newPasswordSettings} />
    );
    const instance = wrapper.instance();

    instance.onGeneratePassword();

    expect(wrapper.state("type")).toBe("text");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <PasswordInput {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("accepts className", () => {
    const wrapper = mount(<PasswordInput {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("Tooltip disabled when isDisableTooltip is true", () => {
    const wrapper = mount(
      <PasswordInput {...baseProps} isDisableTooltip={true} />
    );

    expect(wrapper.prop("isDisableTooltip")).toEqual(true);
  });

  it("TextTooltip shown when isTextTooltipVisible is true", () => {
    const wrapper = mount(
      <PasswordInput {...baseProps} isTextTooltipVisible={true} />
    );

    expect(wrapper.prop("isTextTooltipVisible")).toEqual(true);
  });
});
