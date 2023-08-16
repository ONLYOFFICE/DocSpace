import React, { useState, useCallback, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { ButtonsWrapper, LoginFormWrapper, LoginContent } from "./StyledLogin";
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
import MoreLoginModal from "@docspace/common/components/MoreLoginModal";
import RecoverAccessModalDialog from "@docspace/common/components/Dialogs/RecoverAccessModalDialog";
import FormWrapper from "@docspace/components/form-wrapper";
import Register from "./sub-components/register-container";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import SSOIcon from "PUBLIC_DIR/images/sso.react.svg";
import { Dark, Base } from "@docspace/components/themes";
import { useMounted } from "../helpers/useMounted";
import { getBgPattern } from "@docspace/common/utils";
import useIsomorphicLayoutEffect from "../hooks/useIsomorphicLayoutEffect";
import { getLogoFromPath } from "@docspace/common/utils";
import { useThemeDetector } from "@docspace/common/utils/useThemeDetector";
import { TenantStatus } from "@docspace/common/constants";

interface ILoginProps extends IInitialState {
  isDesktopEditor?: boolean;
  theme: IUserTheme;
  setTheme: (theme: IUserTheme) => void;
}

const Login: React.FC<ILoginProps> = ({
  portalSettings,
  providers,
  capabilities,
  isDesktopEditor,
  match,
  currentColorScheme,
  theme,
  setTheme,
  logoUrls,
}) => {
  const isRestoringPortal =
    portalSettings?.tenantStatus === TenantStatus.PortalRestore;

  useEffect(() => {
    isRestoringPortal && window.location.replace("/preparation-portal");
  }, []);
  const [isLoading, setIsLoading] = useState(false);
  const [moreAuthVisible, setMoreAuthVisible] = useState(false);
  const [recoverDialogVisible, setRecoverDialogVisible] = useState(false);

  const {
    enabledJoin,
    greetingSettings,
    enableAdmMess,
    cookieSettingsEnabled,
  } = portalSettings || {
    enabledJoin: false,
    greetingSettings: false,
    enableAdmMess: false,
    cookieSettingsEnabled: false,
  };

  const ssoLabel = capabilities?.ssoLabel || "";
  const ssoUrl = capabilities?.ssoUrl || "";
  const { t } = useTranslation(["Login", "Common"]);
  const mounted = useMounted();
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

  const ssoExists = () => {
    if (ssoUrl) return true;
    else return false;
  };

  const ssoButton = () => {
    const onClick = () => (window.location.href = ssoUrl);
    return (
      <div className="buttonWrapper">
        <SocialButton
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
    if (!capabilities?.oauthEnabled) return false;

    let existProviders = 0;
    providers && providers.length > 0;
    providers?.map((item) => {
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

  const bgPattern = getBgPattern(currentColorScheme?.id);

  const logo = logoUrls && Object.values(logoUrls)[1];
  const logoUrl = !logo
    ? undefined
    : !theme?.isBase
    ? getLogoFromPath(logo.path.dark)
    : getLogoFromPath(logo.path.light);

  if (!mounted) return <></>;
  if (isRestoringPortal) return <></>;

  return (
    <LoginFormWrapper
      id="login-page"
      enabledJoin={enabledJoin}
      isDesktop={isDesktopEditor}
      bgPattern={bgPattern}
    >
      <div className="bg-cover"></div>
      <LoginContent enabledJoin={enabledJoin}>
        <ColorTheme themeId={ThemeType.LinkForgotPassword} theme={theme}>
          <img src={logoUrl} className="logo-wrapper" />
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
                    color={currentColorScheme?.main?.accent}
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
                <Text className="or-label">{t("Or")}</Text>
              </div>
            )}
            <LoginForm
              isDesktop={!!isDesktopEditor}
              isLoading={isLoading}
              hashSettings={portalSettings?.passwordHash}
              setIsLoading={setIsLoading}
              onRecoverDialogVisible={onRecoverDialogVisible}
              match={match}
              enableAdmMess={enableAdmMess}
              cookieSettingsEnabled={cookieSettingsEnabled}
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
            id="recover-access-modal"
          />
        </ColorTheme>
      </LoginContent>

      {!checkIsSSR() && enabledJoin && (
        <Register
          id="login_register"
          enabledJoin={enabledJoin}
          currentColorScheme={currentColorScheme}
          trustedDomains={portalSettings?.trustedDomains}
          trustedDomainsType={portalSettings?.trustedDomainsType}
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
