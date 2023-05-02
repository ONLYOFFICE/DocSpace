module.exports = {
  stories: ["../all/all.stories.js",
  // default page
  "../**/*.stories.js" //"../**/*.stories.@(js|mdx)",
  ],

  staticDirs: ["../../../public"],
  addons: ["@storybook/addon-links", "@storybook/addon-essentials", "@storybook/addon-actions", "@storybook/addon-controls", "@storybook/addon-viewport", "@storybook/addon-contexts/register"
  // {
  //   name: "@storybook/addon-docs",
  //   options: {
  //     babelOptions: {
  //       plugins: [
  //         [
  //           "@babel/plugin-proposal-private-property-in-object",
  //           {
  //             loose: true,
  //           },
  //         ],
  //       ],
  //     },
  //   },
  // },
  , "@storybook/addon-mdx-gfm"],
  framework: {
    name: "@storybook/react-webpack5",
    options: {}
  },
  docs: {
    autodocs: true
  }
};