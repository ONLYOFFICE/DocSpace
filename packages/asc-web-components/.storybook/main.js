module.exports = {
  stories: ["../**/*.stories.@(js|mdx)"],
  addons: [
    "@storybook/addon-links",
    "@storybook/addon-docs",
    "@storybook/addon-essentials",
    "@storybook/addon-actions",
    "@storybook/addon-controls",
    "@storybook/addon-viewport",
    "@storybook/addon-contexts/register",
  ],
};
