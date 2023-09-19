import React, { useMemo } from "react";
import { useTranslation } from "react-i18next";
import { DateTime } from "luxon";

import ComboBox from "../../combobox";
import { Option } from "../types";
import { fixFormatValue } from "../util";
import { SelectWrapper } from "./Select.styled";
import SelectProps from "./Select.props";

function Select({
  unit,
  value,
  placeholder,
  setValue,
  prefix,
  dropDownMaxHeight,
}: SelectProps) {
  const { i18n } = useTranslation();

  const options = useMemo(() => {
    const { altWithTranslation } = unit;

    if (altWithTranslation) {
      return altWithTranslation.map((item, index) => {
        const number = unit.min === 0 ? index : index + 1;

        return {
          key: number,
          label: item,
        };
      });
    }

    // const local = new Intl.Locale(i18n.language);
    // // DateTime.

    return [...Array(unit.total)].map((_, index) => {
      const number = unit.min === 0 ? index : index + 1;

      return {
        key: number,
        label: fixFormatValue(number, i18n.language),
      };
    });
  }, [i18n.language]);

  const selectedOption = useMemo(() => {
    const isEmpty = value.length === 0;

    return {
      key: isEmpty ? -1 : value[0],
      label: isEmpty
        ? placeholder
        : unit.altWithTranslation
        ? unit.altWithTranslation[value[0] - unit.min]
        : fixFormatValue(value[0], i18n.language),
    };
  }, [value, placeholder, i18n.language]);

  const onSelect = (option: Option<number, string>) => {
    setValue([option.key]);
  };

  const onReset = (option: Option<number, string>) => {
    if (option.key === value[0]) {
      setValue([]);
    }
  };

  return (
    <SelectWrapper>
      <span>{prefix}</span>
      <ComboBox
        scaledOptions
        size="content"
        scaled={false}
        noBorder={false}
        showDisabledItems
        options={options}
        onSelect={onSelect}
        onClickSelectedItem={onReset}
        selectedOption={selectedOption}
        dropDownMaxHeight={dropDownMaxHeight}
      />
    </SelectWrapper>
  );
}

export default Select;
