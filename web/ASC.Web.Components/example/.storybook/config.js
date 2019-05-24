import React from 'react';
import { configure } from '@storybook/react';
import { addDecorator } from '@storybook/react';
import { withConsole } from '@storybook/addon-console';
import 'bootstrap/dist/css/bootstrap.css';

// automatically import all files ending in *.stories.js
const req = require.context('../stories', true, /\.stories\.js$/);
function loadStories() {
  req.keys().forEach(filename => req(filename));
}

addDecorator((storyFn, context) => withConsole()(storyFn)(context));

configure(loadStories, module);

