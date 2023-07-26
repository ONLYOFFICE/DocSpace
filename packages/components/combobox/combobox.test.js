import React from "react";
import { mount, shallow } from "enzyme";
import ComboBox from ".";
import DropDownItem from "../drop-down-item";

const baseOptions = [
  {
    key: 0,
    label: "Select",
  },
  {
    key: 1,
    label: "Select",
  },
  {
    key: 2,
    label: "Select",
  },
];

const advancedOptions = (
  <>
    <DropDownItem>
      <span>Some text</span>
    </DropDownItem>
  </>
);

const baseProps = {
  noBorder: false,
  isDisabled: false,
  selectedOption: {
    key: 0,
    icon: "CatalogFolderIcon",
    label: "Select",
    default: true,
  },
  options: baseOptions,
  opened: false,
  onSelect: () => jest.fn(),
  size: "base",
  scaled: true,
};

const toggleDisplayProps = {
  options: [],
  selectedOption: {
    key: 0,
    label: "Selected option",
  },
  scaled: false,
  size: "content",
  displayType: "toggle",
};

describe("<ComboBox />", () => {
  it("rendered without error", () => {
    const wrapper = mount(<ComboBox {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("with advanced options", () => {
    const wrapper = mount(
      <ComboBox {...baseProps} options={[]} advancedOptions={advancedOptions} />
    );

    expect(wrapper).toExist();
  });

  it("disabled", () => {
    const wrapper = mount(<ComboBox {...baseProps} isDisabled={true} />);

    expect(wrapper.prop("isDisabled")).toEqual(true);
  });

  it("without borders", () => {
    const wrapper = mount(<ComboBox {...baseProps} noBorder={true} />);

    expect(wrapper.prop("noBorder")).toEqual(true);
  });

  it("opened", () => {
    const wrapper = mount(<ComboBox {...baseProps} opened={true} />);

    expect(wrapper.prop("opened")).toEqual(true);
  });

  it("opened without borders", () => {
    const wrapper = mount(
      <ComboBox {...baseProps} opened={true} noBorder={true} />
    );

    expect(wrapper.prop("opened")).toEqual(true);
    expect(wrapper.prop("noBorder")).toEqual(true);
  });

  it("with DropDown max height", () => {
    const wrapper = mount(<ComboBox {...baseProps} dropDownMaxHeight={200} />);

    expect(wrapper.prop("dropDownMaxHeight")).toEqual(200);
  });

  it("without scaled", () => {
    const wrapper = mount(<ComboBox {...baseProps} scaled={false} />);

    expect(wrapper.prop("scaled")).toEqual(false);
  });

  it("scaled", () => {
    const wrapper = mount(<ComboBox {...baseProps} scaled={true} />);

    expect(wrapper.prop("scaled")).toEqual(true);
  });

  it("scaled options", () => {
    const wrapper = mount(<ComboBox {...baseProps} scaledOptions={true} />);

    expect(wrapper.prop("scaledOptions")).toEqual(true);
  });

  it("middle size options", () => {
    const wrapper = mount(
      <ComboBox {...baseProps} scaled={false} size="middle" />
    );

    expect(wrapper.prop("size")).toEqual("middle");
  });

  it("big size options", () => {
    const wrapper = mount(
      <ComboBox {...baseProps} scaled={false} size="big" />
    );

    expect(wrapper.prop("size")).toEqual("big");
  });

  it("huge size options", () => {
    const wrapper = mount(
      <ComboBox {...baseProps} scaled={false} size="huge" />
    );

    expect(wrapper.prop("size")).toEqual("huge");
  });

  it("by content size options", () => {
    const wrapper = mount(
      <ComboBox {...baseProps} scaled={false} size="content" />
    );

    expect(wrapper.prop("size")).toEqual("content");
  });

  it("with children node", () => {
    const wrapper = mount(
      <ComboBox {...baseProps}>
        <div>demo</div>
      </ComboBox>
    );

    expect(wrapper.contains(<div>demo</div>)).toBe(true);
  });

  it("not re-render", () => {
    const wrapper = shallow(<ComboBox {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(
      wrapper.props,
      wrapper.state
    );

    expect(shouldUpdate).toBe(false);
  });

  it("re-render", () => {
    const wrapper = shallow(<ComboBox {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(
      { opened: true },
      wrapper.state
    );

    expect(shouldUpdate).toBe(true);
  });

  it("causes function comboBoxClick() with disabled prop", () => {
    const wrapper = shallow(<ComboBox {...baseProps} isDisabled={true} />);
    const instance = wrapper.instance();

    instance.comboBoxClick();

    expect(wrapper.state("isOpen")).toBe(false);
  });

  it("causes function comboBoxClick()", () => {
    const wrapper = shallow(<ComboBox {...baseProps} />);
    const instance = wrapper.instance();

    instance.comboBoxClick();

    expect(wrapper.state("isOpen")).toBe(true);
  });

  it("causes function optionClick()", () => {
    const onSelect = jest.fn();
    const selectedOption = {
      key: 1,
      label: "Select",
    };
    const wrapper = shallow(
      <ComboBox {...baseProps} opened={true} onSelect={onSelect} />
    );
    const instance = wrapper.instance();

    instance.optionClick(selectedOption);

    expect(wrapper.state("isOpen")).toBe(false);
    expect(onSelect).toHaveBeenCalledWith(selectedOption);
  });

  it("causes function stopAction()", () => {
    const wrapper = mount(<ComboBox {...baseProps} />);
    const instance = wrapper.instance();

    instance.stopAction(new Event("click"));

    expect(wrapper.state("isOpen")).toBe(false);
  });

  it("causes function handleClick() with simulate", () => {
    const wrapper = mount(<ComboBox {...baseProps} opened={true} />);

    wrapper.simulate("click");

    expect(wrapper.state("isOpen")).toBe(false);
  });

  it("causes function handleClick() with simulate and ComboBox not opened", () => {
    const wrapper = mount(<ComboBox {...baseProps} />);

    wrapper.simulate("click");

    expect(wrapper.state("isOpen")).toBe(true);
  });

  it("componentDidUpdate() lifecycle test", () => {
    const wrapper = shallow(<ComboBox {...baseProps} />);
    const instance = wrapper.instance();
    const newSelected = { key: 1, label: "Select" };

    jest.spyOn(instance, "setIsOpen");

    wrapper.setProps({
      opened: true,
    });

    expect(wrapper.props().opened).toBe(true);

    wrapper.setProps({
      opened: false,
    });

    expect(wrapper.props().opened).toBe(false);

    instance.componentDidUpdate(
      {
        opened: true,
        selectedOption: newSelected,
      },
      {
        isOpen: true,
      }
    );

    instance.forceUpdate(); //Need for manual re-render, enzyme issue

    expect(instance.setIsOpen).toHaveBeenCalled();
  });

  it("accepts id", () => {
    const wrapper = mount(<ComboBox {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<ComboBox {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<ComboBox {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("render like toggle displayType", () => {
    const onToggleClick = jest.fn();
    const wrapper = mount(
      <ComboBox {...toggleDisplayProps} onToggle={onToggleClick} />
    );

    expect(wrapper.prop("displayType")).toEqual("toggle");
  });

  it("click on toggle", () => {
    const onToggleClick = jest.fn();
    const wrapper = mount(
      <ComboBox {...toggleDisplayProps} onToggle={onToggleClick} />
    );

    jest.spyOn(wrapper.instance(), "setIsOpen");

    wrapper.simulate("click");

    expect(onToggleClick).toHaveBeenCalled();
  });

  it("click outside", () => {
    const onToggleClick = jest.fn();
    const wrapper = mount(
      <ComboBox {...baseProps} opened onToggle={onToggleClick} />
    );
    const instance = wrapper.instance();

    jest.spyOn(instance, "handleClickOutside");

    instance.handleClickOutside(new Event("click")); //TODO: rework with simulation

    expect(wrapper.state("isOpen")).toBe(false);
    expect(wrapper.prop("opened")).toBe(true);
    expect(instance.handleClickOutside).toHaveBeenCalled();
    expect(onToggleClick).toHaveBeenCalled();
  });
});
