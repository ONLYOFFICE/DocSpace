import { useState, useEffect, memo } from "react";
import { options } from "./options";
import { FixedSizeList as List } from "react-window";
import { StyledBox } from "./styled-input-phone";
import { InvalidFlag } from "./svg";
import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import Box from "@docspace/components/box";
import ComboBox from "@docspace/components/combobox";
import TextInput from "@docspace/components/text-input";
import InputBlock from "@docspace/components/input-block";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";
import Text from "@docspace/components/text";

export const InputPhone = memo((props) => {
  const [country, setCountry] = useState(props.defaultCountry);
  const [searchValue, setSearchValue] = useState("");
  const [filteredOptions, setFilteredOptions] = useState([]);
  const [isOpen, setIsOpen] = useState(false);
  const [isValid, setIsValid] = useState(true);

  const handleChange = (e) => {
    const str = e.target.value.replace(/\s/g, "");
    const el = options.find((option) => option.dialCode === str);

    if (e.target.value === "") {
      setCountry((prev) => ({ ...prev, dialCode: "+", icon: InvalidFlag }));
      setIsValid(false);
    }

    if (el) {
      setIsValid(true);
      setCountry({
        locale: el.code,
        dialCode: el.dialCode,
        mask: el.mask,
        icon: el.flag,
      });
    }
  };

  const handleSearch = (e) => {
    setSearchValue(e.target.value);
  };

  const getMask = (locale) => {
    return options.find((option) => option.code === locale).mask;
  };

  const handleClick = () => {
    setIsOpen(!isOpen), setIsValid(true);
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

    const countrySelection = () => {
      setIsOpen(!isOpen);
      setCountry({
        locale: country.code,
        dialCode: country.dialCode,
        mask: country.mask,
        icon: country.flag,
      });
    };

    return (
      <DropDownItem
        key={country.code}
        style={style}
        icon={country.flag}
        fillIcon={false}
        className="country-item"
        data-option={country.code}
        onClick={countrySelection}
      >
        <Text className="country-name">{country.name}</Text>
        <Text className="country-dialcode">{country.dialCode}</Text>
      </DropDownItem>
    );
  };

  return (
    <StyledBox hasError={!isValid} displayProp="flex" alignItems="center">
      <ComboBox
        onClick={handleClick}
        options={[]}
        scaled={true}
        noBorder={true}
        className="country-box"
        selectedOption={country}
      />

      <TextInput
        type="tel"
        className="input-phone"
        placeholder={props.phonePlaceholderText}
        mask={getMask(country.locale)}
        withBorder={false}
        tabIndex={1}
        value={country.dialCode}
        onChange={handleChange}
      />

      <DropDown
        open={isOpen}
        clickOutsideAction={handleClick}
        isDefaultMode={false}
        className="drop-down"
        manualWidth="100%"
      >
        <InputBlock
          type="text"
          iconName="static/images/search.react.svg"
          placeholder={props.searchPlaceholderText}
          value={searchValue}
          className="search-input"
          tabIndex={2}
          scale={true}
          onChange={handleSearch}
        />
        <Box marginProp="6px 0 0">
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
            <Text textAlign="center" className="empty-message" fontSize="14px">
              {props.searchEmptyMessage}
            </Text>
          )}
        </Box>
      </DropDown>
      {!isValid && (
        <Text
          className="error-text"
          color="#f21c0e"
          fontSize="11px"
          lineHeight="14px"
        >
          {props.errorMessage}
        </Text>
      )}
    </StyledBox>
  );
});

InputPhone.propTypes = {
  /** Default selected country Russia */
  defaultCountry: PropTypes.object.isRequired,
  /** Text displayed on the Input placeholder */
  phonePlaceholderText: PropTypes.string,
  /** Text displayed on the SearchInput placeholder */
  searchPlaceholderText: PropTypes.string,
  /** Called when field is clicked */
  onClick: PropTypes.func,
  /** Called when value is changed */
  onChange: PropTypes.func,
  /** Gets the country mask  */
  getMask: PropTypes.func,
  /** Text is displayed when nothing found */
  searchEmptyMessage: PropTypes.string,
  /** Text is displayed when invalid country dial code */
  errorMessage: PropTypes.string,
};

InputPhone.defaultProps = {
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

InputPhone.displayName = "InputPhone";
