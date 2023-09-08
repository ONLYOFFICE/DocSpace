import { useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

import {
  smallTablet,
  hugeMobile,
  size,
} from "@docspace/components/utils/device";

import MetadataUrlField from "./sub-components/MetadataUrlField";
import { useIsMobileView } from "../../../utils/useIsMobileView";

const StyledWrapper = styled.div`
  .button-wrapper {
    margin-top: 24px;
  }

  @media ${smallTablet} {
    .button-wrapper {
      box-sizing: border-box;
      position: absolute;
      bottom: 0;
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              right: 0;
            `
          : css`
              left: 0;
            `}
      width: 100%;
      padding: 16px 16px 16px 24px;
    }
  }

  @media ${hugeMobile}{
    .button-wrapper {
      padding: 16px;
    }
  }
}
`;

const ProviderMetadata = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const isMobileView = useIsMobileView();
  const navigate = useNavigate();
  const location = useLocation();

  const { downloadMetadata } = props;

  const url = window.location.origin;

  useEffect(() => {
    checkWidth();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      location.pathname.includes("sp-metadata") &&
      navigate("/portal-settings/integration/single-sign-on");
  };

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
