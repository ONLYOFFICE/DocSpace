import React, { useState, useEffect } from "react";
import styled from "styled-components";

import InfoIcon from "PUBLIC_DIR/images/info.react.svg?url";
import Link from "@docspace/components/link";

import { Hint } from "../styled-components";

import { PasswordInput } from "@docspace/components";

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

export const SecretKeyInput = ({
  isResetVisible,
  name,
  value,
  onChange,
  passwordSettings,
  isPasswordValid,
  setIsPasswordValid,
  isFormBlank,
  setIsFormBlank,
  secretKeyInputRef,
  generatePassword,
}) => {
  const [isHintVisible, setIsHintVisible] = useState(false);

  const toggleHint = () => setIsHintVisible((prevIsHintVisible) => !prevIsHintVisible);

  const handleInputValidation = (isValid) => {
    setIsPasswordValid(isValid);
  };

  const handleHintDisapear = () => {
    toggleHint();
    generatePassword();
  };

  useEffect(() => {
    if (secretKeyInputRef.current && isFormBlank) {
      generatePassword();
      setIsFormBlank(false);
    }
  }, [isFormBlank]);

  return (
    <div>
      <Header>
        Secret key <StyledInfoIcon src={InfoIcon} alt="infoIcon" onClick={toggleHint} />
      </Header>

      <InfoHint hidden={!isHintVisible} onClick={handleHintDisapear}>
        Setting a webhook secret allows you to verify requests sent to the payload URL. <br />
        <ReadMore href="">Read more</ReadMore>
      </InfoHint>
      <div hidden={isResetVisible}>
        <PasswordInput
          onChange={onChange}
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
