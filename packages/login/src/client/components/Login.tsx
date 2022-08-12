import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import {
  ButtonsWrapper,
  LoginContainer,
  LoginFormWrapper,
} from "./StyledLogin";
import Logo from "../../../../../public/images/docspace.big.react.svg";
import Text from "@docspace/components/text";
import SocialButton from "@docspace/components/social-button";
import { getProviderTranslation } from "@docspace/common/utils";

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
      </LoginContainer>
    </LoginFormWrapper>
  );
};

export default App;
