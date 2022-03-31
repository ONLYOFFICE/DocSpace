import React from "react";
import { mount } from "enzyme";
import renderer from "react-test-renderer";
import "jest-styled-components";
import PhoneInput from ".";
import {
  StyledDropDown,
  StyledCountryItem,
  StyledSearchPanel,
} from "./styled-phone-input";
import ThemeProvider from "../theme-provider";
import { Base } from "../themes";

const defaultProps = {
  locale: "RU",
  value: "12",
  onChange: jest.fn(),
};

jest.mock("react-text-mask", () => (props) => (
  <input type="text" {...{ ...props }} />
));

describe("<PhoneInput />", () => {
  it("component renders without error", () => {
    const wrapper = mount(<PhoneInput {...defaultProps} />);
    expect(wrapper).toExist();
  });

  it("input on change test", () => {
    const event = { target: { value: "value" } };
    const wrapper = mount(<PhoneInput {...defaultProps} />);
    const input = wrapper.find("input");
    input.simulate("change", event);
    expect(defaultProps.onChange).toHaveBeenCalled();
  });

  it("set search", () => {
    const wrapper = mount(<PhoneInput />);
    const searchInput = wrapper.find("TextInput");
    expect(searchInput).toExist();
  });

  it("check open prop", () => {
    const wrapper = mount(<PhoneInput open {...defaultProps} />);
    expect(wrapper.prop("open")).toEqual(true);
  });

  it("accepts locale", () => {
    const wrapper = mount(<PhoneInput locale="RU" />);
    expect(wrapper.prop("locale")).toEqual("RU");
  });

  it("accepts width", () => {
    const wrapper = mount(<PhoneInput width="304px" />);
    expect(wrapper.prop("width")).toEqual("304px");
  });

  it("accepts item hover color", () => {
    const wrapper = mount(<PhoneInput itemHoverColor="#fff" />);
    expect(wrapper.prop("itemHoverColor")).toEqual("#fff");
  });

  it("accepts item background color", () => {
    const wrapper = mount(<PhoneInput itemBackgroundColor="#fff" />);
    expect(wrapper.prop("itemBackgroundColor")).toEqual("#fff");
  });

  it("accepts placeholder color", () => {
    const wrapper = mount(<PhoneInput placeholderColor="#fff" />);
    expect(wrapper.prop("placeholderColor")).toEqual("#fff");
  });

  it("should applies theme props", () => {
    const phoneInput = renderer
      .create(
        <ThemeProvider theme={Base}>
          <PhoneInput {...defaultProps} />
        </ThemeProvider>
      )
      .toJSON();

    expect(phoneInput).toHaveStyleRule("width", Base.phoneInput.width);
  });

  it("should applies theme props to nested styled components", () => {
    const styledDropDown = renderer.create(<StyledDropDown />).toJSON();
    expect(styledDropDown).toHaveStyleRule("width", Base.phoneInput.width);

    const styledCountryItem = renderer.create(<StyledCountryItem />).toJSON();
    expect(styledCountryItem).toHaveStyleRule(
      "background-color",
      Base.phoneInput.itemBackgroundColor
    );

    const styledSearchPanel = renderer.create(<StyledSearchPanel />).toJSON();
    expect(styledSearchPanel).toHaveStyleRule(
      "color",
      Base.phoneInput.placeholderColor,
      {
        modifier: ".phone-input-searcher::placeholder",
      }
    );
  });

  it("should applies new theme", () => {
    const newTheme = Object.assign({}, Base, {
      phoneInput: {
        width: "100px",
      },
    });

    const phoneInput2 = renderer
      .create(
        <ThemeProvider theme={newTheme}>
          <PhoneInput {...defaultProps} />
        </ThemeProvider>
      )
      .toJSON();

    expect(phoneInput2).toHaveStyleRule("width", newTheme.phoneInput.width);
  });
});
