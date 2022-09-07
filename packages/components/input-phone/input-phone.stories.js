import { InputPhone } from ".";
import { deviceType } from "react-device-detect";

export default {
  title: "Components/InputPhone",
  component: InputPhone,
};

const Template = (args) => (
  <div style={{ height: "500px" }}>
    <InputPhone {...args} />
  </div>
);

export const Default = Template.bind({});

Default.args = {
  country: "ru",
  enableSearch: true,
};
