import { InputPhone } from ".";

export default {
  title: "InputPhone",
  component: InputPhone,
};

const Template = (args) => <InputPhone {...args} />;

export const Default = Template.bind({});

Default.args = {
  defaultCountry: "ru",
  placeholder: "XXX XXX-XX-XX",
};
