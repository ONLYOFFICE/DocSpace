import React from "react";
import TabContainer from "./";

export default {
  title: "Components/TabContainer",
  component: TabContainer,
  parameters: {
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?type=design&node-id=638-4439&mode=design&t=TBNCKMQKQMxr44IZ-0",
    },
  },
};

const arrayItems = [
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
    ),
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
    ),
  },
  {
    key: "tab2",
    title: "Title3",
    content: (
      <div>
        <div>
          <input /> <input /> <input />
        </div>
        <div>
          <input /> <input /> <input />
        </div>
        <div>
          <input /> <input /> <input />
        </div>
      </div>
    ),
  },
  {
    key: "tab3",
    title: "Title4",
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
    ),
  },
  {
    key: "tab4",
    title: "Title5",
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
    ),
  },
];

const scrollArrayItems = [
  {
    key: "tab0",
    title: "First long tab container",
    content: (
      <>
        <label>Tab_0 Tab_0 Tab_0</label>
        <br />
        <label>Tab_0 Tab_0 Tab_0</label>
        <br />
        <label>Tab_0 Tab_0 Tab_0</label>
      </>
    ),
  },
  {
    key: "tab1",
    title: "Short",
    content: (
      <>
        <label>Tab_1 Tab_1 Tab_1</label>
        <br />
        <label>Tab_1 Tab_1 Tab_1</label>
        <br />
        <label>Tab_1 Tab_1 Tab_1</label>
      </>
    ),
  },
  {
    key: "tab2",
    title: "Second long tab container",
    content: (
      <>
        <label>Tab_2 Tab_2 Tab_2</label>
        <br />
        <label>Tab_2 Tab_2 Tab_2</label>
        <br />
        <label>Tab_2 Tab_2 Tab_2</label>
      </>
    ),
  },
  {
    key: "tab3",
    title: "Short2",
    content: (
      <>
        <label>Tab_3 Tab_3 Tab_3</label>
        <br />
        <label>Tab_3 Tab_3 Tab_3</label>
        <br />
        <label>Tab_3 Tab_3 Tab_3</label>
      </>
    ),
  },
  {
    key: "tab4",
    title: "Third long tab container header",
    content: (
      <>
        <label>Tab_4 Tab_4 Tab_4</label>
        <br />
        <label>Tab_4 Tab_4 Tab_4</label>
        <br />
        <label>Tab_4 Tab_4 Tab_4</label>
      </>
    ),
  },
  {
    key: "tab5",
    title: "Short3",
    content: (
      <>
        <label>Tab_5 Tab_5 Tab_5</label>
        <br />
        <label>Tab_5 Tab_5 Tab_5</label>
        <br />
        <label>Tab_5 Tab_5 Tab_5</label>
      </>
    ),
  },
  {
    key: "tab6",
    title: "tab container",
    content: (
      <>
        <label>Tab_6 Tab_6 Tab_6</label>
        <br />
        <label>Tab_6 Tab_6 Tab_6</label>
        <br />
        <label>Tab_6 Tab_6 Tab_6</label>
      </>
    ),
  },
  {
    key: "tab7",
    title: "Very long tabs-container field",
    content: (
      <>
        <label>Tab_7 Tab_7 Tab_7</label>
        <br />
        <label>Tab_7 Tab_7 Tab_7</label>
        <br />
        <label>Tab_7 Tab_7 Tab_7</label>
      </>
    ),
  },
  {
    key: "tab8",
    title: "tab container",
    content: (
      <>
        <label>Tab_8 Tab_8 Tab_8</label>
        <br />
        <label>Tab_8 Tab_8 Tab_8</label>
        <br />
        <label>Tab_8 Tab_8 Tab_8</label>
      </>
    ),
  },
  {
    key: "tab9",
    title: "Short_04",
    content: (
      <>
        <label>Tab_9 Tab_9 Tab_9</label>
        <br />
        <label>Tab_9 Tab_9 Tab_9</label>
        <br />
        <label>Tab_9 Tab_9 Tab_9</label>
      </>
    ),
  },
  {
    key: "tab10",
    title: "Short__05",
    content: (
      <>
        <label>Tab_10 Tab_10 Tab_10</label>
        <br />
        <label>Tab_10 Tab_10 Tab_10</label>
        <br />
        <label>Tab_10 Tab_10 Tab_10</label>
      </>
    ),
  },
  {
    key: "tab11",
    title: "TabsContainer",
    content: (
      <>
        <label>Tab_11 Tab_11 Tab_11</label>
        <br />
        <label>Tab_11 Tab_11 Tab_11</label>
        <br />
        <label>Tab_11 Tab_11 Tab_11</label>
      </>
    ),
  },
];

const tabsItems = [
  {
    key: "tab0",
    title: "Title00000000",
    content: (
      <>
        <label>Tab_0 Tab_0 Tab_0</label>
        <br />
        <label>Tab_0 Tab_0 Tab_0</label>
        <br />
        <label>Tab_0 Tab_0 Tab_0</label>
      </>
    ),
  },
  {
    key: "tab1",
    title: "Title00000001",
    content: (
      <>
        <label>Tab_1 Tab_1 Tab_1</label>
        <br />
        <label>Tab_1 Tab_1 Tab_1</label>
        <br />
        <label>Tab_1 Tab_1 Tab_1</label>
      </>
    ),
  },
  {
    key: "tab2",
    title: "Title00000002",
    content: (
      <>
        <label>Tab_2 Tab_2 Tab_2</label>
        <br />
        <label>Tab_2 Tab_2 Tab_2</label>
        <br />
        <label>Tab_2 Tab_2 Tab_2</label>
      </>
    ),
  },
  {
    key: "tab3",
    title: "Title00000003",
    content: (
      <>
        <label>Tab_3 Tab_3 Tab_3</label>
        <br />
        <label>Tab_3 Tab_3 Tab_3</label>
        <br />
        <label>Tab_3 Tab_3 Tab_3</label>
      </>
    ),
  },
  {
    key: "tab4",
    title: "Title00000004",
    content: (
      <>
        <label>Tab_4 Tab_4 Tab_4</label>
        <br />
        <label>Tab_4 Tab_4 Tab_4</label>
        <br />
        <label>Tab_4 Tab_4 Tab_4</label>
      </>
    ),
  },
  {
    key: "tab5",
    title: "Title00000005",
    content: (
      <>
        <label>Tab_5 Tab_5 Tab_5</label>
        <br />
        <label>Tab_5 Tab_5 Tab_5</label>
        <br />
        <label>Tab_5 Tab_5 Tab_5</label>
      </>
    ),
  },
  {
    key: "tab6",
    title: "Title00000006",
    content: (
      <>
        <label>Tab_6 Tab_6 Tab_6</label>
        <br />
        <label>Tab_6 Tab_6 Tab_6</label>
        <br />
        <label>Tab_6 Tab_6 Tab_6</label>
      </>
    ),
  },
  {
    key: "tab7",
    title: "Title00000007",
    content: (
      <>
        <label>Tab_7 Tab_7 Tab_7</label>
        <br />
        <label>Tab_7 Tab_7 Tab_7</label>
        <br />
        <label>Tab_7 Tab_7 Tab_7</label>
      </>
    ),
  },
  {
    key: "tab8",
    title: "Title00000008",
    content: (
      <>
        <label>Tab_8 Tab_8 Tab_8</label>
        <br />
        <label>Tab_8 Tab_8 Tab_8</label>
        <br />
        <label>Tab_8 Tab_8 Tab_8</label>
      </>
    ),
  },
  {
    key: "tab9",
    title: "Title00000009",
    content: (
      <>
        <label>Tab_9 Tab_9 Tab_9</label>
        <br />
        <label>Tab_9 Tab_9 Tab_9</label>
        <br />
        <label>Tab_9 Tab_9 Tab_9</label>
      </>
    ),
  },
];

const Template = ({ onSelect, ...args }) => {
  return (
    <div>
      <h5 style={{ marginBottom: 20 }}>Base TabsContainer:</h5>
      <TabContainer
        {...args}
        onSelect={(index) => onSelect(index)}
        selectedItem={arrayItems.indexOf(arrayItems[0])}
        elements={arrayItems}
      />

      <div style={{ marginTop: 32, maxWidth: 430 }}>
        <h5 style={{ marginTop: 100, marginBottom: 20 }}>
          Autoscrolling with different tab widths:
        </h5>
        <TabContainer
          {...args}
          selectedItem={3}
          elements={scrollArrayItems}
          onSelect={(index) => onSelect(index)}
        />
      </div>

      <div style={{ marginTop: 32, maxWidth: 430 }}>
        <h5 style={{ marginTop: 100, marginBottom: 20 }}>
          Autoscrolling with the same tabs width:
        </h5>
        <TabContainer
          {...args}
          selectedItem={5}
          elements={tabsItems}
          onSelect={(index) => onSelect(index)}
        />
      </div>
    </div>
  );
};

export const basic = Template.bind({});
basic.args = {
  isDisabled: false,
};
