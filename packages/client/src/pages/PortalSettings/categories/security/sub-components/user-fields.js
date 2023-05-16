import React, { useState, useEffect, useRef } from "react";
import styled from "styled-components";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import TrashIcon from "PUBLIC_DIR/images/trash.react.svg";
import PlusIcon from "PUBLIC_DIR/images/plus.react.svg";
import Link from "@docspace/components/link";
import TextInput from "@docspace/components/text-input";
import { Base } from "@docspace/components/themes";

const StyledPlusIcon = styled(PlusIcon)`
  ${commonIconsStyles}

  path {
    fill: ${(props) => props.theme.client.settings.iconFill};
  }
`;

StyledPlusIcon.defaultProps = { theme: Base };

const StyledTrashIcon = styled(TrashIcon)`
  ${commonIconsStyles}
  cursor: pointer;
  path {
    fill: ${(props) => props.theme.client.settings.trashIcon};
  }
`;

const StyledInputWrapper = styled.div`
  display: flex;
  flex-direction: row;
  gap: 10px;
  align-items: center;
  margin-bottom: 8px;
  width: 350px;

  @media (max-width: 375px) {
    width: 100%;
  }
`;

const StyledAddWrapper = styled.div`
  display: flex;
  flex-direction: row;
  gap: 6px;
  align-items: center;
  cursor: pointer;
  margin-top: ${(props) => (props.inputsLength > 0 ? "16px" : "0px")};
`;

const usePrevious = (value) => {
  const ref = useRef();
  useEffect(() => {
    ref.current = value;
  }, [value]);
  return ref.current;
};

const UserFields = (props) => {
  const {
    className,
    buttonLabel,
    onChangeInput,
    onDeleteInput,
    onClickAdd,
    inputs,
    regexp,
  } = props;

  const [errors, setErrors] = useState(new Array(inputs.length).fill(false));
  const prevInputs = usePrevious(inputs.length);

  useEffect(() => {
    if (inputs.length > prevInputs) setErrors([...errors, false]);
  }, [inputs]);

  const onBlur = (index) => {
    let newErrors = Array.from(errors);
    newErrors[index] = true;
    setErrors(newErrors);
  };

  const onFocus = (index) => {
    let newErrors = Array.from(errors);
    newErrors[index] = false;
    setErrors(newErrors);
  };

  const onDelete = (index) => {
    let newErrors = Array.from(errors);
    newErrors.splice(index, 1);
    setErrors(newErrors);

    onDeleteInput(index);
  };

  return (
    <div className={className}>
      {inputs ? (
        inputs.map((input, index) => {
          let newInput1;
          let newInput2;

          if (input?.includes("-")) {
            newInput1 = input.split("-")[0];
            newInput2 = input.split("-")[1];
          }

          const error = newInput2
            ? input?.split("-").length - 1 > 1 ||
              !regexp.test(newInput1) ||
              !regexp.test(newInput2)
            : !regexp.test(input);

          return (
            <StyledInputWrapper key={`user-input-${index}`}>
              <TextInput
                id={`user-input-${index}`}
                isAutoFocussed={false}
                value={input}
                onChange={(e) => onChangeInput(e, index)}
                onBlur={() => onBlur(index)}
                onFocus={() => onFocus(index)}
                hasError={errors[index] && error}
              />
              <StyledTrashIcon size="medium" onClick={() => onDelete(index)} />
            </StyledInputWrapper>
          );
        })
      ) : (
        <></>
      )}

      <StyledAddWrapper onClick={onClickAdd} inputsLength={inputs.length}>
        <StyledPlusIcon size="small" />
        <Link type="action" isHovered={true} fontWeight={600}>
          {buttonLabel}
        </Link>
      </StyledAddWrapper>
    </div>
  );
};

export default UserFields;
