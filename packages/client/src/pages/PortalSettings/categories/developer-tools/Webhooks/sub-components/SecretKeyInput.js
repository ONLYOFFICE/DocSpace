import React, { useState, useEffect, useRef } from "react";
import styled from "styled-components";

import InfoIcon from "PUBLIC_DIR/images/info.react.svg?url";
import Link from "@docspace/components/link";

import { Hint } from "../styled-components";

import { PasswordInput } from "@docspace/components";
import { inject, observer } from "mobx-react";

const Header = styled.h1`
  font-family: "Open Sans";
  font-weight: 600;
  font-size: 13px;
  line-height: 20px;

  margin-top: 20px;

  color: #333333;
  display: flex;
  align-items: center;

  img {
    margin-left: 4px;
  }
`;

const StyledInfoIcon = styled.img`
  :hover {
    cursor: pointer;
  }
`;

const InfoHint = styled(Hint)`
  position: absolute;
  z-index: 2;

  width: 320px;
`;

const ReadMore = styled.a`
  display: inline-block;
  margin-top: 10px;
  font-family: "Open Sans";
  font-weight: 600;
  font-size: 13px;
  line-height: 15px;
  color: #333333;
`;

const SecretKeyInput = (props) => {
  const {
    isResetVisible,
    name,
    value,
    onChange,
    passwordSettings,
    isPasswordValid,
    setIsPasswordValid,
    setIsResetVisible,
  } = props;

  const [isHintVisible, setIsHintVisible] = useState(false);

  const secretKeyInputRef = useRef(null);

  const toggleHint = () => setIsHintVisible((prevIsHintVisible) => !prevIsHintVisible);

  const handleInputValidation = (isValid) => {
    setIsPasswordValid(isValid);
  };

  const generatePassword = () => {
    secretKeyInputRef.current.onGeneratePassword();
  };

  const handleHintDisapear = () => {
    toggleHint();
  };

  const handleOnChange = (e) => {
    onChange({ target: { name, value: e.target.value } });
  };

  const hideReset = () => {
    generatePassword();
    setIsResetVisible(false);
  };

  useEffect(() => {
    if (!isResetVisible) {
      onChange({ target: { name, value: secretKeyInputRef.current.state.inputValue } });
    }
  }, [isResetVisible]);

  return (
    <div>
      <Header>
        Secret key <StyledInfoIcon src={InfoIcon} alt="infoIcon" onClick={toggleHint} />
      </Header>

      <InfoHint hidden={!isHintVisible} onClick={handleHintDisapear}>
        Setting a webhook secret allows you to verify requests sent to the payload URL. <br />
        <ReadMore href="">Read more</ReadMore>
      </InfoHint>
      {isResetVisible && (
        <InfoHint>
          You cannot retrieve your secret key again once it has been saved. If you've lost or
          forgotten this secret key, you can reset it, but all integrations using this secret will
          need to be updated. <br />
          <Link
            type="action"
            fontWeight={600}
            isHovered={true}
            onClick={hideReset}
            style={{ marginTop: "6px", display: "inline-block" }}>
            Reset key
          </Link>
        </InfoHint>
      )}
      <div hidden={isResetVisible}>
        <PasswordInput
          onChange={handleOnChange}
          value={value}
          inputName={name}
          placeholder="Enter secret key"
          onValidateInput={handleInputValidation}
          ref={secretKeyInputRef}
          hasError={!isPasswordValid}
          isDisableTooltip={true}
          inputType="password"
          isFullWidth={true}
          passwordSettings={passwordSettings}
        />
        <Link
          type="action"
          fontWeight={600}
          isHovered={true}
          onClick={generatePassword}
          style={{ marginTop: "6px", display: "inline-block" }}>
          Generate
        </Link>
      </div>
    </div>
  );
};

export default inject(({ settingsStore }) => {
  const { passwordSettings } = settingsStore;

  return {
    passwordSettings,
  };
})(observer(SecretKeyInput));
