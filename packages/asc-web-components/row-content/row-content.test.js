import React from "react";
import { mount } from "enzyme";
import RowContent from ".";
import Link from "../link";

describe("<RowContent />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <RowContent>
        <Link
          type="page"
          title="Demo"
          isBold={true}
          fontSize="15px"
          color="#333333"
        >
          Demo
        </Link>
        <Link type="page" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link type="action" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link type="page" title="0 000 0000000" fontSize="12px" color="#A3A9AE">
          0 000 0000000
        </Link>
        <Link type="page" title="demo@demo.com" fontSize="12px" color="#A3A9AE">
          demo@demo.com
        </Link>
      </RowContent>
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(
      <RowContent id="testId">
        <Link
          type="page"
          title="Demo"
          isBold={true}
          fontSize="15px"
          color="#333333"
        >
          Demo
        </Link>
        <Link type="page" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link type="action" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link type="page" title="0 000 0000000" fontSize="12px" color="#A3A9AE">
          0 000 0000000
        </Link>
        <Link type="page" title="demo@demo.com" fontSize="12px" color="#A3A9AE">
          demo@demo.com
        </Link>
      </RowContent>
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <RowContent className="test">
        <Link
          type="page"
          title="Demo"
          isBold={true}
          fontSize="15px"
          color="#333333"
        >
          Demo
        </Link>
        <Link type="page" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link type="action" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link type="page" title="0 000 0000000" fontSize="12px" color="#A3A9AE">
          0 000 0000000
        </Link>
        <Link type="page" title="demo@demo.com" fontSize="12px" color="#A3A9AE">
          demo@demo.com
        </Link>
      </RowContent>
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <RowContent style={{ color: "red" }}>
        <Link
          type="page"
          title="Demo"
          isBold={true}
          fontSize="15px"
          color="#333333"
        >
          Demo
        </Link>
        <Link type="page" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link type="action" title="Demo" fontSize="12px" color="#A3A9AE">
          Demo
        </Link>
        <Link type="page" title="0 000 0000000" fontSize="12px" color="#A3A9AE">
          0 000 0000000
        </Link>
        <Link type="page" title="demo@demo.com" fontSize="12px" color="#A3A9AE">
          demo@demo.com
        </Link>
      </RowContent>
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
