import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
import Button from "@docspace/components/button";
import HelpButton from "@docspace/components/help-button";
import Text from "@docspace/components/text";

import AddIdpCertificateModal from "./sub-components/AddIdpCertificateModal";
import AddSpCertificateModal from "./sub-components/AddSpCertificateModal";
import CertificatesTable from "./sub-components/CertificatesTable";
import CheckboxSet from "./sub-components/CheckboxSet";
import HideButton from "./sub-components/HideButton";
import PropTypes from "prop-types";
import SsoComboBox from "./sub-components/SsoComboBox";
import {
  decryptAlgorithmsOptions,
  verifyAlgorithmsOptions,
} from "./sub-components/constants";

const Certificates = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    provider,
    enableSso,
    openIdpModal,
    openSpModal,
    idpCertificates,
    spCertificates,
    idpShowAdditionalParameters,
    spShowAdditionalParameters,
    idpVerifyAlgorithm,
    spEncryptAlgorithm,
    spDecryptAlgorithm,
    isLoadingXml,
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
        marginProp="40px 0 12px 0"
      >
        <Text as="h2" fontSize="15px" fontWeight={600} noSelect>
          {prefix === "idp" ? t("idpCertificates") : t("spCertificates")}
        </Text>

        <HelpButton
          offsetRight={0}
          tooltipContent={
            prefix === "idp" ? (
              <Text fontSize="12px">{t("idpCertificatesTooltip")}</Text>
            ) : (
              <Text fontSize="12px">{t("spCertificatesTooltip")}</Text>
            )
          }
        />
      </Box>

      {certificates.length > 0 && <CertificatesTable prefix={prefix} />}

      <Box alignItems="center" displayProp="flex" flexDirection="row">
        {prefix === "idp" && (
          <>
            <Button
              isDisabled={!enableSso || isLoadingXml}
              label={t("AddCertificate")}
              onClick={openIdpModal}
              size="small"
              tabIndex={9}
            />
            <AddIdpCertificateModal />
          </>
        )}

        {prefix === "sp" && (
          <>
            <Button
              isDisabled={!enableSso || isLoadingXml}
              label={t("AddCertificate")}
              onClick={openSpModal}
              size="small"
              tabIndex={9}
            />
            <AddSpCertificateModal />
          </>
        )}

        <HideButton
          value={additionalParameters}
          label={
            prefix === "idp"
              ? "idpShowAdditionalParameters"
              : "spShowAdditionalParameters"
          }
          isAdditionalParameters
        />
      </Box>

      {additionalParameters && (
        <>
          <CheckboxSet prefix={prefix} />

          {provider === "IdentityProvider" && (
            <>
              <SsoComboBox
                isDisabled={idpCertificates.length === 0}
                labelText={t("idpSigningAlgorithm")}
                name="idpVerifyAlgorithm"
                options={verifyAlgorithmsOptions}
                tabIndex={14}
                value={idpVerifyAlgorithm}
              />
            </>
          )}

          {provider === "ServiceProvider" && (
            <>
              <SsoComboBox
                isDisabled={spCertificates.length === 0}
                labelText={t("spSigningAlgorithm")}
                name="spSigningAlgorithm"
                options={verifyAlgorithmsOptions}
                tabIndex={14}
                value={spEncryptAlgorithm}
              />

              <SsoComboBox
                isDisabled={spCertificates.length === 0}
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
    openIdpModal,
    openSpModal,
    idpCertificates,
    spCertificates,
    idpShowAdditionalParameters,
    spShowAdditionalParameters,
    idpVerifyAlgorithm,
    spEncryptAlgorithm,
    spDecryptAlgorithm,
    isLoadingXml,
  } = ssoStore;

  return {
    enableSso,
    openIdpModal,
    openSpModal,
    idpCertificates,
    spCertificates,
    idpShowAdditionalParameters,
    spShowAdditionalParameters,
    idpVerifyAlgorithm,
    spEncryptAlgorithm,
    spDecryptAlgorithm,
    isLoadingXml,
  };
})(observer(Certificates));
