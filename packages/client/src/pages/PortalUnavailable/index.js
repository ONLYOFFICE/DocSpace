import React, { useState } from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";
import Button from "@docspace/components/button";
import RecoverAccessModalDialog from "login/recoverAccessModalDialog";

const StyledBodyContent = styled.div`
  max-width: 480px;
  text-align: center;
  button {
    margin-top: 24px;
    max-width: 320px;
  }
`;
const StyledBody = styled.div`
  display: flex;
  flex-direction: column;
  margin: 0 auto;
  .portal-unavailable_svg {
    margin: 0 auto;
    margin-top: 110px;
    height: 44px;
    svg {
      height: 44px;
      width: 100%;
    }
  }

  .portal-unavailable_container {
    padding: 55px;

    .portal-unavailable_contact-text {
      text-decoration: underline;
      cursor: pointer;
      margin-top: 26px;
    }
  }

  @media (max-width: 768px) {
    .portal-unavailable_svg {
      margin-top: 0px;
      background: ${(props) => props.theme.catalog.background};
      width: 100%;
      height: 48px;
      padding: 12px;
      box-sizing: border-box;
      svg {
        height: 22px;
      }
    }
  }
`;

const PortalUnavailable = ({ theme, logoUrl, onLogoutClick }) => {
  const { t, ready } = useTranslation(["PortalUnavailable", "Common"]);
  const [isVisible, setIsVisible] = useState();

  const onClick = () => {
    onLogoutClick();
  };
  const onClickToContact = () => {
    setIsVisible(true);
  };
  const onCloseDialog = () => {
    setIsVisible(false);
  };
  return (
    <StyledBody theme={theme}>
      <ReactSVG
        className="portal-unavailable_svg"
        src={logoUrl}
        beforeInjection={(svg) => {}}
      />
      <RecoverAccessModalDialog
        visible={isVisible}
        t={t}
        emailPlaceholderText={t("Common:RegistrationEmail")}
        textBody={t("AccessingProblem")}
        onClose={onCloseDialog}
      />
      <ErrorContainer
        className="portal-unavailable_container"
        headerText={t("PortalUnavailable")}
      >
        <StyledBodyContent>
          <Text
            textAlign="center"
            className="portal-unavailable_text"
            color={theme.text.disableColor}
          >
            {t("AccessingProblem")}
          </Text>
          <Button
            scale
            label={t("Common:LogoutButton")}
            size={"medium"}
            onClick={onClick}
          />
          <Text
            textAlign="center"
            className="portal-unavailable_contact-text"
            onClick={onClickToContact}
            color={theme.login.linkColor}
          >
            {t("ContactAdministrator")}
          </Text>
        </StyledBodyContent>
      </ErrorContainer>
    </StyledBody>
  );
};

export default inject(({ auth, profileActionsStore }) => {
  const { onLogoutClick } = profileActionsStore;
  const { theme, logoUrl } = auth.settingsStore;
  return {
    logoUrl,
    theme,
    onLogoutClick,
  };
})(observer(PortalUnavailable));
