import React from 'react';
import { Layout } from '../Layout';

export default {
  title: 'Example/Docs_7_1',
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
        options: ["en", "ru", "cs", "de", "es", "fr", "it", "ja", "nl", "pt", "zh"]
      }
    }
  }
};

const Template = (args) => <Layout {...args} />;

export const Default = Template.bind({});

Default.args = {
  origin: "http://localhost:8000", // use your source
  name: "Docs_7_1",
  language: "en",
  theme: "light"
}