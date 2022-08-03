import React from "react";

import Box from "./";

const containerProps = {
  widthProp: "100%",
  paddingProp: "10px",
  displayProp: "flex",
  flexDirection: "column",
  alignItems: "flex-start",
};

const rowProps = {
  displayProp: "flex",
  flexDirection: "row",
};

const commonBoxProps = {
  textAlign: "center",
  marginProp: "10px",
  paddingProp: "10px",
};

export default {
  title: "Components/Box",
  component: Box,
  parameters: {
    docs: {
      description: {
        component:
          "A container that lays out its contents in one direction. Box provides general CSS capabilities like flexbox layout, paddings, background color, border, and animation.",
      },
    },
  },
};

const Template = (args) => <Box {...args}>Example</Box>;

export const Default = Template.bind({});
Default.args = {
  widthProp: "100%",
  paddingProp: "10px",
  displayProp: "flex",
  flexDirection: "column",
  alignItems: "flex-start",
  borderProp: "4px dashed gray",
};

const ExamplesTemplate = (args) => (
  <Box {...containerProps}>
    <Box {...rowProps}>
      <Box {...commonBoxProps} backgroundProp="gray">
        color background
      </Box>
      <Box
        {...commonBoxProps}
        backgroundProp="linear-gradient(90deg, white, gray)"
      >
        linear gradient background
      </Box>
      <Box {...commonBoxProps} backgroundProp="radial-gradient(white, gray)">
        radial gradient background
      </Box>
    </Box>
    <Box {...rowProps}>
      <Box {...commonBoxProps} borderProp="4px solid gray">
        solid border
      </Box>
      <Box {...commonBoxProps} borderProp="4px dashed gray">
        dashed border
      </Box>
      <Box {...commonBoxProps} borderProp="4px dotted gray">
        dotted border
      </Box>
      <Box {...commonBoxProps} borderProp="4px double gray">
        double border
      </Box>
    </Box>
    <Box {...rowProps}>
      <Box
        {...commonBoxProps}
        borderProp={{ style: "solid", width: "1px 0", color: "gray" }}
      >
        Horizontal border
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "0 1px",
          color: "gray",
        }}
      >
        vertical border
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "0 0 0 1px",
          color: "gray",
        }}
      >
        left border
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "1px 0 0 0",
          color: "gray",
        }}
      >
        top border
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "0 1px 0 0",
          color: "gray",
        }}
      >
        right border
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "0 0 1px 0",
          color: "gray",
        }}
      >
        bottom border
      </Box>
    </Box>
    <Box {...rowProps}>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "1px",
          color: "gray",
          radius: "100%",
        }}
      >
        full round
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "1px",
          color: "gray",
          radius: "5px",
        }}
      >
        round
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "1px",
          color: "gray",
          radius: "5px 0 0 5px",
        }}
      >
        left round
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "1px",
          color: "gray",
          radius: "5px 5px 0 0",
        }}
      >
        top round
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "1px",
          color: "gray",
          radius: "0 5px 5px 0",
        }}
      >
        right round
      </Box>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "solid",
          width: "1px",
          color: "gray",
          radius: "0 0 5px 5px",
        }}
      >
        bottom round
      </Box>
    </Box>
    <Box {...rowProps}>
      <Box
        {...commonBoxProps}
        borderProp={{
          style: "dashed solid double dotted",
          width: "2em 1rem 1px 2%",
          color: "red yellow green blue",
          radius: "10% 30% 50% 70%",
        }}
      >
        Mix border
      </Box>
    </Box>
  </Box>
);

export const Examples = ExamplesTemplate.bind({});
