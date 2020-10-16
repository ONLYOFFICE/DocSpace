import React from "react";
import { mount } from "enzyme";
import FilterInput from ".";

describe("<FilterInput />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <FilterInput
        getFilterData={() => [
          {
            key: "filter-example",
            group: "filter-example",
            label: "example group",
            isHeader: true,
          },
          { key: "0", group: "filter-example", label: "Test" },
        ]}
        getSortData={() => [
          { key: "name", label: "Name" },
          { key: "surname", label: "Surname" },
        ]}
        onFilter={jest.fn()}
      />
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(
      <FilterInput
        getFilterData={() => [
          {
            key: "filter-example",
            group: "filter-example",
            label: "example group",
            isHeader: true,
          },
          { key: "0", group: "filter-example", label: "Test" },
        ]}
        getSortData={() => [
          { key: "name", label: "Name" },
          { key: "surname", label: "Surname" },
        ]}
        onFilter={jest.fn()}
        id="testId"
      />
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <FilterInput
        getFilterData={() => [
          {
            key: "filter-example",
            group: "filter-example",
            label: "example group",
            isHeader: true,
          },
          { key: "0", group: "filter-example", label: "Test" },
        ]}
        getSortData={() => [
          { key: "name", label: "Name" },
          { key: "surname", label: "Surname" },
        ]}
        onFilter={jest.fn()}
        className="test"
      />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <FilterInput
        getFilterData={() => [
          {
            key: "filter-example",
            group: "filter-example",
            label: "example group",
            isHeader: true,
          },
          { key: "0", group: "filter-example", label: "Test" },
        ]}
        getSortData={() => [
          { key: "name", label: "Name" },
          { key: "surname", label: "Surname" },
        ]}
        onFilter={jest.fn()}
        style={{ color: "red" }}
      />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
