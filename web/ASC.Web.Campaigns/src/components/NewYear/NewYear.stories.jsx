import React from 'react';
import { Layout } from '../Layout';

export default {
  title: 'Example/NewYear',
  component: Layout,
  parameters: {
    layout: 'fullscreen',
  },
  argTypes: {
    theme: {
      control: {
        type: "select",
        options: ["light", "dark"]
      }
    },
    language: {
      control: {
        type: "select",
        options: ["en", "ru"]
      }
    }
  }
};

const Template = (args) => <Layout {...args} />;

export const Default = Template.bind({});

Default.args = {
  origin: "http://localhost:8001", // use your source
  name: "NewYear",
  language: "en",
  theme: "light"
}
