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
