import React from "react";
import { mount, shallow } from "enzyme";
import { Weekdays, Days, Day } from "./sub-components/";
import NewCalendar from "./";

const baseCalendarProps = {
  isDisabled: false,
  themeColor: "#ED7309",
  selectedDate: new Date(),
  openToDate: new Date(),
  minDate: new Date("1970/01/01"),
  maxDate: new Date("3000/01/01"),
  locale: "en",
  onChange: () => jest.fn()
};

const baseWeekdaysProps = {
  optionsWeekdays: [
    { key: "en_0", value: "Mo", color: "" },
    { key: "en_1", value: "Tu", color: "" },
    { key: "en_2", value: "We", color: "" },
    { key: "en_3", value: "Th", color: "" },
    { key: "en_4", value: "Fr", color: "" },
    { key: "en_5", value: "Sa", color: "#A3A9AE" },
    { key: "en_6", value: "Su", color: "#A3A9AE" }
  ],
  size: "base"
};

const baseDaysProps = {
  optionsDays: [
    {
      className: "calendar-month_neighboringMonth",
      dayState: "prev",
      disableClass: null,
      value: 25
    },
    {
      className: "calendar-month_neighboringMonth",
      dayState: "prev",
      disableClass: null,
      value: 26
    }
  ],
  onDayClick: jest.fn,
  size: "base"
};

const baseDayProps = {
  day: {
    className: "calendar-month_neighboringMonth",
    dayState: "prev",
    disableClass: null,
    value: 26
  },

  onDayClick: jest.fn,
  size: "base"
};

describe("Weekdays tests:", () => {
  it("Weekdays renders without error", () => {
    const wrapper = mount(<Weekdays {...baseWeekdaysProps} />);
    expect(wrapper).toExist();
  });

  it("Weekdays not re-render test", () => {
    const wrapper = shallow(<Weekdays {...baseWeekdaysProps} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props);
    expect(shouldUpdate).toBe(false);
  });

  it("Weekdays property size passed", () => {
    const wrapper = mount(<Weekdays {...baseWeekdaysProps} size={"big"} />);
    expect(wrapper.prop("size")).toEqual("big");
  });
});

describe("Days tests:", () => {
  it("Days renders without error", () => {
    const wrapper = mount(<Days {...baseDaysProps} />);
    expect(wrapper).toExist();
  });

  it("Days not re-render test", () => {
    const wrapper = shallow(<Days {...baseDaysProps} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props);
    expect(shouldUpdate).toBe(false);
  });

  it("Days property size passed", () => {
    const wrapper = mount(<Days {...baseDaysProps} size={"big"} />);
    expect(wrapper.prop("size")).toEqual("big");
  });
  /*
  it("Days click event", () => {
    const mockCallBack = jest.fn();

    const button = shallow(<Days {...baseDaysProps} />);
    button.find("DayContent").simulate("click");
    expect(mockCallBack.mock.calls.length).toEqual(1);
  });
*/
});

describe("Day tests:", () => {
  it("Day renders without error", () => {
    const wrapper = mount(<Day {...baseDayProps} />);
    expect(wrapper).toExist();
  });

  it("Day not re-render test", () => {
    const wrapper = shallow(<Day {...baseDayProps} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props);
    expect(shouldUpdate).toBe(false);
  });

  it("Day property size passed", () => {
    const wrapper = mount(<Day {...baseDayProps} size={"big"} />);
    expect(wrapper.prop("size")).toEqual("big");
  });
});

describe("Calendar tests:", () => {
  it("Calendar renders without error", () => {
    const wrapper = mount(<NewCalendar {...baseCalendarProps} />);
    expect(wrapper).toExist();
  });

  /*
  it("Calendar has rendered content.", () => {
    const wrapper = mount(<NewCalendar {...baseCalendarProps} />);
    //expect(wrapper.find('span')).toExist();
    expect(wrapper.find("ul")).not.toExist();
  });
*/

  it("Calendar not re-render test", () => {
    const wrapper = shallow(<NewCalendar {...baseCalendarProps} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate(
      wrapper.props,
      wrapper.state
    );
    expect(shouldUpdate).toBe(false);
  });

  it("Calendar disabled when isDisabled is passed", () => {
    const wrapper = mount(
      <NewCalendar {...baseCalendarProps} isDisabled={true} />
    );
    expect(wrapper.prop("isDisabled")).toEqual(true);
  });
});
