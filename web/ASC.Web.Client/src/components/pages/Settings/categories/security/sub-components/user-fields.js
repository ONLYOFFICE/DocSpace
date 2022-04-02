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
  const { t, className } = props;
  const [inputs, setInputs] = useState([]);

  const onClickAdd = () => {
    setInputs([...inputs, { key: inputs.length.toString(), value: "" }]);
  };

  const onChangeInput = (e) => {
    const index = inputs.findIndex((el) => el.key === e.target.id);
    let newInputs = Array.from(inputs);
    newInputs[index].value = e.target.value;
    setInputs(newInputs);
  };

  const onDeleteInput = (key) => {
    const index = inputs.findIndex((el) => el.key === key);
    let newInputs = Array.from(inputs);
    newInputs.splice(index, 1);
    setInputs(newInputs);
  };

  return (
    <div className={className}>
      {inputs ? (
        inputs.map((input) => {
          return (
            <StyledInputWrapper key={input.key}>
              <TextInput
                id={input.key}
                value={input.value}
                onChange={onChangeInput}
              />
              <StyledTrashIcon
                size="medium"
                onClick={() => onDeleteInput(input.key)}
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
          {t("AddTrustedDomain")}
        </Link>
      </StyledAddWrapper>
    </div>
  );
};

export default UserFields;
