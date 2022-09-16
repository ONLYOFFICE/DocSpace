import React from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";
import { isDesktop } from "react-device-detect";

import withLoading from "SRC_DIR/HOCs/withLoading";
import Whitelabel from "./settingsBranding/whitelabel";
import CompanyInfoSettings from "./settingsBranding/companyInfoSettings";
import styled, { css } from "styled-components";
import AdditionalResources from "./settingsBranding/additionalResources";

import ForbiddenPage from "../ForbiddenPage";

const StyledComponent = styled.div`
  max-width: 700px;
  width: 100%;
  font-weight: 400;
  font-size: 13px;

  .header {
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;
    padding-bottom: 16px;
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

  .branding_unavailable {
     {
      ${(props) =>
        !props.isSettingPaid &&
        css`
          color: ${(props) => props.theme.text.disableColor};
          pointer-events: none;
          cursor: default;

          label {
            color: ${(props) => props.theme.text.disableColor};
          }
        `}
    }
  }
`;

const Branding = ({ isSettingPaid }) => {
  if (!isDesktop) return <ForbiddenPage />;

  return (
    <StyledComponent isSettingPaid={isSettingPaid}>
      <Whitelabel isSettingPaid={isSettingPaid} />
      <hr />
      <div className="section-description branding_unavailable">
        Specify your company information, add links to external resources, and
        email addresses displayed within the online office interface.
      </div>
      <CompanyInfoSettings isSettingPaid={isSettingPaid} />
      <AdditionalResources isSettingPaid={isSettingPaid} />
    </StyledComponent>
  );
};

export default inject(({ auth }) => {
  const { currentQuotaStore } = auth;
  const { isBrandingAndCustomizationAvailable } = currentQuotaStore;

  const isSettingPaid = isBrandingAndCustomizationAvailable;
  return {
    isSettingPaid,
  };
})(withLoading(withTranslation(["Settings", "Common"])(observer(Branding))));
