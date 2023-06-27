import React from "react";

import Grid from "./";
import Box from "../box";
import Text from "../text";

export default {
  title: "Components/Grid",
  component: Grid,
  subcomponents: { Box },
  argTypes: {},
  parameters: {
    docs: {
      description: {
        component:
          "A container that lays out its contents in a 2-dimensional grid system. Use Box components to define rows and columns",
      },
    },
  },
};

const gridProps = {
  marginProp: "0 0 20px 0",
};

const boxProps = {
  paddingProp: "10px",
  displayProp: "flex",
  alignItems: "center",
  justifyContent: "center",
};

const Template = (args) => {
  return (
    <Grid {...args} {...gridProps}>
      <Box {...boxProps} backgroundProp="#F4991A">
        <Text color={"#000"}>200px</Text>
      </Box>
      <Box {...boxProps} backgroundProp="#F2EAD3">
        <Text color={"#000"}>minmax(100px,1fr)</Text>
      </Box>
      <Box {...boxProps} backgroundProp="#F9F5F0">
        <Text color={"#000"}>auto</Text>
      </Box>
    </Grid>
  );
};

const TemplateColumns = (args) => {
  return (
    <>
      <Grid
        {...args}
        {...gridProps}
        columnsProp={["200px", ["100px", "1fr"], "auto"]}
      >
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text color={"#000"}>200px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text color={"#000"}>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text color={"#000"}>auto</Text>
        </Box>
      </Grid>

      <Grid {...args} {...gridProps} columnsProp="25%">
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text color={"#000"}>25%</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text color={"#000"}>25%</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text color={"#000"}>25%</Text>
        </Box>
      </Grid>

      <Grid {...args} {...gridProps} columnsProp={{ count: 3, size: "100px" }}>
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text color={"#000"}>100px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text color={"#000"}>100px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text color={"#000"}>100px</Text>
        </Box>
      </Grid>

      <Grid
        {...args}
        {...gridProps}
        columnsProp={{ count: 3, size: ["100px", "1fr"] }}
      >
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text color={"#000"}>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text color={"#000"}>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text color={"#000"}>minmax(100px,1fr)</Text>
        </Box>
      </Grid>
    </>
  );
};

const TemplateRows = (args) => {
  return (
    <>
      <Grid
        {...args}
        {...gridProps}
        rowsProp={["100px", ["100px", "1fr"], "auto"]}
      >
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text color={"#000"}>100px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text color={"#000"}>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text color={"#000"}>auto</Text>
        </Box>
      </Grid>

      <Grid {...args} {...gridProps} rowsProp="50px">
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text color={"#000"}>50px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text color={"#000"}>50px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text color={"#000"}>50px</Text>
        </Box>
      </Grid>
    </>
  );
};

const TemplateLayout = (args) => {
  return (
    <Grid
      {...args}
      widthProp="100vw"
      heightProp="100vh"
      gridGap="10px"
      rowsProp={["auto", "1fr", "auto"]}
      columnsProp={[["100px", "1fr"], "3fr", ["100px", "1fr"]]}
      areasProp={[
        { name: "header", start: [0, 0], end: [2, 0] },
        { name: "navbar", start: [0, 1], end: [0, 1] },
        { name: "main", start: [1, 1], end: [1, 1] },
        { name: "sidebar", start: [2, 1], end: [2, 1] },
        { name: "footer", start: [0, 2], end: [2, 2] },
      ]}
    >
      <Box {...boxProps} gridArea="header" backgroundProp="#F4991A">
        <Text color={"#000"}>header</Text>
      </Box>
      <Box {...boxProps} gridArea="navbar" backgroundProp="#F2EAD3">
        <Text color={"#000"}>navbar</Text>
      </Box>
      <Box {...boxProps} gridArea="main" backgroundProp="#F9F5F0">
        <Text color={"#000"}>main</Text>
      </Box>
      <Box {...boxProps} gridArea="sidebar" backgroundProp="#F2EAD3">
        <Text color={"#000"}>sidebar</Text>
      </Box>
      <Box {...boxProps} gridArea="footer" backgroundProp="#F4991A">
        <Text color={"#000"}>footer</Text>
      </Box>
    </Grid>
  );
};

export const Default = Template.bind({});
Default.args = {
  columnsProp: ["200px", ["100px", "1fr"], "auto"],
};
export const Columns = TemplateColumns.bind({});
export const Rows = TemplateRows.bind({});
export const Layout = TemplateLayout.bind({});
