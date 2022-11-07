import React, { useEffect, useState, useCallback } from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { ButtonsWrapper, LoginFormWrapper } from "./StyledLogin";
import Text from "@docspace/components/text";
import SocialButton from "@docspace/components/social-button";
import {
  getProviderTranslation,
  getOAuthToken,
  getLoginLink,
  checkIsSSR,
} from "@docspace/common/utils";
import { providersData } from "@docspace/common/constants";
import Link from "@docspace/components/link";
import Toast from "@docspace/components/toast";
import LoginForm from "./sub-components/LoginForm";
import MoreLoginModal from "./sub-components/more-login";
import RecoverAccessModalDialog from "@docspace/common/components/Dialogs/RecoverAccessModalDialog";
import FormWrapper from "@docspace/components/form-wrapper";
import Register from "./sub-components/register-container";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import SSOIcon from "../../../../../public/images/sso.react.svg";
import { Dark, Base } from "@docspace/components/themes";
import { useMounted } from "../helpers/useMounted";
import { getBgPattern } from "@docspace/common/utils";
import { ReactSVG } from "react-svg";

interface ILoginProps extends IInitialState {
  isDesktopEditor?: boolean;
}
const Login: React.FC<ILoginProps> = ({
  portalSettings,
  buildInfo,
  providers,
  capabilities,
  isDesktopEditor,
  match,
  currentColorScheme,
  theme,
  setTheme,
  logoUrls,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [moreAuthVisible, setMoreAuthVisible] = useState(false);
  const [recoverDialogVisible, setRecoverDialogVisible] = useState(false);

  const { enabledJoin, greetingSettings } = portalSettings;
  const { ssoLabel, ssoUrl } = capabilities;

  const { t } = useTranslation(["Login", "Common"]);
  const mounted = useMounted();

  useEffect(() => {
    const theme =
      window.matchMedia &&
      window.matchMedia("(prefers-color-scheme: dark)").matches
        ? Dark
        : Base;
    setTheme(theme);
  }, []);

  const ssoExists = () => {
    if (ssoUrl) return true;
    else return false;
  };
  const ssoButton = () => {
    const onClick = () => (window.location.href = ssoUrl);
    return (
      <div className="buttonWrapper">
        <SocialButton
          //iconName="/static/images/sso.react.svg"
          IconComponent={SSOIcon}
          className="socialButton"
          label={ssoLabel || getProviderTranslation("sso", t)}
          onClick={onClick}
          isDisabled={isLoading}
        />
      </div>
    );
  };

  const oauthDataExists = () => {
    let existProviders = 0;
    providers && providers.length > 0;
    providers.map((item) => {
      if (!providersData[item.provider]) return;
      existProviders++;
    });

    return !!existProviders;
  };

  const onSocialButtonClick = useCallback(
    (e: HTMLElementEvent<HTMLButtonElement | HTMLElement>) => {
      const { target } = e;
      let targetElement = target;

      if (
        !(targetElement instanceof HTMLButtonElement) &&
        target.parentElement
      ) {
        targetElement = target.parentElement;
      }
      const providerName = targetElement.dataset.providername;
      const url = targetElement.dataset.url || "";

      try {
        const tokenGetterWin = isDesktopEditor
          ? (window.location.href = url)
          : window.open(
              url,
              "login",
              "width=800,height=500,status=no,toolbar=no,menubar=no,resizable=yes,scrollbars=no"
            );

        getOAuthToken(tokenGetterWin).then((code: string) => {
          const token = window.btoa(
            JSON.stringify({
              auth: providerName,
              mode: "popup",
              callback: "authCallback",
            })
          );
          if (tokenGetterWin && typeof tokenGetterWin !== "string")
            tokenGetterWin.location.href = getLoginLink(token, code);
        });
      } catch (err) {
        console.log(err);
      }
    },
    []
  );

  const providerButtons = () => {
    const providerButtons =
      providers &&
      providers.map((item, index) => {
        if (!providersData[item.provider]) return;
        if (index > 1) return;

        const { icon, label, iconOptions, className } = providersData[
          item.provider
        ];

        return (
          <div className="buttonWrapper" key={`${item.provider}ProviderItem`}>
            <SocialButton
              iconName={icon}
              label={getProviderTranslation(label, t)}
              className={`socialButton ${className ? className : ""}`}
              $iconOptions={iconOptions}
              data-url={item.url}
              data-providername={item.provider}
              onClick={onSocialButtonClick}
              isDisabled={isLoading}
            />
          </div>
        );
      });

    return providerButtons;
  };

  const moreAuthOpen = () => {
    setMoreAuthVisible(true);
  };

  const moreAuthClose = () => {
    setMoreAuthVisible(false);
  };

  const onRecoverDialogVisible = () => {
    setRecoverDialogVisible(!recoverDialogVisible);
  };

  const bgPattern = getBgPattern(currentColorScheme.id);

  const loginLogo = Object.values(logoUrls)[1];
  const isSvgLogo = loginLogo.includes(".svg");

  if (!mounted) return <></>;

  return (
    <LoginFormWrapper
      id="login-page"
      enabledJoin={enabledJoin}
      isDesktop={isDesktopEditor}
      //className="with-background-pattern"
      bgPattern={bgPattern}
    >
      <ColorTheme themeId={ThemeType.LinkForgotPassword} theme={theme}>
        {isSvgLogo ? (
          <ReactSVG src={loginLogo} className="logo-wrapper" />
        ) : (
          <img src={loginLogo} className="logo-wrapper" />
        )}
        <Text
          fontSize="23px"
          fontWeight={700}
          textAlign="center"
          className="greeting-title"
        >
          {greetingSettings}
        </Text>
        <FormWrapper id="login-form" theme={theme}>
          {ssoExists() && <ButtonsWrapper>{ssoButton()}</ButtonsWrapper>}
          {oauthDataExists() && (
            <>
              <ButtonsWrapper>{providerButtons()}</ButtonsWrapper>
              {providers && providers.length > 2 && (
                <Link
                  isHovered
                  type="action"
                  fontSize="13px"
                  fontWeight="600"
                  color="#3B72A7"
                  className="more-label"
                  onClick={moreAuthOpen}
                >
                  {t("Common:ShowMore")}
                </Link>
              )}
            </>
          )}
          {(oauthDataExists() || ssoExists()) && (
            <div className="line">
              <Text color="#A3A9AE" className="or-label">
                {t("Or")}
              </Text>
            </div>
          )}
          <LoginForm
            isDesktop={!!isDesktopEditor}
            isLoading={isLoading}
            hashSettings={portalSettings.passwordHash}
            setIsLoading={setIsLoading}
            onRecoverDialogVisible={onRecoverDialogVisible}
            match={match}
          />
        </FormWrapper>
        <Toast />
        <MoreLoginModal
          visible={moreAuthVisible}
          onClose={moreAuthClose}
          providers={providers}
          onSocialLoginClick={onSocialButtonClick}
          ssoLabel={ssoLabel}
          ssoUrl={ssoUrl}
          t={t}
        />

        <RecoverAccessModalDialog
          visible={recoverDialogVisible}
          onClose={onRecoverDialogVisible}
          textBody={t("RecoverTextBody")}
          emailPlaceholderText={t("RecoverContactEmailPlaceholder")}
        />
      </ColorTheme>
      {!checkIsSSR() && enabledJoin && (
        <Register
          enabledJoin={enabledJoin}
          currentColorScheme={currentColorScheme}
        />
      )}
    </LoginFormWrapper>
  );
};

export default inject(({ loginStore }) => {
  return {
    theme: loginStore.theme,
    setTheme: loginStore.setTheme,
  };
})(observer(Login));
