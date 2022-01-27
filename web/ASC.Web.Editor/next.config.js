const serverConfig = require("./server.config.json");
module.exports = {
  //future: { webpack5: true },
  env: {
    remoteApiUrl: serverConfig.remoteApiUrl,
  },
  webpack: (config, options) => {
    return {
      ...config,
      plugins: [...config.plugins],
      experiments: { topLevelAwait: true },
      module: {
        rules: [
          ...config.module.rules,

          {
            test: /\.(png|jpe?g|gif|ico)$/i,
            type: "asset/resource",
            generator: {
              filename: "static/images/[hash][ext][query]",
            },
          },
          {
            test: /\.m?js/,
            type: "javascript/auto",
            resolve: {
              fullySpecified: false,
            },
          },
          {
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
          },
          { test: /\.json$/, loader: "json-loader" },
          {
            test: /\.css$/i,
            use: ["style-loader", "css-loader"],
          },
          {
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
                      if (
                        url.startsWith("/static") ||
                        url.startsWith("data:")
                      ) {
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
          },

          {
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
          },
        ],
      },
    };
  },

  webpackDevMiddleware: (config) => {
    // Perform customizations to webpack dev middleware config
    // Important: return the modified config
    return config;
  },
};
