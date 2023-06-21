import { create } from "@storybook/theming/create";
import Logo from "./darksmall.svg";

export default create({
  base: "dark",
  appBg: "#333",

  brandTitle: "OnlyOffice",
  brandUrl: "https://www.onlyoffice.com/docspace.aspx",
  brandImage: Logo,
  brandTarget: "_self",
});
