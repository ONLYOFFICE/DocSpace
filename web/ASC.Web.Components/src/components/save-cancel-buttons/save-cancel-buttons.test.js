import React from "react";
import { mount } from "enzyme";
import SaveCancelButtons from ".";

describe("<MainButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <SaveCancelButtons
        showReminder={true}
        reminderTest="You have unsaved changes"
        saveButtonLabel="Save"
        cancelButtonLabel="Cancel"
      />
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(
      <SaveCancelButtons
        showReminder={true}
        reminderTest="You have unsaved changes"
        saveButtonLabel="Save"
        cancelButtonLabel="Cancel"
        id="testId"
      />
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <SaveCancelButtons
        showReminder={true}
        reminderTest="You have unsaved changes"
        saveButtonLabel="Save"
        cancelButtonLabel="Cancel"
        className="test"
      />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });
});
