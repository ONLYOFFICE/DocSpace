import { create } from "@storybook/theming/create";
import Logo from "./darksmall.svg?url";

export default create({
  base: "dark",
  appBg: "#333",

  brandTitle: "ONLYOFFICE",
  brandUrl: "https://www.onlyoffice.com/docspace.aspx",
  brandImage: Logo,
  brandTarget: "_self",
});
