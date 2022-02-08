import React, { useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";

import IconButton from "../../icon-button";
import Tooltip from "../../tooltip";

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
    isBlured,
    onDelete,
    onDoubleClick,
    onSaveNewChip,
    onClick,
  } = props;

  const [newValue, setNewValue] = useState(value?.value);
  const [isOpenTooltip, setIsOpenTooltip] = useState(false);

  const tooltipRef = useRef(null);

  useEffect(() => {
    if (isBlured && isOpenTooltip && tooltipRef.current) {
      tooltipRef.current.hideTooltip();
    }
  }, [isBlured]);

  const onOpenTooltip = () => setIsOpenTooltip(true);
  const onCloseTooltip = () => setIsOpenTooltip(false);

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
    onSaveNewChip(value, newValue);
    onDoubleClick(null);
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
        break;
      }
    }
  };

  if (value?.value === currentChip?.value) {
    return (
      <StyledChipInput
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
      isValid={isValid}
    >
      {!isValid && (
        <div className="warning_icon_wrap">
          <IconButton
            iconName={WarningIcon}
            size={12}
            className="warning_icon_wrap warning_icon "
            data-for="group"
            data-tip={"Invalid email address"}
          />
          <Tooltip
            getContent={() => {}}
            id="group"
            afterShow={onOpenTooltip}
            afterHide={onCloseTooltip}
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
  isBlured: PropTypes.bool,
  onClick: PropTypes.func,
  onDoubleClick: PropTypes.func,
  onDelete: PropTypes.func,
  onSaveNewChip: PropTypes.func,
};

export default Chip;
