import React, { useState } from "react";
import SearchInput from ".";
import Button from "../button";

export default {
  title: "Components/SearchInput",
  component: SearchInput,
  argTypes: {
    onChange: { action: "onChange" },
  },
};

const Template = ({ value, onChange, ...args }) => {
  const [searchValue, setSearchValue] = useState(value);

  return (
    <>
      <SearchInput
        {...args}
        value={searchValue}
        onChange={(value) => {
          onChange(value);
          setSearchValue(value);
        }}
      />
    </>
  );
};

export const Default = Template.bind({});
Default.args = {
  id: "",
  isDisabled: false,
  size: "base",
  scale: false,
  placeholder: "Search",
  value: "",
};
