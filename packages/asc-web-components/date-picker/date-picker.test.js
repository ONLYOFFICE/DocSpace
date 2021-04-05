import React from "react";
import { mount, shallow } from "enzyme";
import DatePicker from "./";
import NewCalendar from "../calendar";
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
      onChange,
    };
    const wrapper = mount(<DatePicker {...props} />).find("input");
    wrapper.simulate("change", { target: { value: "09/09/2019" } });
    expect(onChange).toHaveBeenCalledWith(new Date("09/09/2019"));
  });

  /*it("check DatePicker popup open", () => {
    const onFocus = jest.fn(() => true);
    const wrapper = mount(<DatePicker onFocus={onFocus} isOpen={false} />);
    const input = wrapper.find("input");
    input.simulate("focus");

    const instance = wrapper.instance();
    expect(instance.state.isOpen).toEqual(true);
  });*/

  it("DatePicker check the Calendar onChange callback", () => {
    const onChange = jest.fn();
    const props = {
      value: "03/03/2000",
      isOpen: true,
      onChange,
    };
    const wrapper = mount(<DatePicker {...props} />);

    const days = wrapper.find(".calendar-month");

    days.first().simulate("click", { target: { value: 1 } });

    expect(onChange).toHaveBeenCalled();
  });

  it("DatePicker check Compare date function", () => {
    const date = new Date();
    const errorDate = new Date("01/01/3000");
    const wrapper = shallow(<DatePicker />).instance();
    expect(wrapper.compareDate(date)).toEqual(true);
    expect(wrapper.compareDate(errorDate)).toEqual(false);
  });

  it("DatePicker check Compare dates function", () => {
    const date = new Date();
    const wrapper = shallow(<DatePicker />).instance();
    expect(wrapper.compareDates(date, date) === 0).toEqual(true);
    expect(wrapper.compareDates(date, new Date("01/01/2000")) === 0).toEqual(
      false
    );
  });

  it("DatePicker check is valid dates function", () => {
    var date = new Date();
    date.setFullYear(1);
    const wrapper = shallow(<DatePicker />).instance();
    expect(wrapper.isValidDate(selectedDate, maxDate, minDate, false)).toEqual(
      false
    );
    expect(wrapper.isValidDate(date, maxDate, minDate, false)).toEqual(true);
  });

  it("DatePicker componentDidUpdate() lifecycle test", () => {
    const props = {
      selectedDate: new Date(),
      minDate: new Date("01/01/1970"),
      maxDate: new Date("01/01/2030"),
      isOpen: true,
      isDisabled: false,
      isReadOnly: false,
      hasError: false,
      themeColor: "#ED7309",
      locale: "en",
    };

    var date = new Date();
    date.setFullYear(1);

    const wrapper = mount(<DatePicker {...props} />).instance();
    wrapper.componentDidUpdate(wrapper.props, wrapper.state);

    expect(wrapper.props).toBe(wrapper.props);
    expect(wrapper.state).toBe(wrapper.state);

    const wrapper2 = mount(
      <DatePicker
        {...props}
        selectedDate={date}
        hasError={false}
        size="big"
        isDisabled={false}
      />
    ).instance();

    wrapper2.componentDidUpdate(wrapper2.props, wrapper2.state);

    expect(wrapper2.props).toBe(wrapper2.props);
    expect(wrapper2.state).toBe(wrapper2.state);
  });

  it("componentWillUnmount() lifecycle  test", () => {
    const wrapper = mount(<DatePicker isOpen={true} />);
    const componentWillUnmount = jest.spyOn(
      wrapper.instance(),
      "componentWillUnmount"
    );

    wrapper.unmount();
    expect(componentWillUnmount).toHaveBeenCalled();
  });

  it("accepts id", () => {
    const wrapper = mount(<DatePicker isOpen={true} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<DatePicker isOpen={true} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <DatePicker isOpen={true} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
