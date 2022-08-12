import React from "react";
import {
  ButtonsWrapper,
  LoginContainer,
  LoginFormWrapper,
} from "./StyledLogin";
import Logo from "../../../../../public/images/docspace.big.react.svg";
import Text from "@docspace/components/text";

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
  const { enabledJoin } = portalSettings;

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
      </LoginContainer>
    </LoginFormWrapper>
  );
};

export default App;
