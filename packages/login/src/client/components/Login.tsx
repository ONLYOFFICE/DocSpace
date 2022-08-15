import React, { useState, useCallback } from "react";
import { useTranslation } from "react-i18next";
import {
  ButtonsWrapper,
  LoginContainer,
  LoginFormWrapper,
} from "./StyledLogin";
import Logo from "../../../../../public/images/docspace.big.react.svg";
import Text from "@docspace/components/text";
import SocialButton from "@docspace/components/social-button";
import {
  getProviderTranslation,
  combineUrl,
  getOAuthToken,
  getLoginLink,
} from "@docspace/common/utils";
import { providersData, AppServerConfig } from "@docspace/common/constants";

const { proxyURL } = AppServerConfig;
const greetingTitle = "Web Office Applications"; // from PortalSettingsStore

interface ILoginProps {
  portalSettings: IPortalSettings;
  buildInfo: IBuildInfo;
  providers: ProvidersType;
  capabilities: ICapabilities;
  isDesktopEditor?: boolean;
}
const App: React.FC<ILoginProps> = ({
  portalSettings,
  buildInfo,
  providers,
  capabilities,
  isDesktopEditor,
  ...rest
}) => {
  const [isLoading, setIsLoading] = useState(false);

  const { enabledJoin } = portalSettings;
  const { ssoLabel, ssoUrl } = capabilities;

  const { t } = useTranslation(["Login", "Common"]);

  const ssoExists = () => {
    if (ssoUrl) return true;
    else return false;
  };

  const ssoButton = () => {
    return (
      <div className="buttonWrapper">
        <SocialButton
          iconName="/static/images/sso.react.svg"
          className="socialButton"
          label={ssoLabel || getProviderTranslation("sso", t)}
          onClick={() => (window.location.href = ssoUrl)}
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
    (e: React.SyntheticEvent<EventTarget>) => {
      if (!(e.target instanceof HTMLButtonElement)) {
        return;
      }
      const providerName = e.target.dataset.providername;
      const url = e.target.dataset.url || "";

      try {
        const tokenGetterWin = isDesktopEditor
          ? (window.location.href = url)
          : window.open(
              url,
              "login",
              "width=800,height=500,status=no,toolbar=no,menubar=no,resizable=yes,scrollbars=no"
            );

        getOAuthToken(tokenGetterWin).then((code) => {
          const token = window.btoa(
            JSON.stringify({
              auth: providerName,
              mode: "popup",
              callback: "authCallback",
            })
          );

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

  return (
    <LoginFormWrapper enabledJoin={enabledJoin} isDesktop={isDesktopEditor}>
      <LoginContainer>
        <Logo className="logo-wrapper" />
        <Text
          fontSize="23px"
          fontWeight={700}
          textAlign="center"
          className="greeting-title"
        >
          {greetingTitle}
        </Text>
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
      </LoginContainer>
    </LoginFormWrapper>
  );
};

export default App;
