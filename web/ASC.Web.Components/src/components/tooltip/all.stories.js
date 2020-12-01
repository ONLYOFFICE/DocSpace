import React from "react";
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Tooltip from "./";
import Section from "../../../.storybook/decorators/section";
import Link from "../link";
import Text from "../text";

const BodyStyle = { marginTop: 70, marginLeft: 50, position: "absolute" };
const BodyStyle_2 = { marginTop: 70, marginLeft: 250 };

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

storiesOf("Components|Tooltip", module)
  .addDecorator(withReadme(Readme))
  .add("all", () => {
    return (
      <Section>
        <div style={BodyStyle}>
          <h5 style={{ marginLeft: -5 }}>Hover on me</h5>
          <Link data-for="link" data-tip="Bob Johnston">
            Bob Johnston
          </Link>
        </div>
        <Tooltip id="link" offsetRight={90} effect="solid">
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

        <div style={BodyStyle_2}>
          <h5 style={{ marginLeft: -5 }}>Hover group</h5>
          <Link data-for="group" data-tip={0}>
            Bob
          </Link>
          <br />
          <Link data-for="group" data-tip={1}>
            John
          </Link>
          <br />
          <Link data-for="group" data-tip={2}>
            Kevin
          </Link>
          <br />
          <Link data-for="group" data-tip={3}>
            Alex
          </Link>
          <br />
          <Link data-for="group" data-tip={4}>
            Tomas
          </Link>
        </div>

        <Tooltip
          id="group"
          offsetRight={90}
          getContent={(dataTip) =>
            dataTip ? (
              <div>
                <Text isBold={true} fontSize="16px">
                  {arrayUsers[dataTip].name}
                </Text>
                <Text color="#A3A9AE" fontSize="13px">
                  {arrayUsers[dataTip].email}
                </Text>
                <Text fontSize="13px">{arrayUsers[dataTip].position}</Text>
              </div>
            ) : null
          }
        />
      </Section>
    );
  });
