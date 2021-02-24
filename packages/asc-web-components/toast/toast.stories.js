import React from "react";
import { storiesOf } from "@storybook/react";
import Toast from ".";
import toastr from "./toastr";
import Readme from "./README.md";
import {
  text,
  boolean,
  withKnobs,
  select,
  number,
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Section from "../../../.storybook/decorators/section";

const typeToast = ["success", "error", "warning", "info"];

storiesOf("Components|Toast", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    const type = `${select("type", typeToast, "success")}`;
    const data = `${text("data", "Demo text for Toast")}`;
    const title = `${text("title", "Demo title")}`;
    const withCross = boolean("withCross", false);
    const timeout = number("timeout", "5000");
    return (
      <>
        <Toast />
        <Section>
          <button
            onClick={() => {
              switch (type) {
                case "error":
                  toastr.error(data, title, timeout, withCross);
                  break;
                case "warning":
                  toastr.warning(data, title, timeout, withCross);
                  break;
                case "info":
                  toastr.info(data, title, timeout, withCross);
                  break;
                default:
                  toastr.success(data, title, timeout, withCross);
                  break;
              }
            }}
          >
            Show toast
          </button>
        </Section>
      </>
    );
  });
