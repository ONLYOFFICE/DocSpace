import { useState } from "react";
import { options } from "./options";
// import { FixedSizeList as List } from 'react-window';
import Box from "@docspace/components/box";
import InputBlock from "@docspace/components/input-block";
import {
  StyledBox,
  StyledComboBox,
  StyledInput,
  StyledDropDown,
  StyledDropDownItem,
  CountryFlag,
  CountryName,
  CountryDialCode,
} from "./styled-input-phone";

export const InputPhone = () => {
  const [phoneValue, setPhoneValue] = useState("+");
  const [searchValue, setSearchValue] = useState("");
  const [isOpen, setIsOpen] = useState(false);

  const handleChange = (e) => {
    const onlyDigits = e.target.value.replace(/[^+\d]/g, "");
    if (onlyDigits === "") {
      setPhoneValue("+");
    } else {
      setPhoneValue(onlyDigits);
    }
  };

  // const mask = [
  //   "+",
  //   /\d/,
  //   " ",
  //   /\d/,
  //   /\d/,
  //   /\d/,
  //   " ",
  //   /\d/,
  //   /\d/,
  //   /\d/,
  //   "-",
  //   /\d/,
  //   /\d/,
  //   "-",
  //   /\d/,
  //   /\d/,
  // ];

  return (
    <StyledBox displayProp="flex" alignContent="center">
      <Box>
        <StyledComboBox
          onClick={() => setIsOpen(!isOpen)}
          options={[]}
          scaled={true}
          selectedOption={{
            key: options[0].code,
            label: "Flag",
          }}
        />
      </Box>
      <StyledInput
        type="tel"
        placeholder="+7 XXX XXX-XX-XX"
        // mask={mask}
        maxLength={20}
        isAutoFocussed={true}
        value={phoneValue}
        onChange={handleChange}
        className="phone-input"
      />

      <StyledDropDown
        open={isOpen}
        clickOutsideAction={() => setIsOpen(!isOpen)}
        isDefaultMode={false}
        // maxHeight={200}
        manualWidth="100%"
      >
        <InputBlock
          type="text"
          iconName="static/images/search.react.svg"
          placeholder="Search"
          value={searchValue}
          scale={true}
          onChange={(e) => setSearchValue(e.target.value)}
          style={{ height: "32px" }}
        />

        {options
          .filter(
            (val) =>
              val.name.toLowerCase().includes(searchValue) ||
              val.dialCode.includes(searchValue)
          )
          .map((country) => (
            <StyledDropDownItem
              key={country.code}
              onClick={() => {
                setPhoneValue(country.dialCode), setIsOpen(!isOpen);
              }}
            >
              <CountryFlag>Flag</CountryFlag>
              <CountryName>{country.name}</CountryName>
              <CountryDialCode>{country.dialCode}</CountryDialCode>
            </StyledDropDownItem>
          ))}
      </StyledDropDown>
    </StyledBox>
  );
};
