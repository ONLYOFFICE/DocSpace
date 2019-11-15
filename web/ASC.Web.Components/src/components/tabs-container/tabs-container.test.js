import React from "react";
import { mount } from "enzyme";
import TabContainer from ".";

const array_items = [
  {
    key: "tab0",
    title: "Title1",
    content: (
      <div>
        <div>
          <button>BUTTON</button> <button>BUTTON</button>
          <button>BUTTON</button>
        </div>
        <div>
          <button>BUTTON</button> <button>BUTTON</button>
          <button>BUTTON</button>
        </div>
        <div>
          <button>BUTTON</button> <button>BUTTON</button>
          <button>BUTTON</button>
        </div>
      </div>
    )
  },
  {
    key: "tab1",
    title: "Title2",
    content: (
      <div>
        <div>
          <label>LABEL</label> <label>LABEL</label> <label>LABEL</label>
        </div>
        <div>
          <label>LABEL</label> <label>LABEL</label> <label>LABEL</label>
        </div>
        <div>
          <label>LABEL</label> <label>LABEL</label> <label>LABEL</label>
        </div>
      </div>
    )
  },
  {
    key: "tab2",
    title: "Title3",
    content: (
      <div>
        <div>
          <input></input> <input></input> <input></input>
        </div>
        <div>
          <input></input> <input></input> <input></input>
        </div>
        <div>
          <input></input> <input></input> <input></input>
        </div>
      </div>
    )
  },
  {
    key: "tab3",
    title: "Title4",
    content: (
      <div>
        <div>
          {" "}
          <button>BUTTON</button> <button>BUTTON</button>{" "}
          <button>BUTTON</button>{" "}
        </div>
        <div>
          {" "}
          <button>BUTTON</button> <button>BUTTON</button>{" "}
          <button>BUTTON</button>{" "}
        </div>
        <div>
          {" "}
          <button>BUTTON</button> <button>BUTTON</button>{" "}
          <button>BUTTON</button>{" "}
        </div>
      </div>
    )
  },
  {
    key: "tab4",
    title: "Title5",
    content: (
      <div>
        <div>
          {" "}
          <label>LABEL</label> <label>LABEL</label> <label>LABEL</label>{" "}
        </div>
        <div>
          {" "}
          <label>LABEL</label> <label>LABEL</label> <label>LABEL</label>{" "}
        </div>
        <div>
          {" "}
          <label>LABEL</label> <label>LABEL</label> <label>LABEL</label>{" "}
        </div>
      </div>
    )
  }
];

describe("<TabContainer />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <TabContainer>
        {[
          {
            key: "0",
            title: "Title1",
            content: (
              <div>
                <>
                  <button>BUTTON</button>
                </>
                <>
                  <button>BUTTON</button>
                </>
                <>
                  <button>BUTTON</button>
                </>
              </div>
            )
          }
        ]}
      </TabContainer>
    );
    expect(wrapper).toExist();
  });

  it("TabContainer check Compare dates function", () => {
    const item = [
      {
        key: "0",
        title: "Title1",
        content: (
          <div>
            <div>
              <button>BUTTON</button>
            </div>
            <div>
              <button>BUTTON</button>
            </div>
          </div>
        )
      }
    ];
    const wrapper = mount(
      <TabContainer>{array_items}</TabContainer>
    ).instance();
    wrapper.titleClick(2, item);
    expect(wrapper.state.activeTab).toEqual(2);
  });
});

it("TabsContainer not re-render test", () => {
  const wrapper = mount(<TabContainer>{array_items}</TabContainer>).instance();
  const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props, wrapper.state);
  expect(shouldUpdate).toBe(false);
});
