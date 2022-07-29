import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import SimpleCheckbox from "./SimpleCheckbox";

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
  } = props;

  return (
    <Box marginProp="12px 0">
      <SimpleCheckbox
        label={prefix === "idp" ? t("idpAuthRequest") : t("spAuthRequest")}
        name={checkboxesNames[prefix][0]}
        tabIndex={10}
        isChecked={
          prefix === "idp" ? idpVerifyAuthResponsesSign : spSignAuthRequests
        }
      />
      <SimpleCheckbox
        label={
          prefix === "idp" ? t("idpSignExitRequest") : t("spSignExitRequest")
        }
        name={checkboxesNames[prefix][1]}
        tabIndex={11}
        isChecked={
          prefix === "idp" ? idpVerifyLogoutRequestsSign : spSignLogoutRequests
        }
      />
      <SimpleCheckbox
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
        <SimpleCheckbox
          label={t("spDecryptStatements")}
          name={checkboxesNames[prefix][3]}
          tabIndex={13}
          isChecked={spEncryptAssertions}
        />
      )}
    </Box>
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
  } = ssoStore;

  return {
    idpVerifyAuthResponsesSign,
    idpVerifyLogoutRequestsSign,
    idpVerifyLogoutResponsesSign,
    spSignAuthRequests,
    spSignLogoutRequests,
    spSignLogoutResponses,
    spEncryptAssertions,
  };
})(observer(CheckboxSet));
