import { useState } from "react";
import { options } from "./options";
import { InputPhone } from "./index";

export default {
  title: "Components/InputPhone",
  component: InputPhone,
};

const Template = ({ onChange, value, ...args }) => {
  const [val, setValue] = useState(value);

  const onChangeHandler = (e) => {
    onChange(e.target.value);
    setValue(e.target.value);
    console.log(e.target.val);
  };
  return (
    <div style={{ height: "300px" }}>
      <InputPhone {...args} value={val} onChange={onChangeHandler} />
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
  phonePlaceholderText: "+7 XXX XXX-XX-XX",
  searchPlaceholderText: "Search",
  searchEmptyMessage: "Nothing found",
  errorMessage: "Сountry code is invalid",
};
