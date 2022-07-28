import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
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

const Certificates = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    provider,
    enableSso,
    onOpenIdpModal,
    onOpenSpModal,
    idpCertificates,
    spCertificates,
    idpShowAdditionalParameters,
    spShowAdditionalParameters,
    idpVerifyAlgorithm,
    spEncryptAlgorithm,
    spDecryptAlgorithm,
    onLoadXML,
  } = props;

  let prefix = "";
  let additionalParameters = false;
  let certificates = [];

  switch (provider) {
    case "IdentityProvider":
      prefix = "idp";
      additionalParameters = idpShowAdditionalParameters;
      certificates = idpCertificates;
      break;
    case "ServiceProvider":
      prefix = "sp";
      additionalParameters = spShowAdditionalParameters;
      certificates = spCertificates;
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
        <Text as="h2" fontSize="14px" fontWeight={600} noSelect>
          {t(`${prefix}Certificates`)}
        </Text>

        <HelpButton
          offsetRight={0}
          tooltipContent={t(`${prefix}CertificatesTooltip`)}
        />
      </Box>

      {certificates.length > 0 && <CertificatesTable prefix={prefix} />}

      <Box alignItems="center" displayProp="flex" flexDirection="row">
        {prefix === "idp" && (
          <>
            <Button
              isDisabled={!enableSso || onLoadXML}
              label={t("AddCertificate")}
              onClick={onOpenIdpModal}
              size="small"
              tabIndex={9}
            />
            <AddIdpCertificateModal />
          </>
        )}

        {prefix === "sp" && (
          <>
            <Button
              isDisabled={!enableSso || onLoadXML}
              label={t("AddCertificate")}
              onClick={onOpenSpModal}
              size="small"
              tabIndex={9}
            />
            <AddSpCertificateModal />
          </>
        )}

        <HideButton
          value={additionalParameters}
          label={`${prefix}ShowAdditionalParameters`}
          isAdditionalParameters
        />
      </Box>

      {additionalParameters && (
        <>
          <CheckboxSet id={prefix} prefix={prefix} />

          {provider === "IdentityProvider" && (
            <>
              <SimpleComboBox
                labelText={t(`${prefix}SigningAlgorithm`)}
                name={"idpVerifyAlgorithm"}
                options={verifyAlgorithmsOptions}
                tabIndex={14}
                value={idpVerifyAlgorithm}
              />
            </>
          )}

          {provider === "ServiceProvider" && (
            <>
              <SimpleComboBox
                labelText={t(`${prefix}SigningAlgorithm`)}
                name={"spSigningAlgorithm"}
                options={verifyAlgorithmsOptions}
                tabIndex={14}
                value={spEncryptAlgorithm}
              />

              <SimpleComboBox
                labelText={t("StandardDecryptionAlgorithm")}
                name={"spEncryptAlgorithm"}
                options={decryptAlgorithmsOptions}
                tabIndex={15}
                value={spDecryptAlgorithm}
              />
            </>
          )}
        </>
      )}
    </Box>
  );
};

Certificates.propTypes = {
  provider: PropTypes.oneOf(["IdentityProvider", "ServiceProvider"]),
};

export default inject(({ ssoStore }) => {
  const {
    enableSso,
    onOpenIdpModal,
    onOpenSpModal,
    idpCertificates,
    spCertificates,
    idpShowAdditionalParameters,
    spShowAdditionalParameters,
    idpVerifyAlgorithm,
    spEncryptAlgorithm,
    spDecryptAlgorithm,
    onLoadXML,
  } = ssoStore;

  return {
    enableSso,
    onOpenIdpModal,
    onOpenSpModal,
    idpCertificates,
    spCertificates,
    idpShowAdditionalParameters,
    spShowAdditionalParameters,
    idpVerifyAlgorithm,
    spEncryptAlgorithm,
    spDecryptAlgorithm,
    onLoadXML,
  };
})(observer(Certificates));
