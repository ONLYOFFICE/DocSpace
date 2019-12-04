import React from 'react';
import { storiesOf } from '@storybook/react';
import Grid from '.';
import Box from '../box';

storiesOf('Components|Grid', module)
  .add('base', () => (
    <Grid
      widthProp="100%"
      heightProp="100vh"
      gridGap="10px"
      rowsProp={["auto", "1fr", "auto"]}
      columnsProp={["200px", "1fr", "200px"]}
      areasProp={[
        { name: "header", start: [0, 0], end: [2, 0] },
        { name: "navbar", start: [0, 1], end: [0, 1] },
        { name: "main", start: [1, 1], end: [1, 1] },
        { name: "sidebar", start: [2, 1], end: [2, 1] },
        { name: "footer", start: [0, 2], end: [2, 2] }
      ]}
    >
      <Box gridArea="header" displayProp="flex" alignItems="center" justifyContent="center" style={{background: "#F4991A"}}>header</Box>
      <Box gridArea="navbar" displayProp="flex" alignItems="center" justifyContent="center" style={{background: "#F2EAD3"}}>navbar</Box>
      <Box gridArea="main" displayProp="flex" alignItems="center" justifyContent="center" style={{background: "#F9F5F0"}}>main</Box>
      <Box gridArea="sidebar" displayProp="flex" alignItems="center" justifyContent="center" style={{background: "#F2EAD3"}}>sidebar</Box>
      <Box gridArea="footer" displayProp="flex" alignItems="center" justifyContent="center" style={{background: "#F4991A"}}>footer</Box>
    </Grid>
  ));
