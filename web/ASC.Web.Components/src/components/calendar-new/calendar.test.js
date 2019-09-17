import React from "react";
import { mount, shallow } from "enzyme";
import { Weekdays, Days, Day } from "./sub-components/";
import NewCalendar from "./";
import ComboBox from "../combobox";

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
  it("Calendar not re-render test", () => {
    const wrapper = shallow(<NewCalendar {...baseCalendarProps} />).instance();
    const shouldUpdate = wrapper.shouldComponentUpdate(
      wrapper.props,
      wrapper.state
    );
    expect(shouldUpdate).toBe(false);
  });
*/

  it("Calendar selectedDate test", () => {
    const wrapper = mount(
      <NewCalendar {...baseCalendarProps} selectedDate={selectedDate} />
    );
    expect(wrapper.props().selectedDate).toEqual(selectedDate);
  });

  it("Calendar openToDate test", () => {
    const wrapper = mount(
      <NewCalendar {...baseCalendarProps} openToDate={openToDate} />
    );
    expect(wrapper.props().openToDate).toEqual(openToDate);
  });

  it("Calendar minDate test", () => {
    const wrapper = mount(
      <NewCalendar {...baseCalendarProps} minDate={minDate} />
    );
    expect(wrapper.props().minDate).toEqual(minDate);
  });

  it("Calendar maxDate test", () => {
    const wrapper = mount(
      <NewCalendar {...baseCalendarProps} maxDate={maxDate} />
    );
    expect(wrapper.props().maxDate).toEqual(maxDate);
  });

  it("Calendar themeColor test", () => {
    const wrapper = mount(
      <NewCalendar {...baseCalendarProps} themeColor={"#fff"} />
    );
    expect(wrapper.props().themeColor).toEqual("#fff");
  });

  it("Calendar locale test", () => {
    const wrapper = mount(
      <NewCalendar {...baseCalendarProps} locale={"en-GB"} />
    );
    expect(wrapper.prop("locale")).toEqual("en-GB");
  });

  it("Calendar disabled when isDisabled is passed", () => {
    const wrapper = mount(
      <NewCalendar {...baseCalendarProps} isDisabled={true} />
    );
    expect(wrapper.prop("isDisabled")).toEqual(true);
  });
  it("Calendar has rendered content ComboBox", () => {
    const wrapper = mount(<NewCalendar {...baseCalendarProps} />);
    expect(wrapper).toExist(<ComboBox {...baseComboBoxProps} />);
  });

  /*
  it("Calendar check the onChange callback", () => {
    const onChange = jest.fn();
    const props = {
      selectedDate: new Date("03/03/2000"),
      onChange
    };
    const wrapper = shallow(<NewCalendar {...props} />);
    //expect(<NewCalendar {...props} />).toMatchSnapshot();
    wrapper.simulate("change", {
      selectedDate: { date: new Date("09/09/2019") }
    });
    expect(onChange).toHaveBeenCalledWith(new Date("09/09/2019"));
    //expect(<NewCalendar {...props} />).toMatchSnapshot();
    //expect(onChange).toBeCalled();
  });
  */
});

//expect(<NewCalendar />).toMatchSnapshot();
