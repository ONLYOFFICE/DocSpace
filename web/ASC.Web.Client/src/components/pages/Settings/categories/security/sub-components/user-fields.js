import React, { useState } from "react";
import styled from "styled-components";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { PlusIcon, TrashIcon } from "./svg";
import Link from "@appserver/components/link";
import TextInput from "@appserver/components/text-input";

const StyledPlusIcon = styled(PlusIcon)`
  ${commonIconsStyles}
`;

const StyledTrashIcon = styled(TrashIcon)`
  ${commonIconsStyles}
  cursor: pointer;
`;

const StyledInputWrapper = styled.div`
  display: flex;
  flex-direction: row;
  gap: 10px;
  align-items: center;
  margin-bottom: 8px;
  width: 370px;
`;

const StyledAddWrapper = styled.div`
  display: flex;
  flex-direction: row;
  gap: 6px;
  align-items: center;
  cursor: pointer;
`;

const UserFields = (props) => {
  const {
    className,
    buttonLabel,
    onChangeInput,
    onDeleteInput,
    onClickAdd,
    inputs,
  } = props;

  return (
    <div className={className}>
      {inputs ? (
        inputs.map((input, index) => {
          return (
            <StyledInputWrapper key={`domain-input-${index}`}>
              <TextInput
                value={input}
                onChange={(e) => onChangeInput(e, index)}
              />
              <StyledTrashIcon
                size="medium"
                onClick={() => onDeleteInput(index)}
              />
            </StyledInputWrapper>
          );
        })
      ) : (
        <></>
      )}

      <StyledAddWrapper onClick={onClickAdd}>
        <StyledPlusIcon size="small" />
        <Link type="action" isHovered={true}>
          {buttonLabel}
        </Link>
      </StyledAddWrapper>
    </div>
  );
};

export default UserFields;
