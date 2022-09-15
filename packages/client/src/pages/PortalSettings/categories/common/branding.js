import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";
import { isDesktop } from "react-device-detect";

import withLoading from "SRC_DIR/HOCs/withLoading";
import Whitelabel from "./Branding/whitelabel";
import CompanyInfoSettings from "./Branding/companyInfoSettings";
import styled from "styled-components";
import AdditionalResources from "./Branding/additionalResources";

import LoaderBrandingDescription from "./sub-components/loaderBrandingDescription";

import BreakpointWarning from "../../../../components/BreakpointWarning/index";

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
    color: #657077;
    line-height: 20px;
    padding-bottom: 20px;
  }

  hr {
    margin: 24px 0;
    border: none;
    border-top: 1px solid #eceef1;
  }
`;

const Branding = ({ t, isLoadedCompanyInfoSettingsData }) => {
  const [isPortalPaid, setIsPortalPaid] = useState(true);
  const [viewDesktop, setViewDesktop] = useState(false);

  useEffect(() => {
    onCheckView();
    window.addEventListener("resize", onCheckView);

    return () => window.removeEventListener("resize", onCheckView);
  }, []);

  const onCheckView = () => {
    if (isDesktop && window.innerWidth > 1024) {
      setViewDesktop(true);
    } else {
      setViewDesktop(false);
    }
  };

  if (!viewDesktop)
    return <BreakpointWarning sectionName={t("Settings:Branding")} />;

  return (
    <StyledComponent>
      <Whitelabel isPortalPaid={isPortalPaid} />
      <hr />
      {isLoadedCompanyInfoSettingsData ? (
        <div className="section-description">
          {t("Settings:BrandingSectionDescription")}
        </div>
      ) : (
        <LoaderBrandingDescription />
      )}
      <CompanyInfoSettings isPortalPaid={isPortalPaid} />
      <AdditionalResources isPortalPaid={isPortalPaid} />
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {
  const { isLoadedCompanyInfoSettingsData } = common;

  return {
    isLoadedCompanyInfoSettingsData,
  };
})(withLoading(withTranslation(["Settings", "Common"])(observer(Branding))));
