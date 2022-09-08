import { InputPhone } from ".";

export default {
  title: "Components/InputPhone",
  component: InputPhone,
};

const Template = (args) => <InputPhone {...args} />;

export const Default = Template.bind({});

Default.args = {
  country: "ru",
  enableSearch: true,
};
