module.exports = {
  core: {
    builder: "webpack5",
  },
  stories: [
    "../all/all.stories.js",
    // default page
    "../**/*.stories.@(js|jsx|ts|tsx)", //"../**/*.stories.@(js|mdx)",
  ],

  staticDirs: ["../../../public"],
  addons: [
    "@storybook/addon-links",
    "@storybook/addon-essentials",
    "@storybook/addon-actions",
    "@storybook/addon-controls",
    "@storybook/addon-viewport",
    "@storybook/addon-contexts/register",
    "@react-theming/storybook-addon",
    {
      name: "@storybook/addon-docs",
      options: {
        babelOptions: {
          plugins: [
            [
              "@babel/plugin-proposal-private-property-in-object",
              {
                loose: true,
              },
            ],
          ],
        },
      },
    },
    "@storybook/addon-mdx-gfm",
  ],
  framework: {
    name: "@storybook/react-webpack5",
    options: {},
  },
  docs: {
    autodocs: true,
  },
};
