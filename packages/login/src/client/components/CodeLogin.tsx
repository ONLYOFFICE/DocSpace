import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import CodeInput from "@docspace/components/code-input";
import { Trans } from "react-i18next";
import { LoginContainer, LoginFormWrapper } from "./StyledLogin";
import BarLogo from "../../../../../public/images/danger.alert.react.svg";
import DocspaceLogo from "../../../../../public/images/docspace.big.react.svg";
interface IBarProp {
  t: TFuncType;
  expired: boolean;
}

const Bar: React.FC<IBarProp> = (props) => {
  const { t, expired } = props;
  const type = expired ? "warning" : "error";
  const text = expired ? t("ExpiredCode") : t("InvalidCode");

  return (
    <div className={`code-input-bar ${type}`}>
      <BarLogo />
      {text}
    </div>
  );
};

const Form: React.FC = () => {
  const { t } = useTranslation("Login");
  const [invalidCode, setInvalidCode] = useState(false);
  const [expiredCode, setExpiredCode] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const email = "test@onlyoffice.com"; //TODO: get email from form
  const validCode = "123456"; //TODO: get from api

  const onSubmit = (code: number | string) => {
    if (code !== validCode) {
      setInvalidCode(true);
    } else {
      console.log(`Code ${code}`); //TODO: send code on backend
      setIsLoading(true);
      setTimeout(() => setIsLoading(false), 5000); // fake
    }
  };

  const handleChange = () => {
    setInvalidCode(false);
  };

  return (
    <LoginContainer>
      <DocspaceLogo className="logo-wrapper" />

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

      <div className="code-input-container">
        <CodeInput
          onSubmit={onSubmit}
          handleChange={handleChange}
          isDisabled={isLoading}
        />
        {(expiredCode || invalidCode) && <Bar t={t} expired={expiredCode} />}

        {expiredCode && (
          <Link
            isHovered
            type="action"
            fontSize="13px"
            fontWeight="600"
            color="#3B72A7"
          >
            {t("ResendCode")}
          </Link>
        )}

        <Text
          className="not-found-code"
          color="#A3A9AE"
          fontSize="12px"
          textAlign="center"
        >
          {t("NotFoundCode")}
        </Text>
      </div>
    </LoginContainer>
  );
};

const CodeLogin: React.FC<ICodeProps> = (props) => {
  return (
    <LoginFormWrapper>
      <Form {...props} />
    </LoginFormWrapper>
  );
};

export default CodeLogin;
