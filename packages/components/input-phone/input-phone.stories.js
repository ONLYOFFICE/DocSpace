import { useState } from "react";
import { options } from "./options";
import InputPhone from "./index";

export default {
  title: "Components/InputPhone",
  component: InputPhone,
  argTypes: {
    onChange: { control: "onChange" },
  },
};

const Template = ({ onChange, value, ...args }) => {
  const [val, setValue] = useState(value);

  return (
    <div style={{ height: "300px" }}>
      <InputPhone
        {...args}
        value={val}
        onChange={(e) => {
          setValue(e.target.value), onChange(e.target.value);
        }}
      />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  defaultCountry: {
    locale: options[182].code, // default locale RU
    dialCode: options[182].dialCode, // default dialCode +7
    mask: options[182].mask, // default dialCode +7
    icon: options[182].flag, // default flag Russia
  },
  phonePlaceholderText: "7 XXX XXX-XX-XX",
  searchPlaceholderText: "Search",
  scaled: false,
  searchEmptyMessage: "Nothing found",
  errorMessage: "Сountry code is invalid",
};
