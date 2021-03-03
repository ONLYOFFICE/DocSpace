module.exports = {
  stories: [
    "../backdrop/*.stories.@(js|jsx|ts|tsx)",
    "../button/*.stories.@(js|jsx|ts|tsx)",
    "../avatar/*.stories.@(js|jsx|ts|tsx)",
    "../badge/*.stories.@(js|jsx|ts|tsx)",
    "../box/*.stories.@(js|jsx|ts|tsx)",
    "../avatar-editor/*.stories.@(js|jsx|ts|tsx)",
    "../calendar/*.stories.@(js|jsx|ts|tsx)",
    "../checkbox/*.stories.@(js|jsx|ts|tsx)",
    "../combobox/*.stories.@(js|mdx)",
    "../context-menu/*.stories.@(js|mdx)",
    "../context-menu-button/*.stories.@(js|mdx)",
  ],
  addons: [
    "@storybook/addon-links",
    "@storybook/addon-docs",
    "@storybook/addon-essentials",
    "@storybook/addon-actions",
    "@storybook/addon-controls",
  ],
};
