import React, { memo } from "react";
import PropTypes from "prop-types";

import { EmailSettings, parseAddress } from "../../utils/email";
import Chip from "./chip";

import { StyledAllChips } from "../styled-emailchips";

const ChipsRender = memo(
  ({
    chips,
    currentChip,
    blockRef,
    checkSelected,
    invalidEmailText,
    chipOverLimitText,
    onDelete,
    onDoubleClick,
    onSaveNewChip,
    onClick,
    ...props
  }) => {
    const emailSettings = new EmailSettings();

    const checkEmail = (email) => {
      const emailObj = parseAddress(email, emailSettings);
      return emailObj.isValid();
    };

    const checkIsSelected = (value) => {
      return checkSelected(value);
    };

    return (
      <StyledAllChips ref={blockRef}>
        {chips?.map((it) => {
          return (
            <Chip
              key={it?.email}
              value={it}
              currentChip={currentChip}
              isSelected={checkIsSelected(it)}
              isValid={checkEmail(it?.email)}
              invalidEmailText={invalidEmailText}
              chipOverLimitText={chipOverLimitText}
              onDelete={onDelete}
              onDoubleClick={onDoubleClick}
              onSaveNewChip={onSaveNewChip}
              onClick={onClick}
            />
          );
        })}
      </StyledAllChips>
    );
  }
);

ChipsRender.propTypes = {
  chips: PropTypes.arrayOf(PropTypes.object),
  currentChip: PropTypes.object,

  invalidEmailText: PropTypes.string,
  chipOverLimitText: PropTypes.string,

  blockRef: PropTypes.shape({ current: PropTypes.any }),

  checkSelected: PropTypes.func,
  onDelete: PropTypes.func,
  onDoubleClick: PropTypes.func,
  onSaveNewChip: PropTypes.func,
  onClick: PropTypes.func,
};

ChipsRender.displayName = "ChipsRender";

export default ChipsRender;
