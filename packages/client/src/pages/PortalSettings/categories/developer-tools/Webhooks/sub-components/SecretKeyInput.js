import React, { useState, useEffect, useRef } from "react";
import styled from "styled-components";

import InfoIcon from "PUBLIC_DIR/images/info.react.svg?url";
import Link from "@docspace/components/link";
import Label from "@docspace/components/label";

import { Hint } from "../styled-components";

import PasswordInput from "@docspace/components/password-input";
import { inject, observer } from "mobx-react";

import { useTranslation } from "react-i18next";

const SecretKeyWrapper = styled.div`
  .link {
    display: inline-block;
    margin-top: 6px;
  }

  .dotted {
    text-decoration: underline dotted;
  }
`;

const Header = styled.p`
  margin-top: 20px;

  display: block;
  align-items: center;
  margin-bottom: 5px;

  img {
    margin-left: 4px;
  }
`;

const StyledInfoIcon = styled.img`
  :hover {
    cursor: pointer;
  }
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
    webhooksGuideUrl,
    passwordInputKey,
    additionalId,
  } = props;

  const [isHintVisible, setIsHintVisible] = useState(false);
  const { t } = useTranslation(["Webhooks"]);

  const secretKeyInputRef = useRef(null);

  const toggleHint = () =>
    setIsHintVisible((prevIsHintVisible) => !prevIsHintVisible);

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
      onChange({
        target: { name, value: secretKeyInputRef.current.state.inputValue },
      });
    }
  }, [isResetVisible]);

  return (
    <SecretKeyWrapper>
      <Label
        text={
          <Header>
            {t("SecretKey")}
            <StyledInfoIcon
              className="secret-key-tooltip"
              src={InfoIcon}
              alt="infoIcon"
              onClick={toggleHint}
            />
          </Header>
        }
        fontWeight={600}
      >
        <Hint isTooltip hidden={!isHintVisible} onClick={handleHintDisapear}>
          {t("SecretKeyHint")} <br />
          <Link
            id="read-more-link"
            type="page"
            isHovered
            fontWeight={600}
            href={webhooksGuideUrl}
            target="_blank"
            className="link"
            color="#333333"
          >
            {t("ReadMore")}
          </Link>
        </Hint>
        {isResetVisible && (
          <Hint>
            {t("SecretKeyWarning")} <br />
            <Link
              id="reset-key-link"
              type="action"
              fontWeight={600}
              isHovered={true}
              onClick={hideReset}
              className="link"
              color="#333333"
            >
              {t("ResetKey")}
            </Link>
          </Hint>
        )}
        <div hidden={isResetVisible}>
          <PasswordInput
            id={additionalId + "-secret-key-input"}
            onChange={handleOnChange}
            inputValue={value}
            inputName={name}
            placeholder={t("EnterSecretKey")}
            onValidateInput={handleInputValidation}
            ref={secretKeyInputRef}
            hasError={!isPasswordValid}
            isDisableTooltip={true}
            inputType="password"
            isFullWidth={true}
            passwordSettings={passwordSettings}
            key={passwordInputKey}
          />
          <Link
            id="generate-link"
            type="action"
            fontWeight={600}
            isHovered={true}
            onClick={generatePassword}
            className="link dotted"
          >
            {t("Generate")}
          </Link>
        </div>
      </Label>
    </SecretKeyWrapper>
  );
};

export default inject(({ settingsStore, auth }) => {
  const { passwordSettings } = settingsStore;
  const { webhooksGuideUrl } = auth.settingsStore;

  return {
    passwordSettings,
    webhooksGuideUrl,
  };
})(observer(SecretKeyInput));
