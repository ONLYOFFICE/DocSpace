import React from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Checkbox from "@docspace/components/checkbox";

const checkboxesNames = {
  idp: [
    "idpVerifyAuthResponsesSign",
    "idpVerifyLogoutRequestsSign",
    "idpVerifyLogoutResponsesSign",
  ],
  sp: [
    "spSignAuthRequests",
    "spSignLogoutRequests",
    "spSignLogoutResponses",
    "spEncryptAssertions",
  ],
};

const StyledWrapper = styled.div`
  margin: 16px 0;
  .checkbox-input {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin: 10px 0 6px 8px;
          `
        : css`
            margin: 10px 8px 6px 0;
          `}
  }
`;

const CheckboxSet = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    prefix,
    idpVerifyAuthResponsesSign,
    idpVerifyLogoutRequestsSign,
    idpVerifyLogoutResponsesSign,
    spSignAuthRequests,
    spSignLogoutRequests,
    spSignLogoutResponses,
    spEncryptAssertions,
    enableSso,
    setCheckbox,
    isLoadingXml,
  } = props;

  return (
    <StyledWrapper>
      <Checkbox
        id={
          prefix === "idp"
            ? "idp-verify-auth-responses-sign"
            : "sp-sign-auth-requests"
        }
        className="checkbox-input"
        isDisabled={!enableSso || isLoadingXml}
        onChange={setCheckbox}
        label={prefix === "idp" ? t("idpAuthRequest") : t("spAuthRequest")}
        name={checkboxesNames[prefix][0]}
        tabIndex={10}
        isChecked={
          prefix === "idp" ? idpVerifyAuthResponsesSign : spSignAuthRequests
        }
      />
      <Checkbox
        id={
          prefix === "idp"
            ? "idp-verify-logout-requests-sign"
            : "sp-sign-logout-requests"
        }
        className="checkbox-input"
        isDisabled={!enableSso || isLoadingXml}
        onChange={setCheckbox}
        label={
          prefix === "idp" ? t("idpSignExitRequest") : t("spSignExitRequest")
        }
        name={checkboxesNames[prefix][1]}
        tabIndex={11}
        isChecked={
          prefix === "idp" ? idpVerifyLogoutRequestsSign : spSignLogoutRequests
        }
      />
      <Checkbox
        id={
          prefix === "idp"
            ? "idp-verify-logout-responses-sign"
            : "sp-sign-logout-responses"
        }
        className="checkbox-input"
        isDisabled={!enableSso || isLoadingXml}
        onChange={setCheckbox}
        label={
          prefix === "idp"
            ? t("idpSignResponseRequest")
            : t("spSignResponseRequest")
        }
        name={checkboxesNames[prefix][2]}
        tabIndex={12}
        isChecked={
          prefix === "idp"
            ? idpVerifyLogoutResponsesSign
            : spSignLogoutResponses
        }
      />

      {prefix === "sp" && (
        <Checkbox
          id="sp-encrypt-assertions"
          className="checkbox-input"
          isDisabled={!enableSso || isLoadingXml}
          onChange={setCheckbox}
          label={t("spDecryptStatements")}
          name={checkboxesNames[prefix][3]}
          tabIndex={13}
          isChecked={spEncryptAssertions}
        />
      )}
    </StyledWrapper>
  );
};

export default inject(({ ssoStore }) => {
  const {
    idpVerifyAuthResponsesSign,
    idpVerifyLogoutRequestsSign,
    idpVerifyLogoutResponsesSign,
    spSignAuthRequests,
    spSignLogoutRequests,
    spSignLogoutResponses,
    spEncryptAssertions,
    enableSso,
    setCheckbox,
    isLoadingXml,
  } = ssoStore;

  return {
    idpVerifyAuthResponsesSign,
    idpVerifyLogoutRequestsSign,
    idpVerifyLogoutResponsesSign,
    spSignAuthRequests,
    spSignLogoutRequests,
    spSignLogoutResponses,
    spEncryptAssertions,
    enableSso,
    setCheckbox,
    isLoadingXml,
  };
})(observer(CheckboxSet));
