import React from "react";
import Link from "../link";
import Text from "../text";
import Tooltip from "./";
import TooltipDocs from "./tooltip.mdx";

export default {
  title: "Components/Tooltip",
  component: Tooltip,
  parameters: {
    docs: {
      page: TooltipDocs,
    },
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?node-id=649%3A4458&mode=dev",
    },
  },
};

const BodyStyle = { marginTop: 100, marginLeft: 200, position: "absolute" };

const Template = (args) => {
  return (
    <div style={{ height: "240px" }}>
      <div style={BodyStyle}>
        <Link data-tooltip-id="link" data-tooltip-content="Bob Johnston">
          Bob Johnston
        </Link>
      </div>

      <Tooltip
        {...args}
        id="link"
        getContent={({ content }) => (
          <div>
            <Text isBold={true} fontSize="16px">
              {content}
            </Text>
            <Text color="#A3A9AE" fontSize="13px">
              BobJohnston@gmail.com
            </Text>
            <Text fontSize="13px">Developer</Text>
          </div>
        )}
      />
    </div>
  );
};

export const basic = Template.bind({});
basic.args = {
  float: true,
  place: "top",
};

const arrayUsers = [
  {
    key: "user_1",
    name: "Bob",
    email: "Bob@gmail.com",
    position: "developer",
  },
  {
    key: "user_2",
    name: "John",
    email: "John@gmail.com",
    position: "developer",
  },
  {
    key: "user_3",
    name: "Kevin",
    email: "Kevin@gmail.com",
    position: "developer",
  },
  {
    key: "user_4",
    name: "Alex",
    email: "Alex@gmail.com",
    position: "developer",
  },
  {
    key: "user_5",
    name: "Tomas",
    email: "Tomas@gmail.com",
    position: "developer",
  },
];

const AllTemplate = (args) => {
  return (
    <div>
      <div>
        <h5 style={{ marginLeft: -5 }}>Hover on me</h5>
        <Link data-tooltip-id="link" data-tooltip-content="Bob Johnston">
          Bob Johnston
        </Link>
      </div>
      <Tooltip id="link" offset={0}>
        <div>
          <Text isBold={true} fontSize="16px">
            Bob Johnston
          </Text>
          <Text color="#A3A9AE" fontSize="13px">
            BobJohnston@gmail.com
          </Text>
          <Text fontSize="13px">Developer</Text>
        </div>
      </Tooltip>

      <div>
        <h5 style={{ marginLeft: -5 }}>Hover group</h5>
        <Link data-tooltip-id="group" data-tooltip-content={0}>
          Bob
        </Link>
        <br />
        <Link data-tooltip-id="group" data-tooltip-content={1}>
          John
        </Link>
        <br />
        <Link data-tooltip-id="group" data-tooltip-content={2}>
          Kevin
        </Link>
        <br />
        <Link data-tooltip-id="group" data-tooltip-content={3}>
          Alex
        </Link>
        <br />
        <Link data-tooltip-id="group" data-tooltip-content={4}>
          Tomas
        </Link>
      </div>

      <Tooltip
        id="group"
        offsetRight={0}
        getContent={({ content }) =>
          content ? (
            <div>
              <Text isBold={true} fontSize="16px">
                {arrayUsers[content].name}
              </Text>
              <Text color="#A3A9AE" fontSize="13px">
                {arrayUsers[content].email}
              </Text>
              <Text fontSize="13px">{arrayUsers[content].position}</Text>
            </div>
          ) : null
        }
      />
    </div>
  );
};

export const all = AllTemplate.bind({});
