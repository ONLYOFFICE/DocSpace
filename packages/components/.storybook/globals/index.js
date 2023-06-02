const globalTypes = {
  theme: {
    description: "Global theme for components",
    defaultValue: "Light",
    toolbar: {
      title: "Theme",
      icon: "photo",
      items: ["Light", "Dark"],
      dynamicTitle: true,
    },
  },
};

export default globalTypes;
