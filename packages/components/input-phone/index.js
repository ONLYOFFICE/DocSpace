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
  locale: options[182].code, // default locale RU
  dialCode: options[182].dialCode, // default dialCode +7
  mask: options[182].mask, // default dialCode +7
  icon: options[182].flag, // default flag Russia
};

export const InputPhone = memo(() => {
  const [country, setCountry] = useState(defaultCountry);
  const [searchValue, setSearchValue] = useState("");
  const [filteredOptions, setFilteredOptions] = useState([]);
  const [isOpen, setIsOpen] = useState(false);
  const [isValid, setIsValid] = useState(true);

  const handleChange = (e) => {
    const str = e.target.value.replace(/\s/g, "");
    const el = options.find((option) => option.dialCode === str);

    console.log("el", el);

    // if (e.target.value === "") {
    //   setCountry((prev) => ({ ...prev, dialCode: "+" }));
    // } else {
    //   setCountry((prev) => ({ ...prev, dialCode: e.target.value }));
    // }

    if (el) {
      setCountry(() => ({
        ...prev,
        dialCode: e.target.value,
        mask: el.mask,
        icon: el.flag,
      }));
      setIsValid(true);
    } else {
      setIsValid(false);
    }
  };
  console.log("country", country);

  const getMask = (locale) => {
    return options.find((option) => option.code === setCountry()).mask;
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
          tabIndex={1}
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
            tabIndex={2}
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
