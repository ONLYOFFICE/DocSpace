import React from "react";

import Avatar from "../../../avatar";
import Text from "../../../text";
import Checkbox from "../../../checkbox";

import StyledSelectAll from "./StyledSelectAll";
import { SelectAllProps } from "./SelectAll.types";

const SelectAll = React.memo(
  ({
    label,
    icon,
    onSelectAll,
    isChecked,
    isIndeterminate,
    isLoading,
    rowLoader,
  }: SelectAllProps) => {
    const onClick = (e: React.MouseEvent<HTMLDivElement>) => {
      if (e.target instanceof HTMLElement && e.target.closest(".checkbox"))
        return;

      onSelectAll && onSelectAll();
    };

    return (
      <StyledSelectAll onClick={onClick}>
        {isLoading ? (
          rowLoader
        ) : (
          <>
            <Avatar
              className="select-all_avatar"
              source={icon}
              role={"user"}
              size={"min"}
            />

            <Text
              className="label"
              fontWeight={600}
              fontSize={"14px"}
              noSelect
              truncate
            >
              {label}
            </Text>

            <Checkbox
              className="checkbox"
              isChecked={isChecked}
              isIndeterminate={isIndeterminate}
              onChange={onSelectAll}
            />
          </>
        )}
      </StyledSelectAll>
    );
  }
);

export default SelectAll;
