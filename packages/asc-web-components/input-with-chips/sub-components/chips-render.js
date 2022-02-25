import React, { memo } from "react";
import PropTypes from "prop-types";

import { EmailSettings, parseAddress } from "../../utils/email";
import Chip from "./chip";

import { StyledAllChips } from "../styled-inputwithchips";

const ChipsRender = memo(
  ({
    chips,
    currentChip,
    blockRef,
    checkSelected,
    invalidEmailText,
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
              key={it?.value}
              value={it}
              currentChip={currentChip}
              isSelected={checkIsSelected(it)}
              isValid={checkEmail(it?.value)}
              invalidEmailText={invalidEmailText}
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

  blockRef: PropTypes.ref,

  checkSelected: PropTypes.func,
  onDelete: PropTypes.func,
  onDoubleClick: PropTypes.func,
  onSaveNewChip: PropTypes.func,
  onClick: PropTypes.func,
};

ChipsRender.displayName = "ChipsRender";

export default ChipsRender;
