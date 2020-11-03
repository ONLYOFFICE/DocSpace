import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, text, select } from "@storybook/addon-knobs/react";
import { BooleanValue } from "react-values";
import ProfileMenu from ".";
import Section from "../../../.storybook/decorators/section";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import { DropDownItem, Avatar } from "asc-web-components";

const roleOptions = ["owner", "admin", "guest", "user"];
const defaultAvatar =
  "https://static-www.onlyoffice.com/images/team/developers_photos/personal_44_2x.jpg";

storiesOf("Components|ProfileMenu", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))

  .add("base", () => {
    const userRole = select("avatarRole", roleOptions, "admin");
    const userAvatar = text("avatarSource", "") || defaultAvatar;
    const userEmail = text("email", "") || "janedoe@gmail.com";
    const userDisplayName = text("displayName", "") || "Jane Doe";

    return (
      <Section>
        <BooleanValue>
          {({ value, toggle }) => (
            <div
              style={{
                position: "relative",
                float: "right",
                height: "56px",
                paddingRight: "4px",
              }}
            >
              <Avatar
                size="medium"
                role={userRole}
                source={userAvatar}
                userName={userDisplayName}
                onClick={() => toggle(!value)}
              />
              <ProfileMenu
                avatarRole={userRole}
                avatarSource={userAvatar}
                displayName={userDisplayName}
                email={userEmail}
                open={value}
              >
                <DropDownItem
                  key="1"
                  label="Profile"
                  onClick={() => console.log("Profile click")}
                />
                <DropDownItem
                  key="2"
                  label="Subscriptions"
                  onClick={() => console.log("Subscriptions click")}
                />
                <DropDownItem key="sep" isSeparator />
                <DropDownItem
                  key="3"
                  label="About this program"
                  onClick={() => console.log("About click")}
                />
                <DropDownItem
                  key="4"
                  label="Log out"
                  onClick={() => console.log("Log out click")}
                />
              </ProfileMenu>
            </div>
          )}
        </BooleanValue>
      </Section>
    );
  });
