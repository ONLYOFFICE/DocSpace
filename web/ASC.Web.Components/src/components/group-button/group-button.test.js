import React from "react";
import { mount, shallow } from "enzyme";
import GroupButton from ".";

const baseProps = {
  label: "test",
  disabled: false,
  opened: false,
  isDropdown: false,
};

describe("<GroupButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(<GroupButton {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("renders with child", () => {
    const wrapper = mount(
      <GroupButton {...baseProps} isDropdown>
        <div key="demo" label="demo">
          Test
        </div>
      </GroupButton>
    );

    expect(wrapper).toExist();
    expect(wrapper.instance().props.isDropdown).toBe(true);
  });

  it("renders with child and open first item", () => {
    const wrapper = mount(
      <GroupButton {...baseProps} isDropdown opened>
        <div key="demo" label="demo">
          Test
        </div>
      </GroupButton>
    );

    expect(wrapper).toExist();
    expect(wrapper.instance().props.isDropdown).toBe(true);
    expect(wrapper.instance().props.opened).toBe(true);
  });

  it("applies disabled prop", () => {
    const wrapper = mount(<GroupButton {...baseProps} disabled={true} />);

    expect(wrapper.prop("disabled")).toEqual(true);
  });

  it("applies opened prop", () => {
    const wrapper = mount(<GroupButton {...baseProps} opened={true} />);

    expect(wrapper.prop("opened")).toEqual(true);
    expect(wrapper.state("isOpen")).toEqual(true);
  });

  it("applies isDropdown prop", () => {
    const wrapper = mount(<GroupButton {...baseProps} isDropdown={true} />);

    expect(wrapper.prop("isDropdown")).toEqual(true);
  });

  it("applies activated prop", () => {
    const wrapper = mount(<GroupButton {...baseProps} activated={true} />);

    expect(wrapper.prop("activated")).toEqual(true);
  });

  it("applies hovered prop", () => {
    const wrapper = mount(<GroupButton {...baseProps} hovered={true} />);

    expect(wrapper.prop("hovered")).toEqual(true);
  });

  it("applies isSeparator prop", () => {
    const wrapper = mount(<GroupButton {...baseProps} isSeparator={true} />);

    expect(wrapper.prop("isSeparator")).toEqual(true);
  });

  it("applies isSelect prop", () => {
    const wrapper = mount(<GroupButton {...baseProps} isSelect={true} />);

    expect(wrapper.prop("isSelect")).toEqual(true);
  });

  it("applies checked prop", () => {
    const wrapper = mount(<GroupButton {...baseProps} checked={true} />);

    expect(wrapper.prop("checked")).toEqual(true);
  });

  it("applies isIndeterminate prop", () => {
    const wrapper = mount(
      <GroupButton {...baseProps} isIndeterminate={true} />
    );

    expect(wrapper.prop("isIndeterminate")).toEqual(true);
  });

  it("applies dropDownMaxHeight prop", () => {
    const wrapper = mount(
      <GroupButton {...baseProps} dropDownMaxHeight={100} />
    );

    expect(wrapper.prop("dropDownMaxHeight")).toEqual(100);
  });

  it("applies id", () => {
    const wrapper = mount(<GroupButton {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("applies className", () => {
    const wrapper = mount(<GroupButton {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("applies style", () => {
    const wrapper = mount(
      <GroupButton {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("causes function dropDownItemClick()", () => {
    const onSelect = jest.fn();
    const child = <div data-index={0}>test</div>;
    const wrapper = shallow(
      <GroupButton
        {...baseProps}
        opened={true}
        isDropdown={true}
        onSelect={onSelect}
      >
        {child}
      </GroupButton>
    );
    const instance = wrapper.instance();

    instance.dropDownItemClick({
      currentTarget: {
        dataset: {
          index: 0,
        },
      },
    });

    expect(onSelect).toBeCalled();
    expect(wrapper.state("isOpen")).toBe(false);
  });

  it("causes function dropDownToggleClick()", () => {
    const child = <div>test</div>;
    const wrapper = shallow(
      <GroupButton {...baseProps} opened isDropdown>
        {child}
      </GroupButton>
    );
    const instance = wrapper.instance();

    instance.dropDownToggleClick();

    expect(wrapper.state("isOpen")).toBe(false);
  });

  it("causes function checkboxChange(e)", () => {
    const child = <div>test</div>;
    const onChange = jest.fn();
    const wrapper = mount(
      <GroupButton
        {...baseProps}
        opened
        isSelect
        isDropdown
        onChange={onChange}
        selected="test"
      >
        {child}
      </GroupButton>
    );
    const instance = wrapper.instance();
    const event = new Event("click", { target: { checked: true } });

    instance.checkboxChange(event);

    /*  TODO: Way of normal simulation
        wrapper.find('input[type="checkbox"]').simulate('change', {target:{checked:true}}) */

    expect(onChange).toBeCalled();
    expect(wrapper.state("selected")).toBe("test");
  });

  it("causes function clickOutsideAction(e)", () => {
    const child = <div>test</div>;
    const wrapper = mount(
      <GroupButton {...baseProps} opened isSelect isDropdown selected="test">
        {child}
      </GroupButton>
    );
    const instance = wrapper.instance();
    const event = new Event("click", { target: {} });

    instance.clickOutsideAction(event);

    expect(wrapper.state("selected")).toBe("test");
    expect(wrapper.state("isOpen")).toBe(false);
  });

  it("calling componentDidUpdate()", () => {
    const child = <div>test</div>;
    const wrapper = mount(
      <GroupButton {...baseProps} opened isSelect isDropdown selected="test">
        {child}
      </GroupButton>
    );

    expect(wrapper.state("selected")).toBe("test");
    expect(wrapper.state("isOpen")).toBe(true);

    wrapper.setProps({
      opened: false,
    });

    expect(wrapper.state("selected")).toBe("test");
    expect(wrapper.state("isOpen")).toBe(false);

    wrapper.setProps({
      selected: "new test",
    });

    expect(wrapper.state("selected")).toBe("new test");
    expect(wrapper.state("isOpen")).toBe(false);
  });
});
