import React from "react";
import styled from "styled-components";

import Avatar from "../../../avatar";
import Text from "../../../text";
import Checkbox from "../../../checkbox";
import { Base } from "../../../themes";

const StyledSelectAll = styled.div`
  width: 100%;
  max-height: 61px;
  height: 61px;
  min-height: 61px;

  display: flex;
  align-items: center;

  border-bottom: ${(props) => props.theme.selector.border};

  box-sizing: border-box;

  padding: 8px 16px 20px;

  margin-bottom: 12px;

  .select-all_avatar {
    min-width: 32px;
  }

  .label {
    width: 100%;
    max-width: 100%;

    line-height: 16px;

    margin-left: 8px;
  }

  .checkbox {
    svg {
      margin-right: 0px;
    }
  }
`;

StyledSelectAll.defaultProps = { theme: Base };

const SelectAll = React.memo(
  ({
    label,
    icon,
    onSelectAll,
    isChecked,
    isIndeterminate,
    isLoading,
    rowLoader,
  }) => {
    return (
      <StyledSelectAll>
        <div style={{ background: "red", height: "150px" }}></div>
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
