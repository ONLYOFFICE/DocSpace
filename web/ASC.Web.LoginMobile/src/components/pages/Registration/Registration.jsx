import React, { useEffect } from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";

import { Box } from "ASC.Web.Components";
import i18n from "../../../i18n";

import Header from "../../Header";
import { RegistrationTitle } from "./sub-components";

const StyledRegistration = styled(Box)`
  display: grid;
  grid-template-columns: 1fr;
  grid-gap: 30px;
  align-items: center;
  margin: 33px 32px 0 32px;
  height: min-content;
`;

const RegistrationComponent = ({ t }) => {
  return (
    <>
      <StyledRegistration>
        <RegistrationTitle t={t} />
        Registration
      </StyledRegistration>
    </>
  );
};

const RegistrationWrapper = withTranslation()(RegistrationComponent);

const Registration = (props) => {
  const { language } = props;
  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  return <RegistrationWrapper i18n={i18n} {...props} />;
};

export default Registration;
