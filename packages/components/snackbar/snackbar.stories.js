import React from "react";
import Box from "../box";
import SnackBar from "./";

export default {
  title: "Components/SnackBar",
  component: SnackBar,
  parameters: {
    docs: {
      description: {
        component: "SnackBar is used for displaying important messages.",
      },
    },
  },
  argTypes: {
    textColor: { control: "color" },
    backgroundColor: { control: "color" },
    showIcon: { control: "boolean" },
  },
};

const Wrapper = ({ children }) => (
  <Box id="main-bar" displayProp="grid">
    {children}
  </Box>
);

const BaseTemplate = (args) => (
  <Wrapper>
    <SnackBar {...args} onClose={(e) => alert("OnClose handled!", e)} />
  </Wrapper>
);

export const base = BaseTemplate.bind({});
base.args = {
  backgroundImg: "",
  backgroundColor: "#f8f7bf",
  textColor: "#000",
  opacity: 1,
  headerText: "Attention",
  text: "We apologize for any short-term technical issues in service functioning, that may appear on 22.06.2021 during the update of Onlyoffice Personal.",
  showIcon: true,
  fontSize: "13px",
  fontWeight: "400",
  textAlign: "left",
  htmlContent: "",
};
