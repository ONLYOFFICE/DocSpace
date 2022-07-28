import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";

import withLoading from "SRC_DIR/HOCs/withLoading";
import Whitelabel from "./settingsBranding/whitelabel";
import CompanyInfoSettings from "./settingsBranding/companyInfoSettings";
import styled from "styled-components";
import AdditionalResources from "./settingsBranding/additionalResources";

const StyledComponent = styled.div`
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
    padding-bottom: 32px;
  }

  .save-cancel-buttons {
    padding-bottom: 24px;
  }

  .section-description {
    color: #657077;
    line-height: 20px;
    padding-bottom: 20px;
  }
`;

const Branding = (props) => {
  return (
    <StyledComponent>
      <Whitelabel />
      <div className="section-description">
        Specify your company information, add links to external resources, and
        email addresses displayed within the online office interface.
      </div>
      <CompanyInfoSettings />
      <AdditionalResources />
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {})(
  withLoading(withTranslation(["Settings", "Common"])(observer(Branding)))
);
