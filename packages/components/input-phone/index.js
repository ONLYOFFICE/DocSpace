import { useState } from "react";
import { options } from "./options";
// import { FixedSizeList as List } from 'react-window';
import ComboBox from "@docspace/components/combobox";
import Box from "@docspace/components/box";
import InputBlock from "@docspace/components/input-block";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";
import TextInput from "@docspace/components/text-input";
import styled from "styled-components";

const StyledComboBox = styled(ComboBox)`
  .combo-button-label {
    font-weight: 400;
    line-height: 22px;
    padding: 10px;
    text-align: center;
    margin: 0;
  }
  .combo-button {
    height: 44px;
    border-right: 0;
    border-top-right-radius: 0;
    border-bottom-right-radius: 0;
    cursor: pointer;
    padding-left: 0;
    &:focus-within {
      border-color: red;
    }
  }
`;

const StyledInput = styled(TextInput)`
  padding-left: 10px;
  border-top-left-radius: 0;
  border-bottom-left-radius: 0;
`;

const CountryFlag = styled.div`
  border: 1px solid #aeaeae;
  margin-right: 10px;
  padding: 0 5px;
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
  const [phoneValue, setPhoneValue] = useState("+");
  const [searchValue, setSearchValue] = useState("");
  const [isOpen, setIsOpen] = useState(false);

  const handleChange = (e) => {
    if (e.target.value === "") {
      setPhoneValue("+");
    } else {
      setPhoneValue(e.target.value);
    }
  };

  return (
    <Box
      displayProp="flex"
      alignContent="center"
      style={{ position: "relative", maxWidth: "320px" }}
    >
      <Box style={{ cursor: "pointer" }}>
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
        onKeyPress={(e) => {
          if (!/[0-9]/.test(e.key)) {
            e.preventDefault();
          }
        }}
        type="text"
        maxLength={15}
        value={phoneValue}
        onChange={handleChange}
        className="phone-input"
      />

      <DropDown
        // isDropdown
        open={isOpen}
        clickOutsideAction={() => setIsOpen(!isOpen)}
        isDefaultMode={false}
        // maxHeight={200}
        manualWidth="100%"
        style={{
          padding: "12px 16px",
          boxSizing: "border-box",
          marginTop: "4px",
        }}
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
          .filter((val) => val.name.toLowerCase().includes(searchValue))
          .map((country) => (
            <DropDownItem
              // noHover
              onClick={() => {
                setPhoneValue(country.dialCode), setIsOpen(!isOpen);
              }}
              key={country.code}
              style={{
                padding: "9px 0",
                boxSizing: "border-box",
                display: "flex",
                alignItems: "center",
              }}
            >
              <CountryFlag>Flag</CountryFlag>
              <CountryTitle>{country.name}</CountryTitle>
              <CountryDialCode>{country.dialCode}</CountryDialCode>
            </DropDownItem>
          ))}
      </DropDown>
    </Box>
  );
};
