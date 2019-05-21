import React from 'react';
import { configure } from '@storybook/react';
import { addDecorator } from '@storybook/react';
import { withConsole } from '@storybook/addon-console';
import 'bootstrap/dist/css/bootstrap.css';

const sectionStyles = {
  padding: 16,
};

const SectionDecorator = storyFn => <div style={sectionStyles}>{storyFn()}</div>;

// automatically import all files ending in *.stories.js
const req = require.context('../stories', true, /\.stories\.js$/);
function loadStories() {
  req.keys().forEach(filename => req(filename));
}

addDecorator(SectionDecorator);

addDecorator((storyFn, context) => withConsole()(storyFn)(context));

configure(loadStories, module);

