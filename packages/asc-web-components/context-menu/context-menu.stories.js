import React from "react";

import RowContainer from "../row-container";
import RowContent from "../row-content";
import Row from "../row";
import ContextMenu from "./index";

export default {
  title: "Components/ContextMenu",
  component: ContextMenu,
  subcomponents: { RowContainer, Row, RowContent },
  parameters: {
    docs: {
      description: {
        component: `ContextMenu is used for a call context actions on a page.
        Implemented as part of RowContainer component.

For use within separate component it is necessary to determine active zone and events for calling and transferring options in menu.

In particular case, state is created containing options for particular Row element and passed to component when called.
        `,
      },
    },
  },
};

const getRndString = (n) =>
  Math.random()
    .toString(36)
    .substring(2, n + 2);

const array = Array.from(Array(10).keys());
const Template = (args) => (
  <RowContainer {...args} manualHeight="300px">
    {array.map((item, index) => {
      return (
        <Row
          key={`${item + 1}`}
          contextOptions={
            index !== 3
              ? [
                  { key: 1, label: getRndString(5) },
                  { key: 2, label: getRndString(5) },
                  { key: 3, label: getRndString(5) },
                  { key: 4, label: getRndString(5) },
                ]
              : []
          }
        >
          <RowContent>
            <span>{getRndString(5)}</span>
            <></>
          </RowContent>
        </Row>
      );
    })}
  </RowContainer>
);

export const Default = Template.bind({});
