const imageLoader = {
  test: /\.(png|jpe?g|gif|ico)$/i,
  type: "asset/resource",
  generator: {
    filename: "static/images/[hash][ext][query]",
  },
};

const mJsLoader = {
  test: /\.m?js/,
  type: "javascript/auto",
  resolve: {
    fullySpecified: false,
  },
};

const svgLoader = {
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
};

const jsonLoader = { test: /\.json$/, loader: "json-loader" };

const cssLoader = {
  test: /\.css$/i,
  use: ["style-loader", "css-loader"],
};

const preprocessorStyleLoader = {
  test: /\.s[ac]ss$/i,
  use: [
    // Creates `style` nodes from JS strings
    "style-loader",
    // Translates CSS into CommonJS
    {
      loader: "css-loader",
      options: {
        url: {
          filter: (url, resourcePath) => {
            // resourcePath - path to css file

            // Don't handle `/static` urls
            if (url.startsWith("/static") || url.startsWith("data:")) {
              return false;
            }

            return true;
          },
        },
      },
    },
    // Compiles Sass to CSS
    "sass-loader",
  ],
};

const babelLoader = {
  test: /\.(js|jsx)$/,
  exclude: /node_modules/,
  use: [
    {
      loader: "babel-loader",
      options: {
        presets: ["@babel/preset-react", "@babel/preset-env"],
        plugins: [
          "@babel/plugin-transform-runtime",
          "@babel/plugin-proposal-class-properties",
          "@babel/plugin-proposal-export-default-from",
        ],
      },
    },
    "source-map-loader",
  ],
};

const client = [
  imageLoader,
  mJsLoader,
  svgLoader,
  jsonLoader,
  cssLoader,
  preprocessorStyleLoader,
  babelLoader,
];

module.exports = {
  client,
};
