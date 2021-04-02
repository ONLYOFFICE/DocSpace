import React from "react";
import { mount, shallow } from "enzyme";
import Row from ".";

const baseProps = {
  checked: false,
  element: <span>1</span>,
  contextOptions: [{ key: "1", label: "test" }],
  children: <span>Some text</span>,
};

describe("<Row />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Row {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("call changeCheckbox(e)", () => {
    const onSelect = jest.fn();
    const wrapper = shallow(
      <Row
        {...baseProps}
        onChange={onSelect}
        onSelect={onSelect}
        data={{ test: "test" }}
      />
    );

    wrapper.simulate("change", { target: { checked: true } });

    expect(onSelect).toHaveBeenCalled();
  });

  it("renders with children", () => {
    const wrapper = mount(<Row {...baseProps} />);

    expect(wrapper).toHaveProp("children", baseProps.children);
  });

  it("renders with contentElement and sectionWidth", () => {
    const element = <div>content</div>;
    const wrapper = mount(
      <Row {...baseProps} contentElement={element} sectionWidth={600} />
    );

    expect(wrapper).toHaveProp("contentElement", element);
  });

  it("can apply contextButtonSpacerWidth", () => {
    const test = "10px";
    const wrapper = mount(
      <Row {...baseProps} contextButtonSpacerWidth={test} />
    );

    expect(wrapper).toHaveProp("contextButtonSpacerWidth", test);
  });

  it("can apply data property", () => {
    const test = { test: "test" };
    const wrapper = mount(<Row {...baseProps} data={test} />);

    expect(wrapper).toHaveProp("data", test);
  });

  it("can apply indeterminate", () => {
    const test = true;
    const wrapper = mount(<Row {...baseProps} indeterminate={test} />);

    expect(wrapper).toHaveProp("indeterminate", test);
  });

  it("accepts id", () => {
    const wrapper = mount(<Row {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Row {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Row {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
