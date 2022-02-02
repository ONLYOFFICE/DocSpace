import React, { useState, useRef } from "react";
import PageLayout from "@appserver/common/components/PageLayout";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import {
  I18nextProvider,
  useTranslation,
  withTranslation,
} from "react-i18next";
import Text from "@appserver/components/text";
import FieldContainer from "@appserver/components/field-container";
import TextInput from "@appserver/components/text-input";
import Button from "@appserver/components/button";
import Link from "@appserver/components/link";
import { Trans } from "react-i18next";

import i18n from "./i18n";
import {
  ButtonsWrapper,
  LoginContainer,
  LoginFormWrapper,
} from "./StyledLogin";

const Form = () => {
  const inputRef = useRef(null);
  const { t } = useTranslation("Login");
  const [isLoading, setIsLoading] = useState(false);
  const [url, setUrl] = useState("");

  const onChangeLogin = (e) => {
    setUrl(e.target.value);
  };

  return (
    <LoginContainer>
      <Text
        fontSize="23px"
        fontWeight={700}
        textAlign="center"
        className="workspace-title"
      >
        {t("WorkspaceLoginTitle")}
      </Text>

      <Text fontSize="12px" fontWeight={400} textAlign="center" color="#A3A9AE">
        {t("WorkspaceLoginSubtitle")}
      </Text>

      <form className="auth-form-container">
        <FieldContainer isVertical={true} labelVisible={false}>
          <TextInput
            id="url"
            name="url"
            type="text"
            value={url}
            placeholder="URL"
            size="large"
            scale={true}
            isAutoFocussed={true}
            tabIndex={1}
            isDisabled={isLoading}
            onChange={onChangeLogin}
            forwardedRef={inputRef}
          />
        </FieldContainer>

        <Button
          id="submit"
          className="login-button"
          primary
          size="large"
          scale={true}
          label={isLoading ? t("Common:LoadingProcessing") : t("LoginButton")}
          tabIndex={1}
          isDisabled={isLoading}
          isLoading={isLoading}
          onClick={() => console.log("Log in")}
        />

        <Text color="#A3A9AE" fontSize="12px">
          <Trans t={t} i18nKey="WorkspaceLoginDescription" ns="Login">
            Don't know your portal URL?
            <Link color="#2DA7DB;" fontSize="12px" href="">
              Find your workspaces
            </Link>
          </Trans>
        </Text>
      </form>
    </LoginContainer>
  );
};

const WorkspaceLoginForm = (props) => {
  return (
    <LoginFormWrapper>
      <PageLayout>
        <PageLayout.SectionBody>
          <Form {...props} />
        </PageLayout.SectionBody>
      </PageLayout>
    </LoginFormWrapper>
  );
};

const WorkspaceLogin = withRouter(
  observer(withTranslation(["Login", "Common"])(WorkspaceLoginForm))
);

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <WorkspaceLogin {...props} />
  </I18nextProvider>
);
