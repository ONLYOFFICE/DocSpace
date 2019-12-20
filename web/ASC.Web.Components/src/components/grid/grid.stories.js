import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Grid from '.';
import Box from '../box';
import Text from "../text";

const gridProps = {
  marginProp: "0 0 20px 0"
};

const boxProps = {
  paddingProp: "10px",
  displayProp: "flex",
  alignItems: "center",
  justifyContent: "center"
};

storiesOf('Components|Grid', module)
  .addDecorator(withReadme(Readme))
  .add("Columns", () => (
    <>
      <Grid {...gridProps} columnsProp={["200px", ["100px","1fr"], "auto"]}>
        <Box {...boxProps} backgroundProp="#F4991A"><Text>200px</Text></Box>
        <Box {...boxProps} backgroundProp="#F2EAD3"><Text>minmax(100px,1fr)</Text></Box>
        <Box {...boxProps} backgroundProp="#F9F5F0"><Text>auto</Text></Box>
      </Grid>

      <Grid {...gridProps} columnsProp="25%">
        <Box {...boxProps} backgroundProp="#F4991A"><Text>25%</Text></Box>
        <Box {...boxProps} backgroundProp="#F2EAD3"><Text>25%</Text></Box>
        <Box {...boxProps} backgroundProp="#F9F5F0"><Text>25%</Text></Box>
      </Grid>
      
      <Grid {...gridProps} columnsProp={{ count: 3, size: "100px" }}>
        <Box {...boxProps} backgroundProp="#F4991A"><Text>100px</Text></Box>
        <Box {...boxProps} backgroundProp="#F2EAD3"><Text>100px</Text></Box>
        <Box {...boxProps} backgroundProp="#F9F5F0"><Text>100px</Text></Box>
      </Grid>

      <Grid {...gridProps} columnsProp={{ count: 3, size: ["100px", "1fr"] }}>
        <Box {...boxProps} backgroundProp="#F4991A"><Text>minmax(100px,1fr)</Text></Box>
        <Box {...boxProps} backgroundProp="#F2EAD3"><Text>minmax(100px,1fr)</Text></Box>
        <Box {...boxProps} backgroundProp="#F9F5F0"><Text>minmax(100px,1fr)</Text></Box>
      </Grid>
    </>  
  ))
  .add("Rows", () => (
    <>
      <Grid {...gridProps} rowsProp={["100px", ["100px","1fr"], "auto"]}>
        <Box {...boxProps} backgroundProp="#F4991A"><Text>100px</Text></Box>
        <Box {...boxProps} backgroundProp="#F2EAD3"><Text>minmax(100px,1fr)</Text></Box>
        <Box {...boxProps} backgroundProp="#F9F5F0"><Text>auto</Text></Box>
      </Grid>

      <Grid {...gridProps} rowsProp="50px">
        <Box {...boxProps} backgroundProp="#F4991A"><Text>50px</Text></Box>
        <Box {...boxProps} backgroundProp="#F2EAD3"><Text>50px</Text></Box>
        <Box {...boxProps} backgroundProp="#F9F5F0"><Text>50px</Text></Box>
      </Grid>
    </>  
  ))
  .add('Layout', () => (
    <Grid
      widthProp="100vw"
      heightProp="100vh"
      gridGap="10px"
      rowsProp={["auto", "1fr", "auto"]}
      columnsProp={[["100px","1fr"], "3fr", ["100px","1fr"]]}
      areasProp={[
        { name: "header", start: [0, 0], end: [2, 0] },
        { name: "navbar", start: [0, 1], end: [0, 1] },
        { name: "main", start: [1, 1], end: [1, 1] },
        { name: "sidebar", start: [2, 1], end: [2, 1] },
        { name: "footer", start: [0, 2], end: [2, 2] }
      ]}
    >
      <Box {...boxProps} gridArea="header" backgroundProp="#F4991A"><Text>header</Text></Box>
      <Box {...boxProps} gridArea="navbar" backgroundProp="#F2EAD3"><Text>navbar</Text></Box>
      <Box {...boxProps} gridArea="main" backgroundProp="#F9F5F0"><Text>main</Text></Box>
      <Box {...boxProps} gridArea="sidebar" backgroundProp="#F2EAD3"><Text>sidebar</Text></Box>
      <Box {...boxProps} gridArea="footer" backgroundProp="#F4991A"><Text>footer</Text></Box>
    </Grid>
  ));
