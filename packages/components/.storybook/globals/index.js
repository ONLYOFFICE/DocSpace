const globalTypes = {
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

  locale: {
    name: "Locale",
    description: "Internationalization locale",
    toolbar: {
      icon: "globe",
      items: [
        { value: "en", title: "English" },
        { value: "ru", title: "Русский" },
      ],
      showName: true,
    },
  },
};

export default globalTypes;
