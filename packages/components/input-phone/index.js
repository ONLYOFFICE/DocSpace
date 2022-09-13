import { useState } from "react";
import { options } from "./options";
import ComboBox from "@docspace/components/combobox";
import Box from "@docspace/components/box";
import SearchInput from "@docspace/components/search-input";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";
import TextInput from "@docspace/components/text-input";
import styled from "styled-components";

const StyledComboBox = styled(ComboBox)`
  .combo-button-label {
    font-weight: 400;
    line-height: 22px;
  }
  .combo-button {
    height: 44px;
    border-right: 0;
    border-top-right-radius: 0;
    border-bottom-right-radius: 0;
  }
`;

const CountryTitle = styled.h3`
  font-size: 13px;
  font-weight: 400;
  color: #33333;
  margin: 0;
  margin-right: 5px;
  line-height: 20px;
`;

const CountryDialCode = styled.p`
  font-size: 13px;
  font-weight: 400;
  line-height: 20px;
  margin: 0;
  color: #a3a9ae;
`;

export const InputPhone = () => {
  const [phoneValue, setPhoneValue] = useState("");
  const [open, setOpen] = useState(false);

  const handleChange = (e) => {
    setPhoneValue(e.target.value.replace(/\D/g, ""));
  };

  // const newOptions = [];

  // options.map((option) => {
  //   newOptions.push({
  //     key: option.code,
  //     icon: option.flag,
  //     label: option.name,
  //   });
  // });

  return (
    <Box
      displayProp="flex"
      alignContent="center"
      widthProp="320px"
      style={{ position: "relative" }}
    >
      <Box>
        <StyledComboBox
          onClick={() => setOpen(true)}
          options={[]}
          scaled={true}
          selectedOption={{
            key: options[0].code,
            label: options[0].dialCode,
          }}
          style={{ cursor: "pointer" }}
        />
      </Box>
      <TextInput
        type="text"
        value={phoneValue}
        onChange={handleChange}
        style={{
          paddingLeft: "10px",
          borderTopLeftRadius: "0",
          borderBottomLeftRadius: "0",
        }}
      />
      <DropDown
        open
        isDefaultMode={false}
        style={{
          padding: "12px 16px",
          width: "320px",
          height: "165px",
          boxSizing: "border-box",
          marginTop: "4px",
        }}
      >
        <SearchInput />
        {options.map((country) => {
          return (
            <DropDownItem
              style={{
                padding: "9px 0",
                boxSizing: "border-box",
              }}
            >
              <CountryTitle>{country.name}</CountryTitle>
              <CountryDialCode>{country.dialCode}</CountryDialCode>
            </DropDownItem>
          );
        })}
      </DropDown>
    </Box>
  );
};
