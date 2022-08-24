import React from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";

const StyledBodyContent = styled.div`
  max-width: 480px;
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
    .portal-unavailable_text {
      color: ${(props) => props.theme.text.disableColor};
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

const PortalUnavailable = ({ theme, logoUrl }) => {
  const { t, ready } = useTranslation("PortalUnavailable");
  console.log("logoUrl", logoUrl);
  return (
    <StyledBody theme={theme}>
      <ReactSVG
        className="portal-unavailable_svg"
        src={logoUrl}
        beforeInjection={(svg) => {}}
      />

      <ErrorContainer
        className="portal-unavailable_container"
        headerText={t("PortalUnavailable")}
      >
        <StyledBodyContent>
          <Text textAlign="center" className="portal-unavailable_text">
            {t("AccessingProblem")}
          </Text>
        </StyledBodyContent>
      </ErrorContainer>
    </StyledBody>
  );
};

export default inject(({ auth }) => {
  const { theme, logoUrl } = auth.settingsStore;
  return {
    logoUrl,
    theme,
  };
})(observer(PortalUnavailable));
