import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import CodeInput from "@docspace/components/code-input";
import { Trans } from "react-i18next";
import { ReactSVG } from "react-svg";
import { LoginFormWrapper } from "./StyledLogin";
import BarLogo from "PUBLIC_DIR/images/danger.alert.react.svg";
import { Dark, Base } from "@docspace/components/themes";
import { getBgPattern, getLogoFromPath } from "@docspace/common/utils";
import { useMounted } from "../helpers/useMounted";
import useIsomorphicLayoutEffect from "../hooks/useIsomorphicLayoutEffect";
import LoginContainer from "@docspace/components/ColorTheme/styled/sub-components/LoginContainer";
import { useThemeDetector } from "@docspace/common/utils/useThemeDetector";

interface ILoginProps extends IInitialState {
  isDesktopEditor?: boolean;
  theme: IUserTheme;
  setTheme: (theme: IUserTheme) => void;
}

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

const Form: React.FC<ILoginProps> = ({ theme, setTheme, logoUrls }) => {
  const { t } = useTranslation("Login");
  const [invalidCode, setInvalidCode] = useState(false);
  const [expiredCode, setExpiredCode] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const email = "test@onlyoffice.com"; //TODO: get email from form
  const validCode = "123456"; //TODO: get from api
  const systemTheme = typeof window !== "undefined" && useThemeDetector();

  useIsomorphicLayoutEffect(() => {
    const theme =
      window.matchMedia &&
      window.matchMedia("(prefers-color-scheme: dark)").matches
        ? Dark
        : Base;
    setTheme(theme);
  }, []);

  useIsomorphicLayoutEffect(() => {
    if (systemTheme === "Base") setTheme(Base);
    else setTheme(Dark);
  }, [systemTheme]);

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

  const logo = logoUrls && Object.values(logoUrls)[1];
  const logoUrl = !logo
    ? undefined
    : !theme?.isBase
    ? getLogoFromPath(logo.path.dark)
    : getLogoFromPath(logo.path.light);

  return (
    <LoginContainer id="code-page" theme={theme}>
      <img src={logoUrl} className="logo-wrapper" />
      <Text
        id="workspace-title"
        fontSize="23px"
        fontWeight={700}
        textAlign="center"
        className="workspace-title"
      >
        {t("CodeTitle")}
      </Text>

      <Text
        className="code-description"
        fontSize="12px"
        fontWeight={400}
        textAlign="center"
      >
        <Trans t={t} i18nKey="CodeSubtitle" ns="Login" key={email}>
          We sent a 6-digit code to {{ email }}. The code has a limited validity
          period, enter it as soon as possible.{" "}
        </Trans>
      </Text>

      <div className="code-input-container">
        <CodeInput
          theme={theme}
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
          className="not-found-code code-description"
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
  const bgPattern = getBgPattern(props.currentColorScheme.id);
  const mounted = useMounted();

  if (!mounted) return <></>;
  return (
    <LoginFormWrapper bgPattern={bgPattern}>
      <Form {...props} />
    </LoginFormWrapper>
  );
};

export default inject(({ loginStore }) => {
  return {
    theme: loginStore.theme,
    setTheme: loginStore.setTheme,
  };
})(observer(CodeLogin));
