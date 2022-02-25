import React, { memo, useState } from "react";
import PropTypes from "prop-types";

import Link from "../../link";
import TextInput from "../../text-input";
import { tryParseEmail } from "./helpers";

import { StyledInputWithLink, StyledTooltip } from "../styled-inputwithchips";

const InputGroup = memo(
  ({
    chips,
    setChips,
    clearButtonLabel,
    inputRef,
    setSelectedChips,
    containerRef,

    placeholder,
    exceededLimit,
    exceededLimitText,
    existEmailText,
    maxLength,
    ...props
  }) => {
    const [value, setValue] = useState("");
    /** is existing tooltip active */
    const [isExistedOn, setIsExistedOn] = useState(false);
    /** is exceeded tooltip active */
    const [isExceededLimit, setIsExceededLimit] = useState(
      chips.length >= exceededLimit
    );

    const onInputChange = (e) => {
      if (e.target.value == "") setIsExceededLimit(false);
      setValue(e.target.value);
      setIsExistedOn(false);
    };

    const onInputKeyDown = (e) => {
      const code = e.code;

      switch (code) {
        case "Enter":
        case "NumpadEnter": {
          onEnterPress();
          break;
        }
        case "ArrowLeft": {
          const isCursorStart = inputRef.current.selectionStart === 0;
          if (!isCursorStart) return;
          setSelectedChips([chips[chips?.length - 1]]);
          if (inputRef) {
            inputRef.current.blur();
            containerRef.current.setAttribute("tabindex", "0");
            containerRef.current.focus();
          }
        }
      }
    };

    const onEnterPress = () => {
      setIsExceededLimit(chips.length >= exceededLimit);
      if (isExceededLimit || chips.length >= exceededLimit) return;
      if (value.trim().length > 0) {
        const separators = [",", " ", ", "];
        let indexesForFilter = [];

        const chipsFromString = value
          .split(new RegExp(separators.join("|"), "g"))
          .filter((it) => it.trim().length !== 0)
          .map((it, idx, arr) => {
            if (it.includes('"') && arr[idx + 1]) {
              indexesForFilter.push(idx + 1);
              return `${it} ${arr[idx + 1]}`;
            }
            return it;
          })
          .map((it) => (tryParseEmail(it) ? tryParseEmail(it) : it.trim()))
          .filter((it, idx) => !indexesForFilter.includes(idx));

        if (chipsFromString.length === 1) {
          let isExisted = !!chips.find(
            (chip) =>
              chip.value === chipsFromString[0] ||
              chip.value === chipsFromString[0]?.value
          );
          setIsExistedOn(isExisted);
          if (isExisted) return;
        }

        const filteredChips = chipsFromString
          .filter((it) => {
            return !chips.find(
              (chip) => chip.value === it || chip.value === it?.value
            );
          })
          .map((it) => ({
            label: it?.label ?? it,
            value: it?.value ?? it,
          }));
        setChips([...chips, ...filteredChips]);
        setValue("");
      }
    };

    const onClearClick = () => {
      setChips([]);
    };

    return (
      <StyledInputWithLink>
        {isExistedOn && <StyledTooltip>{existEmailText}</StyledTooltip>}

        {isExceededLimit && chips.length >= exceededLimit && (
          <StyledTooltip>{exceededLimitText}</StyledTooltip>
        )}

        <TextInput
          value={value}
          onChange={onInputChange}
          forwardedRef={inputRef}
          onKeyDown={onInputKeyDown}
          placeholder={placeholder}
          withBorder={false}
          className="textInput"
          maxLength={maxLength}
        />
        <Link
          type="action"
          isHovered={true}
          className="link"
          onClick={onClearClick}
        >
          {clearButtonLabel}
        </Link>
      </StyledInputWithLink>
    );
  }
);

InputGroup.propTypes = {
  chips: PropTypes.arrayOf(PropTypes.object),

  clearButtonLabel: PropTypes.string,
  placeholder: PropTypes.string,
  exceededLimitText: PropTypes.string,
  existEmailText: PropTypes.string,

  maxLength: PropTypes.number,
  exceededLimit: PropTypes.number,

  inputRef: PropTypes.ref,
  containerRef: PropTypes.ref,

  setChips: PropTypes.func,
  setSelectedChips: PropTypes.func,
};

InputGroup.displayName = "InputGroup";

export default InputGroup;
