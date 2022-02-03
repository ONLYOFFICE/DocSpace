import React, { useState } from "react";
import PropTypes from "prop-types";

import IconButton from "../../icon-button";
import TextInput from "../../text-input";

import { DeleteIcon, WarningIcon } from "../svg";

import { StyledChip, StyledChipValue } from "../styled-inputwithchips.js";

const Chip = (props) => {
  const {
    value,
    isSelected,
    isValid,
    onDelete,
    currentChip,
    onDoubleClick,
    onSaveNewChip,
    onClick,
  } = props;

  const [newValue, setNewValue] = useState(value?.value);

  const onChange = (e) => {
    setNewValue(e.target.value);
  };

  const onClickHandler = (e) => {
    if (e.shiftKey) {
      document.getSelection().removeAllRanges();
    }

    onClick(value);
  };

  const onDoubleClickHandler = () => {
    onDoubleClick(value);
  };

  const onIconClick = () => {
    onDelete(value);
  };

  const onBlur = () => {
    onDoubleClick(null);
  };

  const onInputKeyDown = (e) => {
    const code = e.code;

    switch (code) {
      case "Enter":
      case "NumpadEnter": {
        onSaveNewChip(value, newValue);
        return;
      }
      case "Escape": {
        onSaveNewChip(value, newValue);
        onBlur();
        return;
      }
    }
  };

  if (value?.value === currentChip?.value) {
    return (
      <TextInput
        value={newValue}
        onBlur={onBlur}
        onChange={onChange}
        onKeyDown={onInputKeyDown}
        isAutoFocussed
        withBorder={false}
      />
    );
  }

  return (
    <StyledChip
      isSelected={isSelected}
      onDoubleClick={onDoubleClickHandler}
      onClick={onClickHandler}
    >
      {!isValid && (
        <IconButton
          iconName={WarningIcon}
          size={12}
          onClick={onIconClick}
          style={{ marginRight: "4px" }}
        />
      )}
      <StyledChipValue>{value?.label}</StyledChipValue>
      <IconButton iconName={DeleteIcon} size={12} onClick={onIconClick} />
    </StyledChip>
  );
};

Chip.propTypes = {
  value: PropTypes.string,
  isSelected: PropTypes.array,
  isValid: PropTypes.bool,
  currentChip: PropTypes.string,
  onClick: PropTypes.func,
  onDoubleClick: PropTypes.func,
  onDelete: PropTypes.func,
  onSaveNewChip: PropTypes.func,
};

export default Chip;
