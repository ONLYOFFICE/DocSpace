import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { text, boolean, withKnobs, select } from "@storybook/addon-knobs/react";
import MainButton from ".";
import DropDownItem from "../drop-down-item";
import { Icons } from "../icons";
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
      ? { iconName: `${select("iconName", iconNames, "PeopleIcon")}` }
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
            icon="CatalogEmployeeIcon"
            label="New employee"
            onClick={() => action("New employee clicked")}
          />
          <DropDownItem
            icon="CatalogGuestIcon"
            label="New quest"
            onClick={() => action("New quest clicked")}
          />
          <DropDownItem
            icon="CatalogDepartmentsIcon"
            label="New department"
            onClick={() => action("New department clicked")}
          />
          <DropDownItem isSeparator />
          <DropDownItem
            icon="InvitationLinkIcon"
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
