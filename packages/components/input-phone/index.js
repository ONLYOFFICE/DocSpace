import { useState, useEffect, memo } from "react";
import { options } from "./options";
import { FixedSizeList as List } from "react-window";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import Box from "@docspace/components/box";
import InputBlock from "@docspace/components/input-block";

import {
  StyledBox,
  StyledComboBox,
  StyledInput,
  StyledDropDown,
  StyledDropDownItem,
  StyledText,
  CountryName,
  CountryDialCode,
  ErrorText,
} from "./styled-input-phone";

const defaultCountry = {
  locale: options[16].code, // RU default Russia
  dialCode: options[16].dialCode, // +7 default Russia
  icon: options[16].flag, // flag default Russia
};

export const InputPhone = memo(() => {
  const [country, setCountry] = useState(defaultCountry);
  const [searchValue, setSearchValue] = useState("");
  const [filteredOptions, setFilteredOptions] = useState([]);
  const [isOpen, setIsOpen] = useState(false);
  const [isValid, setIsValid] = useState(true);

  const handleChange = (e) => {
    if (e.target.value === "") {
      setCountry((prev) => ({ ...prev, dialCode: "+" }));
    } else {
      setCountry((prev) => ({ ...prev, dialCode: e.target.value }));
    }
  };

  const getMask = (locale) => {
    return options.find((option) => option.code === locale).mask;
  };

  useEffect(() => {
    if (isOpen) {
      setFilteredOptions(
        options.filter(
          (val) =>
            val.name.toLowerCase().includes(searchValue) ||
            val.dialCode.includes(searchValue)
        )
      );
    }
  }, [isOpen, searchValue]);

  const Row = ({ data, index, style }) => {
    const country = data[index];

    return (
      <StyledDropDownItem
        key={country.code}
        style={{ ...style }}
        data-option={country.code}
        icon={country.flag}
        fillIcon={false}
        onClick={() => {
          setIsOpen(!isOpen);
          setCountry({
            locale: country.code,
            dialCode: country.dialCode,
            icon: country.flag,
          });
        }}
      >
        <CountryName>{country.name}</CountryName>
        <CountryDialCode>{country.dialCode}</CountryDialCode>
      </StyledDropDownItem>
    );
  };

  return (
    <>
      <StyledBox>
        <Box>
          <StyledComboBox
            onClick={() => setIsOpen(!isOpen)}
            options={[]}
            scaled={true}
            noBorder={true}
            hasError={!isValid}
            selectedOption={country}
          />
        </Box>

        <StyledInput
          type="tel"
          hasError={!isValid}
          placeholder="+7 XXX XXX-XX-XX"
          mask={getMask(country.locale)}
          isAutoFocussed={true}
          withBorder={false}
          value={country.dialCode}
          onChange={handleChange}
        />

        <StyledDropDown
          open={isOpen}
          clickOutsideAction={() => setIsOpen(!isOpen)}
          isDefaultMode={false}
          manualWidth="100%"
        >
          <InputBlock
            type="text"
            iconName="static/images/search.react.svg"
            placeholder="Search"
            value={searchValue}
            scale={true}
            onChange={(e) => setSearchValue(e.target.value)}
            style={{ marginBottom: "6px" }}
          />
          <Box>
            {filteredOptions.length ? (
              <List
                itemData={filteredOptions}
                height={108}
                itemCount={filteredOptions.length}
                itemSize={36}
                outerElementType={CustomScrollbarsVirtualList}
                width="auto"
              >
                {Row}
              </List>
            ) : (
              <StyledText>Country Not Found</StyledText>
            )}
          </Box>
        </StyledDropDown>
      </StyledBox>
      {!isValid && <ErrorText>Сountry code is invalid</ErrorText>}
    </>
  );
});
