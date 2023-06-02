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
  direction: {
    description: "Interface direction",
    defaultValue: "ltr",
    toolbar: {
      title: "Dir",
      icon: "paragraph",
      items: ["ltr", "rtl"],
      dynamicTitle: true,
    },
  },
};

export default globalTypes;
