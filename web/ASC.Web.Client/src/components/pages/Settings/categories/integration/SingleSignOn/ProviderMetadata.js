import React from "react";
import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import HideButton from "./sub-components/HideButton";
import SimpleFormField from "./sub-components/SimpleFormField";
import { observer } from "mobx-react";

const ProviderMetadata = ({ FormStore, t }) => {
  return (
    <Box>
      <HideButton FormStore={FormStore} label="SPMetadata" t={t} />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("SPEntityId")}
        name="sp_entityId"
        placeholder="https://www.test.com"
        tabIndex={25}
        tooltipContent={t("SPEntityIdTooltip")}
      />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("SPAssertionConsumerURL")}
        name="sp_assertionConsumerUrl"
        placeholder="https://www.test.com"
        tabIndex={26}
        tooltipContent={t("SPAssertionConsumerURLTooltip")}
      />

      <SimpleFormField
        FormStore={FormStore}
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
    </Box>
  );
};

export default observer(ProviderMetadata);
