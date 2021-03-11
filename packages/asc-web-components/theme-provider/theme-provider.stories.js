import React, { useState } from "react";
import Box from "../box";
import Text from "../text";
import JSONPretty from "react-json-pretty";
import Base from "../themes/base";
import Dark from "../themes/dark";
import Heading from "../heading";
import Checkbox from "../checkbox";
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
          checked={value}
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

/*
import { storiesOf } from "@storybook/react";
import ThemeProvider from ".";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import { BooleanValue } from "react-values";
import Box from "../Box";
import Text from "../Text";
import JSONPretty from "react-json-pretty";
import Base from "../themes/base";
import Heading from "../Heading";
import Checkbox from "../Checkbox";

const LightTheme = {
  backgroundColor: "#FFF",
  fontFamily: "sans-serif",
  text: { color: "#333" },
};

const DarkTheme = {
  backgroundColor: "#1F2933",
  fontFamily: "Open Sans",
  text: { color: "#E4E7EB" },
};

storiesOf("Components|ThemeProvider", module)
  .addDecorator(withReadme(Readme))
  .add("Default", () => (
    <BooleanValue>
      {({ value, toggle }) => (
        <Box displayProp="flex" paddingProp="16px" alignItems="center">
          <ThemeProvider theme={value ? DarkTheme : LightTheme}>
            <Checkbox
              checked={value}
              onChange={(e) => toggle(e.target.value)}
              label={value ? "Dark" : "Light"}
            />
          </ThemeProvider>
        </Box>
      )}
    </BooleanValue>
  ));
storiesOf("Components|ThemeProvider", module)
  .addParameters({ options: { showAddonPanel: false } })
  .add("Base theme", () => {
    const jsonTheme = {
      main: "line-height:1.5;background:#FFF;overflow:auto;",
      error: "line-height:1.5;background:#FFF;overflow:auto;",
      key: "color:#444;",
      string: "color:#00873D;",
    };
    return (
      <ThemeProvider theme={LightTheme}>
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
  });
storiesOf("Components|ThemeProvider", module)
  .addParameters({ options: { showAddonPanel: false } })
  .add("Dark theme", () => {
    const jsonTheme = {
      main: "line-height:1.5;background:#1F2933;overflow:auto;",
      error: "line-height:1.5;background:#1F2933;overflow:auto;",
      key: "color:#1F97CA;",
      string: "color:#00873D;",
    };
    return (
      <ThemeProvider theme={DarkTheme}>
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
  });
*/
