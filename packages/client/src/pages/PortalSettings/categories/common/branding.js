import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";
import { isDesktop } from "react-device-detect";

import withLoading from "SRC_DIR/HOCs/withLoading";
import Whitelabel from "./settingsBranding/whitelabel";
import CompanyInfoSettings from "./settingsBranding/companyInfoSettings";
import styled from "styled-components";
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
`;

const Branding = (props) => {
  const [isPortalPaid, setIsPortalPaid] = useState(true);

  if (!isDesktop) return <ForbiddenPage />;

  return (
    <StyledComponent>
      <Whitelabel isPortalPaid={isPortalPaid} />
      <hr />
      <div className="section-description">
        Specify your company information, add links to external resources, and
        email addresses displayed within the online office interface.
      </div>
      <CompanyInfoSettings isPortalPaid={isPortalPaid} />
      <AdditionalResources isPortalPaid={isPortalPaid} />
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {})(
  withLoading(withTranslation(["Settings", "Common"])(observer(Branding)))
);
