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
        component: `A container that lays out its contents in a 2-dimensional grid system. Use Box components to define rows and columns.
        
### Properties

|      Props       |           Type            | Required |                                                                                           Values                                                                                            | Default | Description                                                                                                                                                                                                                                                                                                                              |
| :--------------: | :-----------------------: | :------: | :-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :-----: | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|  alignContent  |         string          |    -     |                                                                                              -                                                                                              |    -    | sets the distribution of space between and around content items along a flexbox's cross-axis or a grid's block axis                                                                                                                                                                                                                      |
|   alignItems   |         string          |    -     |                                                                                              -                                                                                              |    -    | sets the align-self value on all direct children as a group. In Flexbox, it controls the alignment of items on the Cross Axis. In Grid Layout, it controls the alignment of items on the Block Axis within their grid area.                                                                                                              |
|   alignSelf    |         string          |    -     |                                                                                              -                                                                                              |    -    | overrides a grid or flex item's align-items value. In Grid, it aligns the item inside the grid area. In Flexbox, it aligns the item on the cross axis.                                                                                                                                                                                   |
|   areasProp    |          array          |    -     | [["header","header"],["navbar","main"]], [{ name: "header", start: [0, 0], end: [1, 0] }, { name: "navbar", start: [0, 1], end: [0, 1] }, { name: "main", start: [1, 1], end: [1, 1] }]     |    -    | specifies named grid areas. Takes value as array of string arrays that specify named grid areas. Or objects array, that contains names and coordinates of areas.                                                                                                                                                                         |
|  columnsProp   | string,array,object |    -     |                                                        "25%", ["200px", ["100px","1fr"], "auto"], { count: 3, size: "100px" }                                                                   |    -    | defines the sizing of the grid columns. Specifying a single string will repeat several columns of this size. Specifying an object allows you to specify the number of repetitions and the size of the column. Or you can specify an array with column sizes. The column size can be specified as an array of minimum and maximum widths. |
|    gridArea    |         string          |    -     |                                                                                              -                                                                                              |    -    | is a shorthand property for grid-row-start, grid-column-start, grid-row-end and grid-column-end, specifying a grid item’s size and location within the grid by contributing a line, a span, or nothing (automatic) to its grid placement, thereby specifying the edges of its grid area.                                                 |
| gridColumnGap  |         string          |    -     |                                                                                              -                                                                                              |    -    | sets the size of the gap (gutter) between an element's columns.                                                                                                                                                                                                                                                                          |
|    gridGap     |         string          |    -     |                                                                                              -                                                                                              |    -    | sets the gaps (gutters) between rows and columns. It is a shorthand for row-gap and column-gap.                                                                                                                                                                                                                                          |
|   gridRowGap   |         string          |    -     |                                                                                              -                                                                                              |    -    | sets the size of the gap (gutter) between an element's grid rows.                                                                                                                                                                                                                                                                        |
|   heightProp   |         string          |    -     |                                                                                              -                                                                                              | 100%    | defines the height of the border of the element area.                                                                                                                                                                                                                                                                                    |
| justifyContent |         string          |    -     |                                                                                              -                                                                                              |    -    | defines how the browser distributes space between and around content items along the main-axis of a flex container, and the inline axis of a grid container.                                                                                                                                                                             |
|  justifyItems  |         string          |    -     |                                                                                              -                                                                                              |    -    | defines the default justify-self for all items of the box, giving them all a default way of justifying each box along the appropriate axis.                                                                                                                                                                                              |
|  justifySelf   |         string          |    -     |                                                                                              -                                                                                              |    -    | sets the way a box is justified inside its alignment container along the appropriate axis.                                                                                                                                                                                                                                               |
|   marginProp   |         string          |    -     |                                                                                              -                                                                                              |    -    | sets the margin area on all four sides of an element. It is a shorthand for margin-top, margin-right, margin-bottom, and margin-left.                                                                                                                                                                                                    |
|  paddingProp   |         string          |    -     |                                                                                              -                                                                                              |    -    | sets the padding area on all four sides of an element. It is a shorthand for padding-top, padding-right, padding-bottom, and padding-left                                                                                                                                                                                                |
|    rowsProp    |     string,array      |    -     |                                                                       "50px", ["100px", ["100px","1fr"], "auto"]                                                                              |    -    | defines the sizing of the grid rows. Specifying a single string will repeat several rows of this size. Or you can specify an array with rows sizes. The row size can be specified as an array of minimum and maximum heights.                                                                                                            |
|   widthProp    |         string          |    -     |                                                                                              -                                                                                              | 100%   | defines the width of the border of the element area.                                                                                                                                                                                                                                                                                     |

        `,
      },
      source: {
        code: `import Grid from "@appserver/components/grid";

<Grid {...args} {...gridProps}>
  <Box {...boxProps} backgroundProp="#F4991A">
    <Text>200px</Text>
  </Box>
  <Box {...boxProps} backgroundProp="#F2EAD3">
    <Text>minmax(100px,1fr)</Text>
  </Box>
  <Box {...boxProps} backgroundProp="#F9F5F0">
    <Text>auto</Text>
  </Box>
</Grid>`,
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
        <Text>200px</Text>
      </Box>
      <Box {...boxProps} backgroundProp="#F2EAD3">
        <Text>minmax(100px,1fr)</Text>
      </Box>
      <Box {...boxProps} backgroundProp="#F9F5F0">
        <Text>auto</Text>
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
          <Text>200px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>auto</Text>
        </Box>
      </Grid>

      <Grid {...args} {...gridProps} columnsProp="25%">
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>25%</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>25%</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>25%</Text>
        </Box>
      </Grid>

      <Grid {...args} {...gridProps} columnsProp={{ count: 3, size: "100px" }}>
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>100px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>100px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>100px</Text>
        </Box>
      </Grid>

      <Grid
        {...args}
        {...gridProps}
        columnsProp={{ count: 3, size: ["100px", "1fr"] }}
      >
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>minmax(100px,1fr)</Text>
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
          <Text>100px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>auto</Text>
        </Box>
      </Grid>

      <Grid {...args} {...gridProps} rowsProp="50px">
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>50px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>50px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>50px</Text>
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
        <Text>header</Text>
      </Box>
      <Box {...boxProps} gridArea="navbar" backgroundProp="#F2EAD3">
        <Text>navbar</Text>
      </Box>
      <Box {...boxProps} gridArea="main" backgroundProp="#F9F5F0">
        <Text>main</Text>
      </Box>
      <Box {...boxProps} gridArea="sidebar" backgroundProp="#F2EAD3">
        <Text>sidebar</Text>
      </Box>
      <Box {...boxProps} gridArea="footer" backgroundProp="#F4991A">
        <Text>footer</Text>
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
/*
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Grid from ".";
import Box from "../box";
import Text from "../text";

const gridProps = {
  marginProp: "0 0 20px 0",
};

const boxProps = {
  paddingProp: "10px",
  displayProp: "flex",
  alignItems: "center",
  justifyContent: "center",
};

storiesOf("Components|Grid", module)
  .addDecorator(withReadme(Readme))
  .add("Columns", () => (
    <>
      <Grid {...gridProps} columnsProp={["200px", ["100px", "1fr"], "auto"]}>
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>200px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>auto</Text>
        </Box>
      </Grid>

      <Grid {...gridProps} columnsProp="25%">
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>25%</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>25%</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>25%</Text>
        </Box>
      </Grid>

      <Grid {...gridProps} columnsProp={{ count: 3, size: "100px" }}>
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>100px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>100px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>100px</Text>
        </Box>
      </Grid>

      <Grid {...gridProps} columnsProp={{ count: 3, size: ["100px", "1fr"] }}>
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>minmax(100px,1fr)</Text>
        </Box>
      </Grid>
    </>
  ))
  .add("Rows", () => (
    <>
      <Grid {...gridProps} rowsProp={["100px", ["100px", "1fr"], "auto"]}>
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>100px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>minmax(100px,1fr)</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>auto</Text>
        </Box>
      </Grid>

      <Grid {...gridProps} rowsProp="50px">
        <Box {...boxProps} backgroundProp="#F4991A">
          <Text>50px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F2EAD3">
          <Text>50px</Text>
        </Box>
        <Box {...boxProps} backgroundProp="#F9F5F0">
          <Text>50px</Text>
        </Box>
      </Grid>
    </>
  ))
  .add("Layout", () => (
    <Grid
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
        <Text>header</Text>
      </Box>
      <Box {...boxProps} gridArea="navbar" backgroundProp="#F2EAD3">
        <Text>navbar</Text>
      </Box>
      <Box {...boxProps} gridArea="main" backgroundProp="#F9F5F0">
        <Text>main</Text>
      </Box>
      <Box {...boxProps} gridArea="sidebar" backgroundProp="#F2EAD3">
        <Text>sidebar</Text>
      </Box>
      <Box {...boxProps} gridArea="footer" backgroundProp="#F4991A">
        <Text>footer</Text>
      </Box>
    </Grid>
  ));
*/
