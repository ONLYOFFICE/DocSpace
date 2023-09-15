import React, { useCallback, useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";

import IconButton from "../../icon-button";
import Tooltip from "../../tooltip";
import { useClickOutside } from "../../utils/useClickOutside.js";

import { DeleteIconSvgUrl, WarningIconSvgUrl } from "../svg";
import {
  MAX_EMAIL_LENGTH,
  MAX_EMAIL_LENGTH_WITH_DOTS,
  sliceEmail,
} from "./helpers";

import {
  StyledChip,
  StyledChipInput,
  StyledChipValue,
  StyledContainer,
} from "../styled-emailchips.js";

const Chip = (props) => {
  const {
    value,
    currentChip,
    isSelected,
    isValid,
    invalidEmailText,
    chipOverLimitText,
    onDelete,
    onDoubleClick,
    onSaveNewChip,
    onClick,
  } = props;

  function initNewValue() {
    return value?.email === value?.name || value?.name === ""
      ? value?.email
      : `"${value?.name}" <${value?.email}>`;
  }

  const [newValue, setNewValue] = useState(initNewValue());
  const [chipWidth, setChipWidth] = useState(0);
  const [isChipOverLimit, setIsChipOverLimit] = useState(false);

  const warningRef = useRef(null);
  const chipRef = useRef(null);
  const chipInputRef = useRef(null);

  useEffect(() => {
    setChipWidth(chipRef.current?.clientWidth);
  }, [chipRef]);

  useEffect(() => {
    if (isSelected) {
      chipRef.current?.scrollIntoView({ block: "end" });
    }
  }, [isSelected]);

  useEffect(() => {
    if (newValue.length > MAX_EMAIL_LENGTH) {
      setIsChipOverLimit(true);
    } else {
      setIsChipOverLimit(false);
    }
  }, [newValue]);

  useClickOutside(
    chipInputRef,
    () => {
      onSaveNewChip(value, newValue);
    },
    newValue
  );

  const onChange = (e) => {
    if (
      e.target.value.length <= MAX_EMAIL_LENGTH_WITH_DOTS ||
      e.target.value.length < newValue.length
    ) {
      setNewValue(e.target.value);
    }
  };

  const onClickHandler = (e) => {
    if (e.shiftKey) {
      document.getSelection().removeAllRanges();
    }
    onClick(value, e.shiftKey);
  };

  const onDoubleClickHandler = () => {
    onDoubleClick(value);
  };

  const onIconClick = () => {
    onDelete(value);
  };

  const onInputKeyDown = useCallback(
    (e) => {
      const code = e.code;

      switch (code) {
        case "Enter":
        case "NumpadEnter": {
          onSaveNewChip(value, newValue);
          setNewValue(sliceEmail(newValue).email);
          break;
        }
        case "Escape": {
          setNewValue(initNewValue());
          onDoubleClick(null);
          return false;
        }
      }
    },
    [newValue]
  );
  if (value?.email === currentChip?.email) {
    return (
      <StyledContainer>
        {isChipOverLimit && <Tooltip id="input" float />}
        <StyledChipInput
          data-tooltip-id="input"
          data-tooltip-content={chipOverLimitText}
          value={newValue}
          forwardedRef={chipInputRef}
          onChange={onChange}
          onKeyDown={onInputKeyDown}
          isAutoFocussed
          withBorder={false}
          maxLength={MAX_EMAIL_LENGTH_WITH_DOTS}
          flexvalue={
            value?.name !== value?.email ? "0 1 auto" : `0 0 ${chipWidth}px`
          }
        />
      </StyledContainer>
    );
  }

  return (
    <StyledChip
      isSelected={isSelected}
      onDoubleClick={onDoubleClickHandler}
      onClick={onClickHandler}
      isValid={isValid}
      ref={chipRef}
    >
      {!isValid && (
        <div className="warning_icon_wrap" ref={warningRef}>
          <IconButton
            iconName={WarningIconSvgUrl}
            size={12}
            className="warning_icon_wrap warning_icon"
            data-tooltip-id="group"
            data-tooltip-content={invalidEmailText}
          />
          <Tooltip id="group" place={"top"} />
        </div>
      )}
      {/*dir="auto" for correct truncate email view (asd@gmai..., ...خالد@الدوح)*/}
      <StyledChipValue dir="auto" isValid={isValid}>
        {value?.name || value?.email}
      </StyledChipValue>
      <IconButton iconName={DeleteIconSvgUrl} size={12} onClick={onIconClick} />
    </StyledChip>
  );
};

Chip.propTypes = {
  value: PropTypes.object,
  currentChip: PropTypes.object,
  isSelected: PropTypes.bool,
  isValid: PropTypes.bool,
  invalidEmailText: PropTypes.string,
  chipOverLimitText: PropTypes.string,
  onClick: PropTypes.func,
  onDoubleClick: PropTypes.func,
  onDelete: PropTypes.func,
  onSaveNewChip: PropTypes.func,
};

export default Chip;
