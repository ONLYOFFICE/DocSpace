import React, { useState } from "react";
import Box from "../box";
import Text from "../text";
import JSONPretty from "react-json-pretty";
import Base from "../themes/base";
import Dark from "../themes/dark";
import Heading from "../heading";
import Checkbox from "../checkbox";
import { ColorTheme, ThemeType } from "../ColorTheme";
import ThemeProvider from "./";

export default {
  title: "Components/ThemeProvider",
  component: ThemeProvider,
  parameters: {
    docs: {
      description: {
        component: `Custom theme provider based on [Theme Provider](https://www.styled-components.com/docs/advanced).

List of themes:
        
- [Base theme](https://dotnet.onlyoffice.com:8084/?path=/story/components-themeprovider--base-theme)
- [Dark theme](https://dotnet.onlyoffice.com:8084/?path=/story/components-themeprovider--dark-theme)


You can change the CSS styles in the theme, and they will be applied to all children components of ThemeProvider`,
      },
    },
  },
};

const Template = (args) => {
  const [value, setValue] = useState(false);
  return (
    <Box displayProp="flex" paddingProp="16px" alignItems="center">
      <ThemeProvider {...args} theme={value ? Dark : Base}>
        <Checkbox
          isChecked={value}
          onChange={(e) => setValue(!value)}
          label={value ? "Dark" : "Light"}
        />
      </ThemeProvider>
    </Box>
  );
};
export const Default = Template.bind({});

const BaseTemplate = (args) => {
  const jsonTheme = {
    main: "line-height:1.5;background:#FFF;overflow:auto;",
    error: "line-height:1.5;background:#FFF;overflow:auto;",
    key: "color:#444;",
    string: "color:#00873D;",
  };

  return (
    <ThemeProvider theme={Base}>
      <Box paddingProp={"16px"}>
        <Heading>Base theme:</Heading>
        <Text as="div" bold fontSize="14px">
          <JSONPretty
            id="json-pretty"
            data={JSON.stringify(Base)}
            theme={jsonTheme}
          />
        </Text>
      </Box>
    </ThemeProvider>
  );
};
export const BaseTheme = BaseTemplate.bind({});

const DarkTemplate = (args) => {
  const jsonTheme = {
    main: "line-height:1.5;background:#1F2933;overflow:auto;",
    error: "line-height:1.5;background:#1F2933;overflow:auto;",
    key: "color:#1F97CA;",
    string: "color:#00873D;",
  };
  return (
    <ThemeProvider theme={Dark}>
      <Box paddingProp={"16px"}>
        <Heading>Dark theme:</Heading>
        <Text as="div" bold color="#1F97CA" fontSize="14px">
          <JSONPretty
            id="json-pretty"
            data={JSON.stringify(Dark)}
            theme={jsonTheme}
          />
        </Text>
      </Box>
    </ThemeProvider>
  );
};
export const DarkTheme = DarkTemplate.bind({});

const ColorTemplate = (args) => {
  const currentColorScheme = {
    id: 7,
    main: {
      accent: "#ef22ad",
      buttons: "#f426a3",
    },
    name: "",
    text: {
      accent: "#fffff",
      buttons: "#fffff",
    },
  };

  return (
    <ThemeProvider
      {...args}
      theme={Base}
      currentColorScheme={currentColorScheme}
    >
      <ColorTheme themeId={ThemeType.Button} primary>
        Button
      </ColorTheme>
    </ThemeProvider>
  );
};

export const CurrentColorTheme = ColorTemplate.bind({});
