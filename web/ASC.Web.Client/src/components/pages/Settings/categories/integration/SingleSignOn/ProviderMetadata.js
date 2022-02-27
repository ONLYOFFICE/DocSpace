import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";

import MetadataUrlField from "./sub-components/MetadataUrlField";

const ProviderMetadata = () => {
  const { t } = useTranslation("SingleSignOn");

  return (
    <>
      <MetadataUrlField
        labelText={t("SPEntityId")}
        name="sp_entityId"
        placeholder="https://www.test.com"
        tooltipContent={t("SPEntityIdTooltip")}
      />

      <MetadataUrlField
        labelText={t("SPAssertionConsumerURL")}
        name="sp_assertionConsumerUrl"
        placeholder="https://www.test.com"
        tooltipContent={t("SPAssertionConsumerURLTooltip")}
      />

      <MetadataUrlField
        labelText={t("SPSingleLogoutURL")}
        name="sp_singleLogoutUrl"
        placeholder="https://www.test.com"
        tooltipContent={t("SPSingleLogoutURLTooltip")}
      />

      <Box>
        <Button
          className="download-button"
          label={t("DownloadMetadataXML")}
          primary
          size="medium"
          tabIndex={25}
        />
      </Box>
    </>
  );
};

export default observer(ProviderMetadata);
