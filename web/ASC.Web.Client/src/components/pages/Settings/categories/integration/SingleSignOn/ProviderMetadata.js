import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";

import SimpleFormField from "./sub-components/SimpleFormField";

const ProviderMetadata = () => {
  const { t } = useTranslation("SingleSignOn");

  return (
    <>
      <SimpleFormField
        labelText={t("SPEntityId")}
        name="sp_entityId"
        placeholder="https://www.test.com"
        tabIndex={25}
        tooltipContent={t("SPEntityIdTooltip")}
      />

      <SimpleFormField
        labelText={t("SPAssertionConsumerURL")}
        name="sp_assertionConsumerUrl"
        placeholder="https://www.test.com"
        tabIndex={26}
        tooltipContent={t("SPAssertionConsumerURLTooltip")}
      />

      <SimpleFormField
        labelText={t("SPSingleLogoutURL")}
        name="sp_singleLogoutUrl"
        placeholder="https://www.test.com"
        tabIndex={27}
        tooltipContent={t("SPSingleLogoutURLTooltip")}
      />

      <Box>
        <Button
          className="download-button"
          label={t("DownloadMetadataXML")}
          primary
          size="medium"
          tabIndex={28}
        />
      </Box>
    </>
  );
};

export default observer(ProviderMetadata);
