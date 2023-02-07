import React from "react";
import { mount } from "enzyme";
import TreeMenu from ".";
import TreeNode from "./sub-components/tree-node";

describe("<TreeMenu />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <TreeMenu
        checkable={false}
        draggable={true}
        disabled={false}
        multiple={false}
        showIcon={true}
        showLine={false}
      >
        <TreeNode title="Parent" key="0-0">
          <TreeNode title="Child" key="0-0-0"></TreeNode>
        </TreeNode>
      </TreeMenu>
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(
      <TreeMenu
        id="testId"
        checkable={false}
        draggable={true}
        disabled={false}
        multiple={false}
        showIcon={true}
        showLine={false}
      >
        <TreeNode title="Parent" key="0-0">
          <TreeNode title="Child" key="0-0-0"></TreeNode>
        </TreeNode>
      </TreeMenu>
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <TreeMenu
        className="test"
        checkable={false}
        draggable={true}
        disabled={false}
        multiple={false}
        showIcon={true}
        showLine={false}
      >
        <TreeNode title="Parent" key="0-0">
          <TreeNode title="Child" key="0-0-0"></TreeNode>
        </TreeNode>
      </TreeMenu>
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  /*   it("accepts style", () => {
    const wrapper = mount(
      <TreeMenu
        style={{ color: "red" }}
        checkable={false}
        draggable={true}
        disabled={false}
        multiple={false}
        showIcon={true}
        showLine={false}
      >
        <TreeNode title="Parent" key="0-0">
          <TreeNode title="Child" key="0-0-0"></TreeNode>
        </TreeNode>
      </TreeMenu>
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  }); */
  it("accepts isFullFillSelection", () => {
    const wrapper = mount(
      <TreeMenu isFullFillSelection={false}>
        <TreeNode title="Parent" key="0-0">
          <TreeNode title="Child" key="0-0-0"></TreeNode>
        </TreeNode>
      </TreeMenu>
    );

    expect(wrapper.prop("isFullFillSelection")).toEqual(false);
  });
  it("accepts gapBetweenNodes and gapBetweenNodesTablet", () => {
    const wrapper = mount(
      <TreeMenu gapBetweenNodes="22" gapBetweenNodesTablet="24">
        <TreeNode title="Parent" key="0-0">
          <TreeNode title="Child" key="0-0-0"></TreeNode>
        </TreeNode>
      </TreeMenu>
    );

    expect(wrapper.prop("gapBetweenNodes")).toEqual("22");
    expect(wrapper.prop("gapBetweenNodesTablet")).toEqual("24");
  });
  it("accepts isEmptyRootNode", () => {
    const wrapper = mount(
      <TreeMenu isEmptyRootNode={true}>
        <TreeNode title="Parent" key="0-0">
          <TreeNode title="Child" key="0-0-0"></TreeNode>
        </TreeNode>
      </TreeMenu>
    );

    expect(wrapper.prop("isEmptyRootNode")).toEqual(true);
  });
});
