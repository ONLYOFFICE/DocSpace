import React from "react";
import MainButton from ".";
import DropDownItem from "../drop-down-item";

export default {
  title: "Components/MainButton",
  component: MainButton,
  subcomponents: { DropDownItem },
  parameters: { docs: { description: { component: "Components/MainButton" } } },
  clickAction: { action: "clickAction" },
  clickActionSecondary: { action: "clickActionSecondary" },
  clickItem: { action: "clickItem", table: { disable: true } },
};

const Template = ({
  clickAction,
  clickActionSecondary,
  clickItem,
  ...args
}) => {
  const clickMainButtonHandler = (e, credentials) => {
    clickAction(e, credentials);
  };

  const clickSecondaryButtonHandler = (e, credentials) => {
    clickActionSecondary(e, credentials);
  };

  let icon = !args.isDropdown
    ? {
        iconName: "static/images/people.react.svg",
      }
    : {};
  return (
    <div style={{ width: "280px" }}>
      <MainButton
        {...args}
        clickAction={clickMainButtonHandler}
        clickActionSecondary={clickSecondaryButtonHandler}
        iconName
        {...icon}
      >
        <DropDownItem
          icon="static/images/catalog.employee.react.svg"
          label="New employee"
          onClick={() => clickItem("New employee clicked")}
        />
        <DropDownItem
          icon="static/images/catalog.departments.react.svg"
          label="New department"
          onClick={() => clickItem("New department clicked")}
        />
        <DropDownItem isSeparator />
        <DropDownItem
          icon="static/images/invitation.link.react.svg"
          label="Invitation link"
          onClick={() => clickItem("Invitation link clicked")}
        />
      </MainButton>
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  isDisabled: false,
  isDropdown: true,
  text: "Actions",
  iconName: "static/images/people.react.svg",
};

/*

import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { text, boolean, withKnobs, select } from "@storybook/addon-knobs/react";

import Section from "../../../.storybook/decorators/section";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";

const iconNames = Object.keys(Icons);

function ClickMainButton(e, credentials) {
  console.log("ClickMainButton", e, credentials);
}

function ClickSecondaryButton(e, credentials) {
  console.log("ClickSecondaryButton", e, credentials);
}

storiesOf("Components|MainButton", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    let isDropdown = boolean("isDropdown", true);

    let icon = !isDropdown
      ? {
          iconName: `${select(
            "iconName",
            iconNames,
            "static/images/people.react.svg"
          )}`,
        }
      : {};

    return (
      <Section>
        <MainButton
          isDisabled={boolean("isDisabled", false)}
          isDropdown={isDropdown}
          text={text("text", "Actions")}
          clickAction={ClickMainButton}
          clickActionSecondary={ClickSecondaryButton}
          {...icon}
        >
          <DropDownItem
            icon="static/images/catalog.employee.react.svg"
            label="New employee"
            onClick={() => action("New employee clicked")}
          />
          <DropDownItem
            icon="CatalogGuestIcon"
            label="New quest"
            onClick={() => action("New quest clicked")}
          />
          <DropDownItem
            icon="static/images/catalog.departments.react.svg"
            label="New department"
            onClick={() => action("New department clicked")}
          />
          <DropDownItem isSeparator />
          <DropDownItem
            icon="static/images/invitation.link.react.svg"
            label="Invitation link"
            onClick={() => action("Invitation link clicked")}
          />
          <DropDownItem
            icon="PlaneIcon"
            label="Invite again"
            onClick={() => action("Invite again clicked")}
          />
          <DropDownItem
            icon="ImportIcon"
            label="Import people"
            onClick={() => action("Import people clicked")}
          />
        </MainButton>
      </Section>
    );
  });
*/
