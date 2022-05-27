module.exports = {
  stories: ["../**/*.stories.@(js|mdx)"],
  addons: [
    "@storybook/addon-links",
    "@storybook/addon-essentials",
    "@storybook/addon-actions",
    "@storybook/addon-controls",
    "@storybook/addon-viewport",
    "@storybook/addon-contexts/register",
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
  ],
};
