import React from "react";
import styled from "styled-components";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { I18nextProvider, useTranslation } from "react-i18next";
import i18n from "./i18n";
import DocspaceLogo from "../../../DocspaceLogo";

const StyledWrapper = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 64px;
`;

const ErrorUnavailable = ({ logoUrl }) => {
  const { t, ready } = useTranslation("Errors");

  return ready ? (
    <StyledWrapper>
      <DocspaceLogo />
      <ErrorContainer headerText={t("ErrorDeactivatedText")} />
    </StyledWrapper>
  ) : (
    <></>
  );
};

export default () => (
  <I18nextProvider i18n={i18n}>
    <ErrorUnavailable />
  </I18nextProvider>
);
