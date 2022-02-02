import React from "react";
import PageLayout from "@appserver/common/components/PageLayout";
import { withRouter } from "react-router";
import {
  I18nextProvider,
  useTranslation,
  withTranslation,
} from "react-i18next";
import Text from "@appserver/components/text";
import CodeInput from "@appserver/components/code-input";
import { Trans } from "react-i18next";

import i18n from "./i18n";
import { LoginContainer, LoginFormWrapper } from "./StyledLogin";

const Form = () => {
  const { t } = useTranslation("Login");
  const email = "test@onlyoffice.com";

  const onSubmit = (code) => {
    console.log("Code ", code);
  };

  return (
    <LoginContainer>
      <Text
        fontSize="23px"
        fontWeight={700}
        textAlign="center"
        className="workspace-title"
      >
        {t("CodeTitle")}
      </Text>

      <Text fontSize="12px" fontWeight={400} textAlign="center" color="#A3A9AE">
        <Trans t={t} i18nKey="CodeSubtitle" ns="Login" key={email}>
          We sent a 6-digit code to {{ email }}. The code has a limited validity
          period, enter it as soon as possible.{" "}
        </Trans>
      </Text>

      <form className="auth-form-container">
        <CodeInput className="code-input" onSubmit={onSubmit} />

        <Text color="#A3A9AE" fontSize="12px" textAlign="center">
          {t("NotFoundCode")}
        </Text>
      </form>
    </LoginContainer>
  );
};

const CodeLoginForm = (props) => {
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

const CodeLogin = withRouter(withTranslation("Login")(CodeLoginForm));

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <CodeLogin {...props} />
  </I18nextProvider>
);
