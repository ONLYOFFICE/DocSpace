import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

import { smallTablet } from "@docspace/components/utils/device";

import MetadataUrlField from "./sub-components/MetadataUrlField";
import { useIsMobileView } from "../../../utils/useIsMobileView";

const StyledWrapper = styled.div`
  .button-wrapper {
    margin: 24px 0;
  }

  @media ${smallTablet} {
    .button-wrapper {
      position: fixed;
      bottom: 4px;
      width: calc(100% - 32px);
    }
  }
`;

const ProviderMetadata = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const isMobileView = useIsMobileView();
  const { downloadMetadata } = props;

  const url = window.location.origin;

  return (
    <StyledWrapper>
      <MetadataUrlField
        labelText={t("SPEntityId")}
        name="spEntityId"
        placeholder={`${url}/sso/metadata`}
        tooltipContent={<Text fontSize="12px">{t("SPEntityIdTooltip")}</Text>}
        tooltipClass="sp-entity-id-tooltip icon-button"
      />

      <MetadataUrlField
        labelText={t("SPAssertionConsumerURL")}
        name="spAssertionConsumerUrl"
        placeholder={`${url}/sso/acs`}
        tooltipContent={
          <Text fontSize="12px">{t("SPAssertionConsumerURLTooltip")}</Text>
        }
        tooltipClass="sp-assertion-consumer-url-tooltip icon-button"
      />

      <MetadataUrlField
        labelText={t("SPSingleLogoutURL")}
        name="spSingleLogoutUrl"
        placeholder={`${url}/sso/slo/callback`}
        tooltipContent={
          <Text fontSize="12px">{t("SPSingleLogoutURLTooltip")}</Text>
        }
        tooltipClass="sp-single-logout-url-tooltip icon-button"
      />

      <div className="button-wrapper">
        <Button
          id="download-metadata-xml"
          label={t("DownloadMetadataXML")}
          primary
          scale={isMobileView}
          size={isMobileView ? "normal" : "small"}
          tabIndex={25}
          onClick={downloadMetadata}
        />
      </div>
    </StyledWrapper>
  );
};

export default inject(({ ssoStore }) => {
  const { downloadMetadata } = ssoStore;

  return {
    downloadMetadata,
  };
})(observer(ProviderMetadata));
