import React from 'react'
import styled from 'styled-components';
import { utils, FieldContainer, RadioButtonGroup, InputBlock, Icons, Link } from 'asc-web-components'

const PasswordBlock = styled.div`
  display: flex;
  align-items: center;
  line-height: 32px;
  flex-direction: row;

  .refresh-btn, .copy-link {
    margin: 0 0 0 16px;
  }

  @media ${utils.device.tablet} {
    flex-direction: column;
    align-items: start;

    .copy-link {
      margin: 0;
    }
  }
`;

const InputContainer = styled.div`
  width: 352px;
  display: flex;
  align-items: center;
`;

const PasswordField = React.memo((props) => {
  const {
    isRequired,
    hasError,
    labelText,

    radioName,
    radioValue,
    radioOptions,
    radioIsDisabled,
    radioOnChange,

    inputName,
    inputValue,
    inputIsDisabled,
    inputOnChange,
    inputTabIndex,

    inputIconOnClick,
    inputShowPassword,

    refreshIconOnClick,

    copyLinkText,
    copyLinkOnClick
  } = props;
  
  return (
    <FieldContainer
      isRequired={isRequired}
      hasError={hasError}
      labelText={labelText}
    >
      <RadioButtonGroup
        name={radioName}
        selected={radioValue}
        options={radioOptions}
        isDisabled={radioIsDisabled}
        onClick={radioOnChange}
        className="radio-group"
      />
      <PasswordBlock>
        <InputContainer>
          <InputBlock 
            name={inputName}
            hasError={hasError}
            isDisabled={inputIsDisabled}
            iconName="EyeIcon"
            value={inputValue}
            onIconClick={inputIconOnClick}
            onChange={inputOnChange}
            scale={true}
            type={inputShowPassword ? "text" : "password"}
            tabIndex={inputTabIndex}
          />
          <Icons.RefreshIcon
            size="medium"
            onClick={refreshIconOnClick}
            className="refresh-btn"
          />
        </InputContainer>
        <Link
          type="action"
          isHovered={true}
          onClick={copyLinkOnClick}
          className="copy-link"
        >
          {copyLinkText}
        </Link>
      </PasswordBlock>
    </FieldContainer>
  );
});

export default PasswordField;