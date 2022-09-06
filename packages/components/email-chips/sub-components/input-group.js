import React, { memo, useState } from "react";
import PropTypes from "prop-types";

import Link from "../../link";
import TextInput from "../../text-input";

import { StyledInputWithLink, StyledTooltip } from "../styled-emailchips";
import { EmailSettings, parseAddresses } from "../../utils/email";

const InputGroup = memo(
  ({
    placeholder,
    exceededLimitText,
    existEmailText,
    exceededLimitInputText,
    clearButtonLabel,

    inputRef,
    containerRef,

    maxLength,

    isExistedOn,
    isExceededLimitChips,
    isExceededLimitInput,

    goFromInputToChips,
    onClearClick,
    onHideAllTooltips,
    showTooltipOfLimit,
    onAddChip,
  }) => {
    const [value, setValue] = useState("");

    const onInputChange = (e) => {
      setValue(e.target.value);
      onHideAllTooltips();
      if (e.target.value.length >= maxLength) showTooltipOfLimit();
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
          goFromInputToChips();
          if (inputRef) {
            onHideAllTooltips();
            inputRef.current.blur();
            containerRef.current.setAttribute("tabindex", "0");
            containerRef.current.focus();
          }
        }
      }
    };

    const onEnterPress = () => {
      if (isExceededLimitChips) return;
      if (isExistedOn) return;
      if (value.trim().length == 0) return;
      const settings = new EmailSettings();
      settings.allowName = true;
      const chipsFromString = parseAddresses(value, settings).map((it) => ({
        name: it.name === "" ? it.email : it.name,
        email: it.email,
        isValid: it.isValid(),
        parseErrors: it.parseErrors,
      }));
      onAddChip(chipsFromString);
      setValue("");
    };

    return (
      <StyledInputWithLink>
        {isExistedOn && <StyledTooltip>{existEmailText}</StyledTooltip>}
        {isExceededLimitChips && (
          <StyledTooltip>{exceededLimitText}</StyledTooltip>
        )}
        {isExceededLimitInput && (
          <StyledTooltip>{exceededLimitInputText}</StyledTooltip>
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
  inputRef: PropTypes.shape({ current: PropTypes.any }),
  containerRef: PropTypes.shape({ current: PropTypes.any }),

  placeholder: PropTypes.string,
  exceededLimitText: PropTypes.string,
  existEmailText: PropTypes.string,
  exceededLimitInputText: PropTypes.string,
  clearButtonLabel: PropTypes.string,

  maxLength: PropTypes.number,

  goFromInputToChips: PropTypes.func,
  onClearClick: PropTypes.func,
  isExistedOn: PropTypes.bool,
  isExceededLimitChips: PropTypes.bool,
  isExceededLimitInput: PropTypes.bool,
  onHideAllTooltips: PropTypes.func,
  showTooltipOfLimit: PropTypes.func,
  onAddChip: PropTypes.func,
};

InputGroup.displayName = "InputGroup";

export default InputGroup;
