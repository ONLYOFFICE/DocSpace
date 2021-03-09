module.exports = ({ config }) => {
  const rules = config.module.rules;

  const fileLoaderRule = rules.find((rule) => rule.test.test(".svg"));
  fileLoaderRule.exclude = /\.react.svg$/;

  rules.push({
    test: /\.react.svg$/,
    use: [
      {
        loader: "@svgr/webpack",
        options: {
          svgoConfig: {
            plugins: [{ removeViewBox: false }],
          },
        },
      },
    ],
  });

  return config;
};
