import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, boolean } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import TabContainer from ".";
import Section from "../../../.storybook/decorators/section";
import { action } from "@storybook/addon-actions";

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
          <input /> <input /> <input />
        </div>
        <div>
          <input /> <input /> <input />
        </div>
        <div>
          <input /> <input /> <input />
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
    )
  }
];

const scrollArrayItems = [
  {
    key: "tab0",
    title: "First long tab container",
    content: (
      <div>
        <button>button</button>
        <button>button</button>
        <button>button</button>
      </div>
    )
  },
  {
    key: "tab1",
    title: "Short",
    content: (
      <div>
        <label>label</label>
        <label>label</label>
        <label>label</label>
      </div>
    )
  },
  {
    key: "tab2",
    title: "Third long tab container",
    content: (
      <div>
        <input />
        <input />
        <input />
      </div>
    )
  },
  {
    key: "tab3",
    title: "Short2",
    content: (
      <div>
        <input />
        <input />
        <input />
      </div>
    )
  },
  {
    key: "tab4",
    title: "Third long tab container",
    content: (
      <div>
        <input />
        <input />
        <input />
      </div>
    )
  },
  {
    key: "tab5",
    title: "Short3",
    content: (
      <div>
        <input />
        <input />
        <input />
      </div>
    )
  },
  {
    key: "tab6",
    title: "short container",
    content: (
      <div>
        <button>button</button>
        <button>button</button>
        <button>button</button>
      </div>
    )
  },
  {
    key: "tab7",
    title: "Very long tabs-container field",
    content: (
      <div>
        <label>label</label>
        <label>label</label>
        <label>label</label>
      </div>
    )
  },
  {
    key: "tab8",
    title: "tab container",
    content: (
      <div>
        <input />
        <input />
        <input />
      </div>
    )
  },
  {
    key: "tab9",
    title: "Short___",
    content: (
      <div>
        <input />
        <input />
        <input />
      </div>
    )
  },
  {
    key: "tab10",
    title: "Short__2",
    content: (
      <div>
        <input />
        <input />
        <input />
      </div>
    )
  },
  {
    key: "tab11",
    title: "TabsContainer",
    content: (
      <div>
        <input />
        <input />
        <input />
      </div>
    )
  }
];

storiesOf("Components|TabContainer", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    return (
      <Section>
        <h5 style={{ marginBottom: 20 }}>Base TabsContainer:</h5>
        <TabContainer
          onSelect={index => action("Selected item")(index)}
          isDisabled={boolean("isDisabled", false)}
          selectedItem={arrayItems.indexOf(arrayItems[0])}
        >
          {arrayItems}
        </TabContainer>

        <div style={{ marginTop: 32, maxWidth: 430 }}>
          <h5 style={{ marginTop: 100, marginBottom: 20 }}>
            TabsContainer with auto scroll:
          </h5>
          <TabContainer
            isDisabled={boolean("isDisabled", false)}
            selectedItem={3}
          >
            {scrollArrayItems}
          </TabContainer>
        </div>

        <div style={{ marginTop: 32, maxWidth: 430 }}>
          <h5 style={{ marginTop: 100, marginBottom: 20 }}>
            TabsContainer with auto scroll:
          </h5>
          <TabContainer
            isDisabled={boolean("isDisabled", false)}
            selectedItem={5}
          >
            {[{
              key: "tab0",
              title: "Title00000000",
              content: <label>LABEL</label>
            },
            {
              key: "tab1",
              title: "Title00000001",
              content: <label>LABEL</label>
            },
            {
              key: "tab2",
              title: "Title00000002",
              content: <label>LABEL</label>
            },{
              key: "tab3",
              title: "Title00000003",
              content: <label>LABEL</label>
            },{
              key: "tab4",
              title: "Title00000004",
              content: <label>LABEL</label>
            },{
              key: "tab5",
              title: "Title00000005",
              content: <label>LABEL</label>
            },{
              key: "tab6",
              title: "Title00000006",
              content: <label>LABEL</label>
            },{
              key: "tab7",
              title: "Title00000007",
              content: <label>LABEL</label>
            },{
              key: "tab8",
              title: "Title00000008",
              content: <label>LABEL</label>
            },{
              key: "tab9",
              title: "Title00000009",
              content: <label>LABEL</label>
            }]}
          </TabContainer>
        </div>
      </Section>
    );
  });
