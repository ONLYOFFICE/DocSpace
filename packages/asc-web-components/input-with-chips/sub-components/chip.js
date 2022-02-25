import React, { useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";

import IconButton from "../../icon-button";
import Tooltip from "../../tooltip";
import { useClickOutside } from "../../utils/useClickOutside.js";

import { DeleteIcon, WarningIcon } from "../svg";

import {
  StyledChip,
  StyledChipInput,
  StyledChipValue,
} from "../styled-inputwithchips.js";

const Chip = (props) => {
  const {
    value,
    currentChip,
    isSelected,
    isValid,
    invalidEmailText,
    onDelete,
    onDoubleClick,
    onSaveNewChip,
    onClick,
  } = props;

  const [newValue, setNewValue] = useState(
    value?.value === value?.label
      ? value?.value
      : `"${value?.label}" <${value?.value}>`
  );
  const [chipWidth, setChipWidth] = useState(0);

  const tooltipRef = useRef(null);
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

  useClickOutside(warningRef, () => tooltipRef.current.hideTooltip());
  useClickOutside(
    chipInputRef,
    () => {
      onSaveNewChip(value, newValue);
    },
    newValue
  );

  const onChange = (e) => {
    setNewValue(e.target.value);
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

  const onInputKeyDown = (e) => {
    const code = e.code;

    switch (code) {
      case "Enter":
      case "NumpadEnter": {
        onSaveNewChip(value, newValue);
        break;
      }
      case "Escape": {
        setNewValue(value.value);
        onDoubleClick(null);
        return false;
      }
    }
  };
  if (value?.value === currentChip?.value) {
    return (
      <StyledChipInput
        value={newValue}
        forwardedRef={chipInputRef}
        onChange={onChange}
        onKeyDown={onInputKeyDown}
        isAutoFocussed
        withBorder={false}
        flexvalue={
          value?.label !== value?.value ? "0 1 auto" : `0 0 ${chipWidth}px`
        }
      />
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
            iconName={WarningIcon}
            size={12}
            className="warning_icon_wrap warning_icon "
            data-for="group"
            data-tip={invalidEmailText}
          />
          <Tooltip
            getContent={() => {}}
            id="group"
            reference={tooltipRef}
            place={"top"}
          />
        </div>
      )}
      <StyledChipValue>{value?.label}</StyledChipValue>
      <IconButton iconName={DeleteIcon} size={12} onClick={onIconClick} />
    </StyledChip>
  );
};

Chip.propTypes = {
  value: PropTypes.object,
  currentChip: PropTypes.object,
  isSelected: PropTypes.bool,
  isValid: PropTypes.bool,
  invalidEmailText: PropTypes.string,
  onClick: PropTypes.func,
  onDoubleClick: PropTypes.func,
  onDelete: PropTypes.func,
  onSaveNewChip: PropTypes.func,
};

export default Chip;
