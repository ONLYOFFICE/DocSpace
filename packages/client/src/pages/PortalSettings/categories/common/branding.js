import React, { useEffect } from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";

import withLoading from "SRC_DIR/HOCs/withLoading";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import Whitelabel from "./Branding/whitelabel";
import CompanyInfoSettings from "./Branding/companyInfoSettings";
import styled from "styled-components";
import AdditionalResources from "./Branding/additionalResources";

import LoaderBrandingDescription from "./sub-components/loaderBrandingDescription";

import MobileView from "./Branding/MobileView";

import { UnavailableStyles } from "../../utils/commonSettingsStyles";
import { resetSessionStorage } from "../../utils";
import { useIsMobileView } from "../../utils/useIsMobileView";

const StyledComponent = styled.div`
  max-width: 700px;
  width: 100%;
  font-weight: 400;
  font-size: 13px;

  .header {
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;
    padding-bottom: 9px;
  }

  .description {
    padding-bottom: 16px;
  }

  .settings-block {
    max-width: 433px;
  }

  .section-description {
    color: ${(props) =>
      props.theme.client.settings.common.brandingDescriptionColor};
    line-height: 20px;
    padding-bottom: 20px;
  }

  hr {
    margin: 24px 0;
    border: none;
    border-top: ${(props) => props.theme.client.settings.separatorBorder};
  }

  ${(props) => !props.isSettingPaid && UnavailableStyles}
`;

const Branding = ({
  t,
  isLoadedCompanyInfoSettingsData,
  isSettingPaid,
  standalone,
}) => {
  const isMobileView = useIsMobileView();

  useEffect(() => {
    setDocumentTitle(t("Branding"));
  }, []);

  useEffect(() => {
    return () => {
      if (!window.location.pathname.includes("customization")) {
        resetSessionStorage();
      }
    };
  }, []);

  if (isMobileView) return <MobileView />;

  return (
    <StyledComponent isSettingPaid={isSettingPaid}>
      <Whitelabel />
      {standalone && (
        <>
          <hr />
          {isLoadedCompanyInfoSettingsData ? (
            <div className="section-description settings_unavailable">
              {t("Settings:BrandingSectionDescription")}
            </div>
          ) : (
            <LoaderBrandingDescription />
          )}
          <CompanyInfoSettings />
          <AdditionalResources />
        </>
      )}
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {
  const { currentQuotaStore, settingsStore } = auth;
  const { isBrandingAndCustomizationAvailable } = currentQuotaStore;
  const { isLoadedCompanyInfoSettingsData } = common;
  const { standalone } = settingsStore;

  return {
    isLoadedCompanyInfoSettingsData,
    isSettingPaid: isBrandingAndCustomizationAvailable,
    standalone,
  };
})(withLoading(withTranslation(["Settings", "Common"])(observer(Branding))));
