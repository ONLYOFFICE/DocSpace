import React from "react";
import { mount, shallow } from "enzyme";
import DatePicker from "./";
import NewCalendar from "../calendar-new";
import InputBlock from "../input-block";
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

  it("DatePicker  has rendered content NewCalendar", () => {
    const wrapper = mount(<DatePicker inOpen={true} />);
    expect(wrapper).toExist(<NewCalendar />);
  });
  it("DatePicker has rendered content InputBlock", () => {
    const wrapper = mount(<DatePicker />);
    expect(wrapper).toExist(<InputBlock />);
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

  it("DatePicker themeColor test", () => {
    const wrapper = mount(<DatePicker themeColor={"#fff"} />);
    expect(wrapper.props().themeColor).toEqual("#fff");
  });

  it("DatePicker input mask test", () => {
    const mask = [/\d/, /\d/, "/", /\d/, /\d/, "/", /\d/, /\d/, /\d/, /\d/];
    const wrapper = mount(<InputBlock mask={mask} />);
    expect(wrapper.props().mask).toBe(mask);
    expect(wrapper.props().mask).toEqual(mask);
  });

  it("DatePicker locale test", () => {
    const wrapper = mount(<DatePicker locale={"en-GB"} />);
    expect(wrapper.prop("locale")).toEqual("en-GB");
  });

  it("DatePicker input value with locale test", () => {
    moment.locale("ru");
    const value = moment(selectedDate).format("L");
    const wrapper = mount(<DatePicker value={value} />);
    expect(wrapper.props().value).toEqual("12.09.2019");
  });

  it("DatePicker check the onChange callback", () => {
    const onChange = jest.fn();
    const props = {
      value: "03/03/2000",
      onChange
    };
    const wrapper = mount(<DatePicker {...props} />).find("input");
    wrapper.simulate("change", { target: { value: "09/09/2019" } });
    expect(onChange).toHaveBeenCalledWith(new Date("09/09/2019"));
  });
  
  it("check DatePicker popup open", () => {
    const onFocus = jest.fn(() => true);
    const wrapper = mount(<DatePicker onFocus={onFocus} isOpen={false} />)
    const input = wrapper.find(
      "input"
    );
    input.simulate("focus");

    const instance = wrapper.instance();
    expect(instance.state.isOpen).toEqual(true);
  });
});
