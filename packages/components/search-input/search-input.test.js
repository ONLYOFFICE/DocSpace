import React from "react";
import { mount, shallow } from "enzyme";
import SearchInput from ".";
import InputBlock from "../input-block";

const baseProps = {
  isNeedFilter: true,
  value: "",
  getFilterData: () => [
    {
      key: "filter-example",
      group: "filter-example",
      label: "example group",
      isHeader: true,
    },
    { key: "filter-example-test", group: "filter-example", label: "Test" },
  ],
};

describe("<SearchInput />", () => {
  it("renders without error", () => {
    const wrapper = mount(<SearchInput {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("middle size prop", () => {
    const wrapper = mount(<SearchInput {...baseProps} size="middle" />);

    expect(wrapper.prop("size")).toEqual("middle");
  });

  it("big size prop", () => {
    const wrapper = mount(<SearchInput {...baseProps} size="big" />);

    expect(wrapper.prop("size")).toEqual("big");
  });

  it("huge size prop", () => {
    const wrapper = mount(<SearchInput {...baseProps} size="huge" />);

    expect(wrapper.prop("size")).toEqual("huge");
  });

  it("accepts id", () => {
    const wrapper = mount(<SearchInput {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<SearchInput {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <SearchInput {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
  // TODO: Fix icons tests
  /*it("call onClearSearch", () => {
    const onClearSearch = jest.fn();
    const onChange = jest.fn();
    const wrapper = mount(
      <SearchInput
        {...baseProps}
        onClearSearch={onClearSearch}
        onChange={onChange}
      />
    );

    wrapper
      .find("input")
      .first()
      .simulate("change", { target: { value: "test" } });

    const icon = wrapper.find(".append div");
    icon.first().simulate("click");
    expect(onClearSearch).toHaveBeenCalled();
  });
  it("not call onClearSearch", () => {
    const onClearSearch = jest.fn();
    const onChange = jest.fn();
    const wrapper = mount(<SearchInput {...baseProps} onChange={onChange} />);

    wrapper
      .find("input")
      .first()
      .simulate("change", { target: { value: "test" } });

    const icon = wrapper.find(".append div");
    icon.first().simulate("click");
    expect(onClearSearch).not.toHaveBeenCalled();
  });
  it("componentDidUpdate() props lifecycle test", () => {
    const wrapper = shallow(<SearchInput {...baseProps} />);
    const instance = wrapper.instance();

    instance.componentDidUpdate(
      {
        opened: true,
        selectedOption: {
          value: "test",
        },
      },
      wrapper.state()
    );

    expect(wrapper.props()).toBe(wrapper.props());
  });
  it("not call setSearchTimer", (done) => {
    const onChange = jest.fn();
    const wrapper = mount(
      <SearchInput {...baseProps} autoRefresh={false} onChange={onChange} />
    );

    const input = wrapper.find("input");
    input.first().simulate("change", { target: { value: "test" } });

    setTimeout(() => {
      expect(onChange).not.toHaveBeenCalled();
      done();
    }, 1000);
  });
  it("call setSearchTimer", (done) => {
    const onChange = jest.fn();
    const wrapper = mount(<SearchInput {...baseProps} onChange={onChange} />);

    const instance = wrapper.instance();
    instance.setSearchTimer("test");

    setTimeout(() => {
      expect(onChange).toHaveBeenCalled();
      done();
    }, 1000);
  });
  it("test icon button size. base size prop", () => {
    const wrapper = mount(<SearchInput {...baseProps} size="base" />);

    wrapper
      .find("input")
      .first()
      .simulate("change", { target: { value: "test" } });

    const inputBlock = wrapper.find(InputBlock);
    expect(inputBlock.prop("iconSize")).toEqual(12);
  });
  it("test icon button size. middle size prop", () => {
    const wrapper = mount(<SearchInput {...baseProps} size="middle" />);

    wrapper
      .find("input")
      .first()
      .simulate("change", { target: { value: "test" } });

    const inputBlock = wrapper.find(InputBlock);
    expect(inputBlock.prop("iconSize")).toEqual(16);
  });
  it("test icon button size. big size prop", () => {
    const wrapper = mount(<SearchInput {...baseProps} size="big" />);

    wrapper
      .find("input")
      .first()
      .simulate("change", { target: { value: "test" } });

    const inputBlock = wrapper.find(InputBlock);
    expect(inputBlock.prop("iconSize")).toEqual(19);
  });
  it("test icon button size. huge size prop", () => {
    const wrapper = mount(<SearchInput {...baseProps} size="huge" />);

    wrapper
      .find("input")
      .first()
      .simulate("change", { target: { value: "test" } });

    const inputBlock = wrapper.find(InputBlock);
    expect(inputBlock.prop("iconSize")).toEqual(22);
  });*/
});
