import React, { useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Button from "@appserver/components/button";
import TextInput from "@appserver/components/text-input";
import Text from "@appserver/components/text";
import Section from "@appserver/common/components/Section";
import { inject, observer } from "mobx-react";
import withLoader from "../withLoader";
import { Base } from "@appserver/components/themes";

const BodyStyle = styled.div`
  margin: 70px auto 0 auto;
  max-width: 432px;

  .edit-header {
    .header-logo {
      max-width: 216px;
      max-height: 35px;
    }

    .header-title {
      word-wrap: break-word;
      margin: 8px 0;
      text-align: left;
      font-size: 24px;
      color: ${(props) => props.theme.studio.confirm.change.titleColor};
    }
  }

  .edit-text {
    margin-bottom: 18px;
  }

  .edit-input {
    margin-bottom: 24px;
  }
`;

BodyStyle.defaultProps = { theme: Base };

const PhoneForm = (props) => {
  const { t, currentPhone, greetingTitle } = props;

  const [phone, setPhone] = useState(currentPhone);
  // eslint-disable-next-line no-unused-vars
  const [isLoading, setIsLoading] = useState(false);

  const subTitleTranslation = `Enter mobile phone number`;
  const infoTranslation = `Your current mobile phone number`;
  const subInfoTranslation = `The two-factor authentication is enabled to provide additional portal security.
                              Enter your mobile phone number to continue work on the portal.
                              Mobile phone number must be entered using an international format with country code.`;
  const phonePlaceholder = `Phone`;
  const buttonTranslation = `Enter number`;

  const onSubmit = () => {
    console.log("onSubmit CHANGE"); //TODO: Why do nothing?
  };

  const onKeyPress = (target) => {
    if (target.code === "Enter") onSubmit();
  };

  const simplePhoneMask = new Array(15).fill(/\d/);

  return (
    <BodyStyle>
      <div className="edit-header">
        <img className="header-logo" src="images/dark_general.png" alt="Logo" />
        <div className="header-title">{greetingTitle}</div>
      </div>
      <Text className="edit-text" isBold fontSize="14px">
        {subTitleTranslation}
      </Text>
      <Text fontSize="13px">
        {infoTranslation}: <b>+{currentPhone}</b>
      </Text>
      <Text className="edit-text" fontSize="13px">
        {subInfoTranslation}
      </Text>
      <TextInput
        id="phone"
        name="phone"
        type="text"
        size="huge"
        scale={true}
        isAutoFocussed={true}
        tabIndex={1}
        autocomple="off"
        placeholder={phonePlaceholder}
        onChange={(event) => {
          setPhone(event.target.value);
          onKeyPress(event.target);
        }}
        value={phone}
        hasError={false}
        isDisabled={isLoading}
        onKeyDown={(event) => onKeyPress(event.target)}
        guide={false}
        mask={simplePhoneMask}
        className="edit-input"
      />
      <Button
        primary
        size="normal"
        tabIndex={3}
        label={isLoading ? t("Common:LoadingProcessing") : buttonTranslation}
        isDisabled={isLoading}
        isLoading={isLoading}
        onClick={onSubmit}
      />
    </BodyStyle>
  );
};

const ChangePhoneForm = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <PhoneForm {...props} />
      </Section.SectionBody>
    </Section>
  );
};

export default inject(({ auth }) => ({
  isLoaded: auth.isLoaded,
  currentPhone: auth.userStore.mobilePhone,
  greetingTitle: auth.settingsStore.greetingSettings,
}))(
  withRouter(
    withTranslation(["Confirm", "Common"])(
      withLoader(observer(ChangePhoneForm))
    )
  )
);
