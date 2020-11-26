import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, boolean, text, select } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Avatar from ".";
import Section from "../../../.storybook/decorators/section";

const roleOptions = ["owner", "admin", "guest", "user"];
const sizeOptions = ["max", "big", "medium", "small", "min"];

const editAction = () => console.log("Edit action");

storiesOf("Components|Avatar", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    const size = select("size", sizeOptions, "max");
    const editing = size === "max" ? boolean("editing", false) : false;
    const editLabel = editing ? text("editLabel", "Edit photo") : "";

    return (
      <Section>
        <Avatar
          size={size}
          role={select("role", roleOptions, "admin")}
          source={text("source", "")}
          userName={text("userName", "")}
          editing={editing}
          editLabel={editLabel}
          editAction={editAction}
        />
      </Section>
    );
  });
