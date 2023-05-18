import React, { useState, useEffect, useRef } from "react";
import styled from "styled-components";

import InfoIcon from "PUBLIC_DIR/images/info.react.svg?url";
import Link from "@docspace/components/link";

import { Hint } from "../styled-components";

import PasswordInput from "@docspace/components/password-input";
import { inject, observer } from "mobx-react";

import { useTranslation } from "react-i18next";

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
  const { t } = useTranslation(["Webhooks"]);

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
        {t("SecretKey")} <StyledInfoIcon src={InfoIcon} alt="infoIcon" onClick={toggleHint} />
      </Header>

      <Hint isTooltip hidden={!isHintVisible} onClick={handleHintDisapear}>
        {t("SecretKeyHint")} <br />
        <ReadMore href="">{t("ReadMore")}</ReadMore>
      </Hint>
      {isResetVisible && (
        <Hint>
          {t("SecretKeyWarning")} <br />
          <Link
            type="action"
            fontWeight={600}
            isHovered={true}
            onClick={hideReset}
            style={{ marginTop: "6px", display: "inline-block" }}>
            {t("ResetKey")}
          </Link>
        </Hint>
      )}
      <div hidden={isResetVisible}>
        <PasswordInput
          onChange={handleOnChange}
          value={value}
          inputName={name}
          placeholder={t("EnterSecretKey")}
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
          {t("Generate")}
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
