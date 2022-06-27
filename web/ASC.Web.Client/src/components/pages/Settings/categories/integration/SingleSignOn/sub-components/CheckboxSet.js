import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import SimpleCheckbox from "./SimpleCheckbox";

const checkboxesNames = {
  idp: [
    "idp_verifyAuthResponsesSign",
    "idp_verifyLogoutRequestsSign",
    "idp_verifyLogoutResponsesSign",
  ],
  sp: [
    "sp_signAuthRequests",
    "sp_signLogoutRequests",
    "sp_signLogoutResponses",
    "sp_encryptAssertions",
  ],
};

const CheckboxSet = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    prefix,
    idp_verifyAuthResponsesSign,
    idp_verifyLogoutRequestsSign,
    idp_verifyLogoutResponsesSign,
    sp_signAuthRequests,
    sp_signLogoutRequests,
    sp_signLogoutResponses,
    sp_encryptAssertions,
  } = props;

  return (
    <Box marginProp="12px 0">
      <SimpleCheckbox
        label={t(`${prefix}AuthRequest`)}
        name={checkboxesNames[prefix][0]}
        tabIndex={10}
        isChecked={
          prefix === "idp" ? idp_verifyAuthResponsesSign : sp_signAuthRequests
        }
      />
      <SimpleCheckbox
        label={t(`${prefix}SignExitRequest`)}
        name={checkboxesNames[prefix][1]}
        tabIndex={11}
        isChecked={
          prefix === "idp"
            ? idp_verifyLogoutRequestsSign
            : sp_signLogoutRequests
        }
      />
      <SimpleCheckbox
        label={t(`${prefix}SignResponseRequest`)}
        name={checkboxesNames[prefix][2]}
        tabIndex={12}
        isChecked={
          prefix === "idp"
            ? idp_verifyLogoutResponsesSign
            : sp_signLogoutResponses
        }
      />

      {prefix === "sp" && (
        <SimpleCheckbox
          label={t(`${prefix}DecryptStatements`)}
          name={checkboxesNames[prefix][3]}
          tabIndex={13}
          isChecked={sp_encryptAssertions}
        />
      )}
    </Box>
  );
};

export default inject(({ ssoStore }) => {
  const {
    idp_verifyAuthResponsesSign,
    idp_verifyLogoutRequestsSign,
    idp_verifyLogoutResponsesSign,
    sp_signAuthRequests,
    sp_signLogoutRequests,
    sp_signLogoutResponses,
    sp_encryptAssertions,
  } = ssoStore;

  return {
    idp_verifyAuthResponsesSign,
    idp_verifyLogoutRequestsSign,
    idp_verifyLogoutResponsesSign,
    sp_signAuthRequests,
    sp_signLogoutRequests,
    sp_signLogoutResponses,
    sp_encryptAssertions,
  };
})(observer(CheckboxSet));
