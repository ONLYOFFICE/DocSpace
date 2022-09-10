import { useState } from "react";
import ComboBox from "@docspace/components/combobox";
import { options } from "./options";
import Box from "@docspace/components/box";
import SearchInput from "./../search-input/index";
import TextInput from "@docspace/components/text-input";

export const InputPhone = () => {
  const [phoneValue, setPhoneValue] = useState("");

  const handleChange = (e) => {
    setPhoneValue(e.target.value.replace(/\D/g, ""));
  };

  const newOptions = [];

  options.map((option) => {
    newOptions.push({
      key: option.code,
      label: option.name,
      icon: option.flag,
    });
  });

  return (
    <Box displayProp="flex" alignItems="center" widthProp="330px">
      <Box style={{ width: "50px", marginRight: "-10px" }}>
        <ComboBox
          onSelect={() => console.log("selected")}
          options={newOptions}
          advancedOption={<SearchInput />}
          scaled={true}
          scaledOptions={true}
          selectedOption={newOptions[2]}
          fillIcon={false}
        />
      </Box>
      <TextInput
        type="text"
        value={phoneValue}
        onChange={handleChange}
        style={{ paddingLeft: "15px" }}
      />
    </Box>
  );
};
