import React, { useEffect, useState, useRef, useCallback, memo } from "react";
import { FixedSizeList as List } from "react-window";
import Box from "../box";
import TextInput from "../text-input";
import Text from "../text";
import * as Countries from "./svg";
import {
  StyledTriangle,
  StyledDropDown,
  StyledCountryItem,
  StyledFlagBox,
  StyledSearchPanel,
  StyledDropDownWrapper,
} from "./styled-phone-input";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";

const Dropdown = memo(
  ({
    value,
    options,
    onChange,
    theme,
    searchPlaceholderText,
    searchEmptyMessage,
  }) => {
    const dropDownMenu = useRef();

    const [open, setOpen] = useState(false);

    const [filteredCountries, setFilteredCountries] = useState(options);
    const [search, setSearch] = useState("");

    const handleClick = (e) => {
      if (dropDownMenu.current.contains(e.target)) {
        return;
      }
      setOpen(false);
      setSearch("");
    };

    const handleChange = (selectedValue) => {
      onChange(selectedValue);
      setOpen(false);
      setSearch("");
    };

    const openDropDown = useCallback(() => setOpen(!open), [open]);

    const listRef = React.createRef();

    useEffect(() => {
      if (open) {
        setFilteredCountries(
          options.filter((option) =>
            option.name.toLowerCase().includes(search.toLowerCase())
          )
        );
      }
      document.addEventListener("mousedown", handleClick);
      return () => {
        document.removeEventListener("mousedown", handleClick);
      };
    }, [open, search]);

    const onSearchCountry = useCallback((e) => {
      const textSearch = e.target.value;
      setSearch(textSearch);
    }, []);

    const onHandleChange = useCallback((e) => {
      handleChange(e.currentTarget.dataset.option);
    }, []);

    const setCountry = options.find((o) => o.code === value).code;

    const CountryItem = ({ data, index, style }) => {
      const option = data[index];
      const text = `${option.name} ${option.dialCode}`;

      return (
        <div style={style}>
          <StyledCountryItem key={option.code}>
            <Box
              displayProp="flex"
              backgroundProp={option.code === value ? "#e9e9e9" : ""}
              data-option={option.code}
              onClick={onHandleChange}
            >
              <Box marginProp={"5px 0 3px 10px"}>
                {React.createElement(Countries[`${option.code}`], {
                  width: 24,
                  height: 16,
                })}
              </Box>
              <Box widthProp="250px" marginProp="2px 0 2px 8px">
                <Text
                  color={theme.phoneInput.itemTextColor}
                  truncate={true}
                  title={text}
                >
                  {text}
                </Text>
              </Box>
            </Box>
          </StyledCountryItem>
        </div>
      );
    };

    return (
      <StyledDropDownWrapper>
        <Box ref={dropDownMenu} displayProp="flex">
          <StyledFlagBox onClick={openDropDown}>
            {value
              ? React.createElement(Countries[`${setCountry}`], {
                  width: 24,
                  height: 16,
                })
              : "n/a"}
          </StyledFlagBox>
          <StyledTriangle onClick={openDropDown} />
          {open && (
            <StyledDropDown>
              <StyledSearchPanel>
                <TextInput
                  value={search}
                  placeholder={searchPlaceholderText}
                  onChange={onSearchCountry}
                  scale={true}
                  className="phone-input-searcher"
                />
              </StyledSearchPanel>
              <div style={{ height: "220px" }}>
                {filteredCountries.length ? (
                  <List
                    itemData={filteredCountries}
                    height={220}
                    itemCount={filteredCountries.length}
                    itemSize={28}
                    width={304}
                    outerElementType={CustomScrollbarsVirtualList}
                    ref={listRef}
                  >
                    {CountryItem}
                  </List>
                ) : (
                  <Box paddingProp="8px">
                    <Text>{searchEmptyMessage}</Text>
                  </Box>
                )}
              </div>
            </StyledDropDown>
          )}
        </Box>
      </StyledDropDownWrapper>
    );
  }
);

Dropdown.displayName = "Dropdown";

export default Dropdown;
