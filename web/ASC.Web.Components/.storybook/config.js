import { configure, addDecorator, addParameters } from '@storybook/react';
import { withA11y } from '@storybook/addon-a11y';
import { addReadme } from 'storybook-readme';
import { withConsole } from '@storybook/addon-console';

import '!style-loader!css-loader!./styles.scss';

//import 'bootstrap/dist/css/bootstrap.css';
//import 'react-toastify/dist/ReactToastify.min.css';
/*
This is a package to make Story panel load and decode all files stories
Also, because of some internal usage, we cannot use the default babel config (for the library)
with this package.

In order to solve that, it's necessary a custom/simple .babelrc inside .storybook/ folder
*/
//import requireContext from 'require-context.macro';

/* Add A11y panel */
addDecorator(withA11y);

/* Enable README for all stories */
addDecorator(addReadme);

/* General options for storybook */
addParameters({
  options:
  {
    name: 'ASC Web Components Storybook',
    //sortStoriesByKind: true,
    storySort: (a, b) => a[1].kind === b[1].kind ? 0 : a[1].id.localeCompare(b[1].id, { numeric: true }),
    showAddonPanel: true,
    addonPanelInRight: true
  },
  /* Options for storybook-readme plugin */
  readme: {
    codeTheme: 'github',
    StoryPreview: ({ children }) => children,
  },
});

/* automatically import all files ending in *.stories.js inside src folder */
//const req = requireContext('../src', true, /\.stories\.js$/);
//const req = require.context('../src', true, /\.stories\.js$/);
const srcStories = require.context('../src', true, /\.stories\.js$/);
function loadStories() {
  //req.keys().forEach(filename => req(filename));
  srcStories.keys().forEach(filename => srcStories(filename));
}

addDecorator((storyFn, context) => withConsole()(storyFn)(context));

configure(loadStories, module);
