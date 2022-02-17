import React from "react";

import Box from "@appserver/components/box";
import SimpleCheckbox from "./SimpleCheckbox";
import { observer } from "mobx-react";

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

const CheckboxSet = ({ FormStore, prefix, t }) => {
  return (
    <Box marginProp="12px 0">
      <SimpleCheckbox
        FormStore={FormStore}
        label={t(`${prefix}AuthRequest`)}
        name={checkboxesNames[prefix][0]}
        tabIndex={10}
      />
      <SimpleCheckbox
        FormStore={FormStore}
        label={t(`${prefix}SignExitRequest`)}
        name={checkboxesNames[prefix][1]}
        tabIndex={11}
      />
      <SimpleCheckbox
        FormStore={FormStore}
        label={t(`${prefix}SignResponseRequest`)}
        name={checkboxesNames[prefix][2]}
        tabIndex={12}
      />

      {prefix === "sp" && (
        <SimpleCheckbox
          FormStore={FormStore}
          label={t(`${prefix}DecryptStatements`)}
          name={checkboxesNames[prefix][3]}
          tabIndex={13}
        />
      )}
    </Box>
  );
};

export default observer(CheckboxSet);
