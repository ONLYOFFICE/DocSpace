import { useState, useEffect, memo } from "react";
import { options } from "./options";
import { FixedSizeList as List } from "react-window";
import { StyledBox } from "./styled-input-phone";
import { InvalidFlag } from "./svg";
import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import Box from "@docspace/components/box";
import ComboBox from "@docspace/components/combobox";
import Label from "@docspace/components/label";
import TextInput from "@docspace/components/text-input";
import SearchInput from "@docspace/components/search-input";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";
import Text from "@docspace/components/text";

const InputPhone = (props) => {
  const [country, setCountry] = useState(props.defaultCountry);
  const [phoneValue, setPhoneValue] = useState(country.dialCode);
  const [searchValue, setSearchValue] = useState("");
  const [filteredOptions, setFilteredOptions] = useState([]);
  const [isOpen, setIsOpen] = useState(false);
  const [isValid, setIsValid] = useState(true);

  const onInputChange = (e) => {
    const str = e.target.value.replace(/\s/g, "");
    const el = options.find((option) => option.dialCode === str);

    if (e.target.value === "" || phoneValue) {
      setIsValid(false);
      setCountry((prev) => ({ ...prev, icon: InvalidFlag }));
    }

    setPhoneValue(e.target.value);

    if (el) {
      setIsValid(true);
      setCountry({
        locale: el.code,
        mask: el.mask,
        icon: el.flag,
      });
    }
    props.onChange && props.onChange(e);
  };

  const onCountrySearch = (value) => {
    setSearchValue(value);
  };

  const onClearSearch = () => {
    setSearchValue("");
  };

  const getMask = (locale) => {
    return options.find((option) => option.code === locale).mask;
  };

  const handleClick = () => {
    setIsOpen(!isOpen);
  };

  useEffect(() => {
    if (isOpen) {
      setFilteredOptions(
        options.filter(
          (val) =>
            val.name.toLowerCase().includes(searchValue.toLowerCase()) ||
            val.dialCode.includes(searchValue.toLowerCase())
        )
      );
    }
  }, [isOpen, searchValue]);

  const onCountryClick = (e) => {
    const data = e.currentTarget.dataset.option;
    const country = filteredOptions[data];

    setIsOpen(!isOpen);
    setCountry({
      locale: country.code,
      mask: country.mask,
      icon: country.flag,
    });
    setIsValid(true);
    setPhoneValue(country.dialCode);
  };

  const Row = ({ data, index, style }) => {
    const country = data[index];

    return (
      <DropDownItem
        key={country.code}
        style={style}
        icon={country.flag}
        fillIcon={false}
        className="country-item"
        data-option={index}
        onClick={onCountryClick}
      >
        <Text className="country-name">{country.name}</Text>
        <Text className="country-dialcode">{`+${country.dialCode}`}</Text>
      </DropDownItem>
    );
  };

  return (
    <StyledBox
      hasError={!isValid}
      displayProp="flex"
      alignItems="center"
      scaled={props.scaled}
    >
      <ComboBox
        onClick={handleClick}
        options={[]}
        noBorder={true}
        className="country-box"
        selectedOption={country}
      />
      <Label text="+" className="prefix" />
      <TextInput
        type="tel"
        className="input-phone"
        placeholder={props.phonePlaceholderText}
        mask={getMask(country.locale)}
        withBorder={false}
        tabIndex={1}
        value={phoneValue}
        onChange={onInputChange}
      />
      <DropDown
        open={isOpen}
        clickOutsideAction={handleClick}
        isDefaultMode={false}
        className="drop-down"
        manualWidth="100%"
      >
        <SearchInput
          placeholder={props.searchPlaceholderText}
          value={searchValue}
          className="search-country_input"
          scale={true}
          onClearSearch={onClearSearch}
          refreshTimeout={100}
          onChange={onCountrySearch}
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
            <Text
              textAlign="center"
              className="phone-input_empty-text"
              fontSize="14px"
            >
              {props.searchEmptyMessage}
            </Text>
          )}
        </Box>
      </DropDown>
      {!isValid && (
        <Text
          className="phone-input_error-text"
          color="#f21c0e"
          fontSize="11px"
          lineHeight="14px"
        >
          {props.errorMessage}
        </Text>
      )}
    </StyledBox>
  );
};

InputPhone.propTypes = {
  /** Default selected country Russia */
  defaultCountry: PropTypes.object.isRequired,
  /** Text displayed on the Input placeholder */
  phonePlaceholderText: PropTypes.string,
  /** Text displayed on the SearchInput placeholder */
  searchPlaceholderText: PropTypes.string,
  /** Indicates the input field has scaled */
  scaled: PropTypes.bool,
  /** Called when value is changed */
  onChange: PropTypes.func,
  /** Gets the country mask  */
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
  phonePlaceholderText: "",
  searchPlaceholderText: "",
  scaled: false,
  searchEmptyMessage: "",
  errorMessage: "",
};

InputPhone.displayName = "InputPhone";

export default memo(InputPhone);
