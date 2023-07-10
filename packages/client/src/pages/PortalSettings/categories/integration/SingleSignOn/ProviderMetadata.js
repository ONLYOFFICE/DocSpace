import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

import MetadataUrlField from "./sub-components/MetadataUrlField";

const ProviderMetadata = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const { downloadMetadata } = props;

  const url = window.location.origin;

  return (
    <>
      <MetadataUrlField
        labelText={t("SPEntityId")}
        name="spEntityId"
        placeholder={`${url}/sso/metadata`}
        tooltipContent={<Text fontSize="12px">{t("SPEntityIdTooltip")}</Text>}
      />

      <MetadataUrlField
        labelText={t("SPAssertionConsumerURL")}
        name="spAssertionConsumerUrl"
        placeholder={`${url}/sso/acs`}
        tooltipContent={
          <Text fontSize="12px">{t("SPAssertionConsumerURLTooltip")}</Text>
        }
      />

      <MetadataUrlField
        labelText={t("SPSingleLogoutURL")}
        name="spSingleLogoutUrl"
        placeholder={`${url}/sso/slo/callback`}
        tooltipContent={
          <Text fontSize="12px">{t("SPSingleLogoutURLTooltip")}</Text>
        }
      />

      <Box marginProp="24px 0">
        <Button
          label={t("DownloadMetadataXML")}
          primary
          size="small"
          tabIndex={25}
          onClick={downloadMetadata}
        />
      </Box>
    </>
  );
};

export default inject(({ ssoStore }) => {
  const { downloadMetadata } = ssoStore;

  return {
    downloadMetadata,
  };
})(observer(ProviderMetadata));
