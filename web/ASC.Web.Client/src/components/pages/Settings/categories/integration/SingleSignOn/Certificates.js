import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import FormStore from "@appserver/studio/src/store/SsoFormStore";
import HelpButton from "@appserver/components/help-button";
import Text from "@appserver/components/text";

import AddIdpCertificateModal from "./sub-components/AddIdpCertificateModal";
import AddSpCertificateModal from "./sub-components/AddSpCertificateModal";
import CertificatesTable from "./sub-components/CertificatesTable";
import CheckboxSet from "./sub-components/CheckboxSet";
import HideButton from "./sub-components/HideButton";
import PropTypes from "prop-types";
import SimpleComboBox from "./sub-components/SimpleComboBox";
import {
  decryptAlgorithmsOptions,
  verifyAlgorithmsOptions,
} from "./sub-components/constants";

const Certificates = ({ provider }) => {
  const { t } = useTranslation("SingleSignOn");

  let prefix = "";

  switch (provider) {
    case "IdentityProvider":
      prefix = "idp";
      break;
    case "ServiceProvider":
      prefix = "sp";
      break;
  }

  return (
    <Box>
      <Box
        alignItems="center"
        displayProp="flex"
        flexDirection="row"
        marginProp="24px 0"
      >
        <Text as="h2" fontSize="14px" fontWeight={600}>
          {t(`${prefix}Certificates`)}
        </Text>

        <HelpButton
          offsetRight={0}
          tooltipContent={t(`${prefix}CertificatesTooltip`)}
        />
      </Box>

      {FormStore[`${prefix}_certificates`].length > 0 && (
        <CertificatesTable prefix={prefix} />
      )}

      <Box alignItems="center" displayProp="flex" flexDirection="row">
        {prefix === "idp" && (
          <>
            <Button
              onClick={FormStore.onOpenIdpModal}
              label={t("AddCertificate")}
              size="medium"
              tabIndex={9}
            />
            <AddIdpCertificateModal />
          </>
        )}

        {prefix === "sp" && (
          <>
            <Button
              onClick={FormStore.onOpenSpModal}
              label={t("AddCertificate")}
              size="medium"
              tabIndex={9}
            />
            <AddSpCertificateModal />
          </>
        )}

        <HideButton
          label={`${prefix}_showAdditionalParameters`}
          isAdditionalParameters
        />
      </Box>

      {FormStore[`${prefix}_showAdditionalParameters`] && (
        <>
          <CheckboxSet id={prefix} prefix={prefix} />

          {provider === "IdentityProvider" && (
            <>
              <SimpleComboBox
                labelText={t(`${prefix}SigningAlgorithm`)}
                name={"idp_verifyAlgorithm"}
                options={verifyAlgorithmsOptions}
                tabIndex={14}
              />
            </>
          )}

          {provider === "ServiceProvider" && (
            <>
              <SimpleComboBox
                labelText={t(`${prefix}SigningAlgorithm`)}
                name={"sp_signingAlgorithm"}
                options={verifyAlgorithmsOptions}
                tabIndex={14}
              />

              <SimpleComboBox
                labelText={t("StandardDecryptionAlgorithm")}
                name={"sp_encryptAlgorithm"}
                options={decryptAlgorithmsOptions}
                tabIndex={15}
              />
            </>
          )}
        </>
      )}
    </Box>
  );
};

export default observer(Certificates);

Certificates.PropTypes = {
  provider: PropTypes.oneOf(["IdentityProvider", "ServiceProvider"]),
};
