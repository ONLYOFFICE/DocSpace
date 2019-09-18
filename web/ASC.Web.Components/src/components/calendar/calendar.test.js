import React from "react";
import { mount, shallow, render } from "enzyme";
import { Weekdays, Days, Day } from "./sub-components/";
import Calendar from "./";
import ComboBox from "../combobox";
import moment from "moment";

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
  onDayClick: jest.fn(),
  size: "base"
};

const options = [
  { key: 0, value: "one" },
  { key: 1, value: "two" },
  { key: 2, value: "three" }
];
const baseComboBoxProps = {
  options: options,
  selectedOption: { key: 0, value: "one" }
};

const selectedDate = new Date("09/12/2019");
const openToDate = new Date("09/12/2019");
const minDate = new Date("01/01/1970");
const maxDate = new Date("01/01/2020");
const months = moment.months();

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

  it("Weekdays render test", () => {
    const wrapper = shallow(<Weekdays {...baseWeekdaysProps} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate({
      ...wrapper.props,
      size: "big"
    });
    expect(shouldUpdate).toBe(true);
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

  it("Days render test", () => {
    const wrapper = shallow(<Days {...baseDaysProps} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate({
      ...wrapper.props,
      size: "big"
    });
    expect(shouldUpdate).toBe(true);
  });

  it("Days property size passed", () => {
    const wrapper = mount(<Days {...baseDaysProps} size={"big"} />);
    expect(wrapper.prop("size")).toEqual("big");
  });
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

  it("Day render test", () => {
    const wrapper = shallow(<Day {...baseDayProps} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate({
      ...wrapper.props,
      size: "big"
    });
    expect(shouldUpdate).toBe(true);
  });

  it("Day property size passed", () => {
    const wrapper = mount(<Day {...baseDayProps} size={"big"} />);
    expect(wrapper.prop("size")).toEqual("big");
  });
});

describe("Calendar tests:", () => {
  it("Calendar renders without error", () => {
    const wrapper = mount(<Calendar {...baseCalendarProps} />);
    expect(wrapper).toExist();
  });

  it("Calendar selectedDate test", () => {
    const wrapper = mount(
      <Calendar {...baseCalendarProps} selectedDate={selectedDate} />
    );
    expect(wrapper.props().selectedDate).toEqual(selectedDate);
  });

  it("Calendar openToDate test", () => {
    const wrapper = mount(
      <Calendar {...baseCalendarProps} openToDate={openToDate} />
    );
    expect(wrapper.props().openToDate).toEqual(openToDate);
  });

  it("Calendar minDate test", () => {
    const wrapper = mount(
      <Calendar {...baseCalendarProps} minDate={minDate} />
    );
    expect(wrapper.props().minDate).toEqual(minDate);
  });

  it("Calendar maxDate test", () => {
    const wrapper = mount(
      <Calendar {...baseCalendarProps} maxDate={maxDate} />
    );
    expect(wrapper.props().maxDate).toEqual(maxDate);
  });

  it("Calendar themeColor test", () => {
    const wrapper = mount(
      <Calendar {...baseCalendarProps} themeColor={"#fff"} />
    );
    expect(wrapper.props().themeColor).toEqual("#fff");
  });

  it("Calendar locale test", () => {
    const wrapper = mount(
      <Calendar {...baseCalendarProps} locale={"en-GB"} />
    );
    expect(wrapper.prop("locale")).toEqual("en-GB");
  });

  it("Calendar disabled when isDisabled is passed", () => {
    const wrapper = mount(
      <Calendar {...baseCalendarProps} isDisabled={true} />
    );
    expect(wrapper.prop("isDisabled")).toEqual(true);
  });
  it("Calendar has rendered content ComboBox", () => {
    const wrapper = mount(<Calendar {...baseCalendarProps} />);
    expect(wrapper).toExist(<ComboBox {...baseComboBoxProps} />);
  });

  it("Calendar check the onChange callback", () => {
    const onChange = jest.fn();
    const props = {
      selectedDate: new Date("03/03/2000"),
      onChange
    };
    const wrapper = shallow(<Calendar {...props} />).instance();
    wrapper.onDayClick({
      value: 1,
      disableClass: "",
      className: "",
      dayState: ""
    });

    expect(onChange).toBeCalled();
  });

  it("Calendar check onSelectYear function", () => {
    const props = {
      openToDate: new Date("05/01/2000"),
      selectedDate: new Date("01/01/2000"),
      minDate: new Date("01/01/1970"),
      maxDate: new Date("01/01/2020")
    };

    const wrapper = shallow(<Calendar {...props} />).instance();
    wrapper.onSelectYear({
      key: 2020,
      value: 2020
    });

    expect(wrapper.state.openToDate).toEqual(new Date("01/01/2020"));
  });

  it("Calendar check onSelectMonth function", () => {
    const props = {
      openToDate: new Date("01/01/2000"),
      selectedDate: new Date("01/01/2000")
    };
    const wrapper = shallow(<Calendar {...props} />).instance();
    wrapper.onSelectMonth({ key: "1", label: "February", disabled: false });

    expect(wrapper.state.openToDate).toEqual(new Date("02/01/2000"));
  });

  it("Calendar check getDays function", () => {
    const props = {
      openToDate: new Date("01/01/2000"),
      selectedDate: new Date("01/01/2000")
    };
    const wrapper = shallow(<Calendar {...props} />).instance();
    wrapper.getDays(minDate, maxDate, openToDate, selectedDate);
    expect(wrapper.state.openToDate).toEqual(new Date("02/01/2000"));
  });
 
});
