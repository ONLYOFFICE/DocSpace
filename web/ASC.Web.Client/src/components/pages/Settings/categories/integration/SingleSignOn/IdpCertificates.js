import React from "react";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import HelpButton from "@appserver/components/help-button";
import Text from "@appserver/components/text";

import AddCertificateModal from "./sub-components/AddCertificateModal";
import HideButton from "./sub-components/HideButton";
import SimpleCheckbox from "./sub-components/SimpleCheckbox";
import SimpleComboBox from "./sub-components/SimpleComboBox";
import {
  decryptAlgorithmsOptions,
  verifyAlgorithmsOptions,
} from "./sub-components/constants";

const IdpCertificates = ({ FormStore, t }) => {
  return (
    <Box>
      <Box
        alignItems="center"
        displayProp="flex"
        flexDirection="row"
        marginProp="24px 0"
      >
        <Text as="h2" fontSize="14px" fontWeight={600}>
          {t("OpenCertificates")}
        </Text>

        <HelpButton
          offsetRight={0}
          tooltipContent={t("OpenCertificatesTooltip")}
        />
      </Box>

      <Box alignItems="center" displayProp="flex" flexDirection="row">
        <Button
          onClick={FormStore.onOpenModal}
          label={t("AddCertificate")}
          size="medium"
          tabIndex={9}
        />

        <AddCertificateModal FormStore={FormStore} t={t} />

        <HideButton
          FormStore={FormStore}
          label="ShowAdditionalParameters"
          isAdditionalParameters
          t={t}
        />
      </Box>

      <Box marginProp="12px 0">
        <SimpleCheckbox
          FormStore={FormStore}
          label={t("SignAuthRequest")}
          name="idp_verifyAuthResponsesSign"
          tabIndex={10}
        />
        <SimpleCheckbox
          FormStore={FormStore}
          label={t("SignExitRequest")}
          name="idp_verifyLogoutRequestsSign"
          tabIndex={11}
        />
        <SimpleCheckbox
          FormStore={FormStore}
          label={t("SignResponseRequest")}
          name="idp_verifyLogoutResponsesSign"
          tabIndex={12}
        />
        <SimpleCheckbox
          FormStore={FormStore}
          label={t("DecryptStatements")}
          name="ipd_decryptAssertions"
          tabIndex={13}
        />
      </Box>

      <SimpleComboBox
        FormStore={FormStore}
        labelText={t("SigningAlgorithm")}
        name="idp_verifyAlgorithm"
        options={verifyAlgorithmsOptions}
        tabIndex={14}
      />

      <SimpleComboBox
        FormStore={FormStore}
        labelText={t("StandardDecryptionAlgorithm")}
        name="idp_decryptAlgorithm"
        options={decryptAlgorithmsOptions}
        tabIndex={15}
      />
    </Box>
  );
};

export default observer(IdpCertificates);
