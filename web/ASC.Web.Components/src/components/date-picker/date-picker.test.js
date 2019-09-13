import React from "react";
import { mount, shallow } from "enzyme";
import DatePicker from "./";
import moment from "moment";

const selectedDate = new Date("09/12/2019");
const minDate = new Date("01/01/1970");
const maxDate = new Date("01/01/2020");
describe("DatePicker tests", () => {
  it("DatePicker renders without error", () => {
    const wrapper = mount(<DatePicker />);
    expect(wrapper).toExist();
  });

  it("DatePicker disabled when isDisabled is passed", () => {
    const wrapper = mount(<DatePicker isDisabled={true} />);
    expect(wrapper.prop("isDisabled")).toEqual(true);
  });

  it("DatePicker opened when inOpen is passed", () => {
    const wrapper = mount(<DatePicker inOpen={true} />);
    expect(wrapper.prop("inOpen")).toEqual(true);
  });

  it("DatePicker hasError is passed", () => {
    const wrapper = mount(<DatePicker hasError={true} />);
    expect(wrapper.prop("hasError")).toEqual(true);
  });

  it("DatePicker disabled when isReadOnly is passed", () => {
    const wrapper = mount(<DatePicker isReadOnly={true} />);
    expect(wrapper.prop("isReadOnly")).toEqual(true);
  });

  it("DatePicker minDate test", () => {
    const wrapper = mount(<DatePicker minDate={minDate} />);
    expect(wrapper.props().minDate).toEqual(minDate);
  });

  it("DatePicker maxDate test", () => {
    const wrapper = mount(<DatePicker maxDate={maxDate} />);
    expect(wrapper.props().maxDate).toEqual(maxDate);
  });

  it("DatePicker selectedDate test", () => {
    const wrapper = mount(<DatePicker selectedDate={selectedDate} />);
    expect(wrapper.props().selectedDate).toEqual(selectedDate);
  });

  it("DatePicker locale test", () => {
    const wrapper = mount(<DatePicker locale={"en-GB"} />);
    expect(wrapper.prop("locale")).toEqual("en-GB");
  });

  it("DatePicker themeColor test", () => {
    const wrapper = mount(<DatePicker themeColor={"#fff"} />);
    expect(wrapper.props().themeColor).toEqual("#fff");
  });

  it("DatePicker call onChange when changing value", () => {
    const onChange = jest.fn(event => {
      expect(event.target.value).toEqual("03/03/2000");
    });
    const event = { target: { value: moment("2000-03-03") } };
    const wrapper = mount(<DatePicker onChange={onChange} />);
    wrapper.simulate("change", event);
  });

  it("DatePicker test click event", () => {
    const onClick = jest.fn(event => {
      expect(event.inOpen).toEqual(true);
    });
    const wrapper = shallow(<DatePicker onClick={onClick} />);
    wrapper.simulate("click");
  });

  it("DatePicker onFocus test", () => {
    const onFocus = jest.fn;
    const wrapper = mount(<DatePicker onFocus={onFocus} />);
    wrapper.simulate("focus");
  });
});
