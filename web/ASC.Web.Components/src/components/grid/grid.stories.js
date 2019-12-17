import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Grid from '.';
import Box from '../box';

storiesOf('Components|Grid', module)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Grid
      widthProp="100%"
      heightProp="100vh"
      gridGap="10px"
      rowsProp={["auto", "1fr", "auto"]}
      columnsProp={[["100px","1fr"], "3fr", "100px"]}
      areasProp={[
        { name: "header", start: [0, 0], end: [2, 0] },
        { name: "navbar", start: [0, 1], end: [0, 1] },
        { name: "main", start: [1, 1], end: [1, 1] },
        { name: "sidebar", start: [2, 1], end: [2, 1] },
        { name: "footer", start: [0, 2], end: [2, 2] }
      ]}
    >
      <Box
        gridArea="header"
        displayProp="flex"
        alignItems="center"
        justifyContent="center"
        backgroundProp="#F4991A"
        paddingProp="30px"
      >
        header
      </Box>
      <Box
        gridArea="navbar"
        displayProp="flex"
        alignItems="center"
        justifyContent="center"
        backgroundProp="radial-gradient(red, blue)"
      >
        navbar
      </Box>
      <Box
        gridArea="main"
        borderProp={{ 
          style: "dashed solid double dotted",
          width: "2em 1rem 1px 2%",
          color: "red yellow green blue",
          radius: "10% 30% 50% 70%"
        }}
        displayProp="flex"
        alignItems="center"
        justifyContent="center"
        backgroundProp="#F9F5F0"
      >
        main
      </Box>
      <Box
        gridArea="sidebar"
        displayProp="flex"
        alignItems="center"
        justifyContent="center"
        backgroundProp="#F2EAD3"
      >
        sidebar
      </Box>
      <Box
        gridArea="footer"
        displayProp="flex"
        alignItems="center"
        justifyContent="center"
        backgroundProp="#F4991A"
        marginProp="30px"
      >
        footer
      </Box>
    </Grid>
  ));
