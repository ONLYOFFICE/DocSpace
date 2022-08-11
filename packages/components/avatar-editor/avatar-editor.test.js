import React from "react";
import { mount, shallow } from "enzyme";
import AvatarEditor from ".";

const baseProps = {
  visible: true,
  headerLabel: "test",
  selectNewPhotoLabel: "test",
  orDropFileHere: "test",
  saveButtonLabel: "test",
  maxSizeFileError: "test",
  image: "",
  maxSize: 1,
  accept: ["image/png", "image/jpeg"],
  unknownTypeError: "test",
  unknownError: "test",
  displayType: "auto",
};

describe("<AvatarEditor />", () => {
  it("renders without error", () => {
    const wrapper = mount(<AvatarEditor {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<AvatarEditor {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<AvatarEditor {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <AvatarEditor {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("componentDidUpdate() props lifecycle test", () => {
    const wrapper = shallow(<AvatarEditor {...baseProps} />);
    const instance = wrapper.instance();

    instance.componentDidUpdate({ visible: false }, wrapper.state());

    instance.componentDidUpdate({ visible: true }, wrapper.state());

    expect(wrapper.props()).toBe(wrapper.props());
  });

  it("causes function onClose()", () => {
    const onClose = jest.fn();
    const wrapper = mount(<AvatarEditor {...baseProps} onClose={onClose} />);
    const instance = wrapper.instance();

    instance.onClose();

    expect(wrapper.state("visible")).toBe(false);
  });

  it("causes function onSaveButtonClick()", () => {
    const onSave = jest.fn();
    const wrapper = mount(<AvatarEditor {...baseProps} onSave={onSave} />);
    const instance = wrapper.instance();

    wrapper.setState({ existImage: false });

    instance.onSaveButtonClick();

    wrapper.setState({ existImage: true });

    instance.onSaveButtonClick();

    expect(wrapper.state("visible")).toBe(true);
  });

  it("causes function onImageChange()", () => {
    const fileString = "";
    const onImageChange = jest.fn();
    const wrapper = mount(
      <AvatarEditor {...baseProps} onImageChange={onImageChange} />
    );
    const instance = wrapper.instance();

    instance.onImageChange(fileString);

    expect(onImageChange).toHaveBeenCalled();
  });

  it("causes function onImageChange() no onImageChange", () => {
    const fileString = "";
    const wrapper = mount(<AvatarEditor {...baseProps} />);
    const instance = wrapper.instance();

    instance.onImageChange(fileString);

    expect(wrapper.state("croppedImage")).toBe(fileString);
  });

  it("causes function onDeleteImage()", () => {
    const onDeleteImage = jest.fn();
    const wrapper = mount(
      <AvatarEditor {...baseProps} onDeleteImage={onDeleteImage} />
    );
    const instance = wrapper.instance();

    instance.onDeleteImage();

    expect(onDeleteImage).toHaveBeenCalled();
  });

  it("causes function onDeleteImage() no onDeleteImage", () => {
    const wrapper = mount(<AvatarEditor {...baseProps} />);
    const instance = wrapper.instance();

    instance.onDeleteImage();

    expect(wrapper.state("existImage")).toBe(false);
  });

  it("causes function onPositionChange()", () => {
    const data = { test: "test" };
    const wrapper = mount(<AvatarEditor {...baseProps} />);
    const instance = wrapper.instance();

    instance.onPositionChange(data);

    expect(wrapper.state("test")).toBe("test");
  });

  it("causes function onLoadFileError()", () => {
    const onLoadFileError = jest.fn();
    const wrapper = mount(
      <AvatarEditor {...baseProps} onLoadFileError={onLoadFileError} />
    );
    const instance = wrapper.instance();

    instance.onLoadFileError();

    expect(onLoadFileError).toHaveBeenCalled();
  });

  it("causes function onLoadFileError() no onLoadFileError", () => {
    const wrapper = mount(<AvatarEditor {...baseProps} />);
    const instance = wrapper.instance();

    instance.onLoadFileError();

    expect(wrapper.state("existImage")).toBe(false);
  });

  it("causes function onLoadFile()", () => {
    const file = "test";
    const onLoadFile = jest.fn();
    const wrapper = mount(
      <AvatarEditor {...baseProps} onLoadFile={onLoadFile} />
    );
    const instance = wrapper.instance();

    instance.onLoadFile(file);

    expect(onLoadFile).toHaveBeenCalled();
    expect(wrapper.state("existImage")).toBe(true);
  });

  it("causes function onLoadFile() no onLoadFile", () => {
    const wrapper = mount(<AvatarEditor {...baseProps} />);
    const instance = wrapper.instance();

    instance.onLoadFile();

    expect(wrapper.state("existImage")).toBe(true);
  });
});
