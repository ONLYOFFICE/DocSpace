import resolve from "rollup-plugin-node-resolve";
import commonjs from "rollup-plugin-commonjs";
import json from "rollup-plugin-json";
import babel from "rollup-plugin-babel";
import cleanup from "rollup-plugin-cleanup";
import replace from "rollup-plugin-replace";
import postcss from 'rollup-plugin-postcss';
import svgrPlugin from "@svgr/rollup";
import pkg from "./package.json";

const getBabelPreset = require("./scripts/get-babel-preset");

const babelOptions = getBabelPreset();

// This list includes common plugins shared between each output format.
// NOTE: the order of the plugins is important!
const configureRollupPlugins = (options = {}) => [
  replace({
    "process.env.NODE_ENV": JSON.stringify("production")
  }),
  // To use the nodejs `resolve` algorithm
  resolve(),
  // See also https://medium.com/@kelin2025/so-you-wanna-use-es6-modules-714f48b3a953
  // Transpile sources using our custom babel preset.
  babel({
    exclude: ["node_modules/**"],
    runtimeHelpers: true,
    ...babelOptions,
    plugins: [
      ...babelOptions.plugins,
      ...(options.babel && options.babel.plugins ? options.babel.plugins : [])
    ]
  }),
  // To convert CJS modules to ES6
  commonjs({
    include: "node_modules/**"
  }),
  // To convert JSON files to ES6
  json(),
  // To convert SVG Icons to ES6
  svgrPlugin({
    // NOTE: only the files ending with `.react.svg` are supposed to be
    // converted to React components
    include: ["**/*.react.svg"],
    icon: false,
    svgoConfig: {
      plugins: [
        { removeViewBox: false },
        // Keeps ID's of svgs so they can be targeted with CSS
        { cleanupIDs: false }
      ]
    }
  }),
  postcss({
    extensions: ['.css'],
  }),
  // To remove comments, trim trailing spaces, compact empty lines,
  // and normalize line endings
  cleanup()
];

const deps = Object.keys(pkg.dependencies || {});
const peerDeps = Object.keys(pkg.peerDependencies || {});
const defaultExternal = deps.concat(peerDeps);

// We need to define 2 separate configs (`esm` and `cjs`) so that each can be
// further customized.
const config = [
  {
    input: "src/index.js",
    external: defaultExternal,
    output: {
      file: pkg.main,
      format: "cjs"
    },
    plugins: configureRollupPlugins()
  },
  {
    input: "src/index.js",
    external: defaultExternal,
    output: {
      file: pkg.module,
      format: "esm"
    },
    plugins: configureRollupPlugins({
      babel: {
        plugins: [
          [
            "transform-rename-import",
            {
              replacements: [{ original: "lodash", replacement: "lodash-es" }]
            }
          ]
        ]
      }
    })
  }
];

export default config;
