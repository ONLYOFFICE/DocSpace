import React from "react";
import { mount, shallow } from "enzyme";
import ModalDialog from ".";

describe("<ModalDialog />", () => {
  it("renders without error", () => {
    const wrapper = mount(<ModalDialog visible={false} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<ModalDialog id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<ModalDialog className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<ModalDialog style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("modal displayType prop", () => {
    const wrapper = mount(<ModalDialog displayType="modal" />);

    expect(wrapper.prop("displayType")).toEqual("modal");
  });

  it("aside displayType prop", () => {
    const wrapper = mount(<ModalDialog displayType="aside" />);

    expect(wrapper.prop("displayType")).toEqual("aside");
  });

  it("exist scale prop", () => {
    const wrapper = mount(<ModalDialog scale />);

    expect(wrapper.prop("scale")).toEqual(true);
  });

  it("componentWillUnmount() lifecycle  test", () => {
    const wrapper = mount(<ModalDialog />);
    const componentWillUnmount = jest.spyOn(
      wrapper.instance(),
      "componentWillUnmount"
    );

    wrapper.unmount();
    expect(componentWillUnmount).toHaveBeenCalled();
  });

  it("componentDidUpdate() state lifecycle test", () => {
    const wrapper = shallow(<ModalDialog displayType="aside" visible />);
    const instance = wrapper.instance();

    instance.componentDidUpdate(wrapper.props(), wrapper.state());

    expect(wrapper.state()).toBe(wrapper.state());
  });

  it("call popstate()", () => {
    const onClose = jest.fn();
    const wrapper = shallow(<ModalDialog onClose={onClose} />);
    const instance = wrapper.instance();

    instance.popstate();

    expect(onClose).toBeCalled();
  });

  it("call resize()", () => {
    const wrapper = shallow(<ModalDialog />);
    const instance = wrapper.instance();

    instance.resize();

    expect(wrapper.state("displayType")).toEqual("aside");
  });
});
