import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";
import { isMobile, isDesktop } from "react-device-detect";

import withLoading from "SRC_DIR/HOCs/withLoading";
import Whitelabel from "./Branding/whitelabel";
import CompanyInfoSettings from "./Branding/companyInfoSettings";
import styled from "styled-components";
import AdditionalResources from "./Branding/additionalResources";

import LoaderBrandingDescription from "./sub-components/loaderBrandingDescription";

import BreakpointWarning from "../../../../components/BreakpointWarning/index";

import { UnavailableStyles } from "../../utils/commonSettingsStyles";
import { resetSessionStorage } from "../../utils";

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
  // const [viewDesktop, setViewDesktop] = useState(false);

  useEffect(() => {
    return () => {
      if (!window.location.pathname.includes("customization")) {
        resetSessionStorage();
      }
    };
  }, []);

  // useEffect(() => {
  //   onCheckView();
  //   window.addEventListener("resize", onCheckView);

  //   return () => window.removeEventListener("resize", onCheckView);
  // }, []);

  // const onCheckView = () => {
  //   if (!isMobile && window.innerWidth > 1024) {
  //     setViewDesktop(true);
  //   } else {
  //     setViewDesktop(false);
  //   }
  // };

  if (isMobile)
    return <BreakpointWarning sectionName={t("Settings:Branding")} />;

  return (
    <StyledComponent isSettingPaid={isSettingPaid}>
      <Whitelabel isSettingPaid={isSettingPaid} />
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
          <CompanyInfoSettings isSettingPaid={isSettingPaid} />
          <AdditionalResources isSettingPaid={isSettingPaid} />
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
