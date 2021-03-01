module.exports = {
  stories: [
    "../backdrop/*.stories.mdx",
    "../backdrop/*.stories.@(js|jsx|ts|tsx)",
    //"../button/*.stories.mdx",
    "../button/*.stories.@(js|jsx|ts|tsx)",
    //"../stories/*.stories.mdx",
    //"../stories/*.stories.@(js|jsx|ts|tsx)",
  ],
  addons: [
    "@storybook/addon-links",
    "@storybook/addon-docs",
    "@storybook/addon-essentials",
    "@storybook/addon-actions",
  ],
};
