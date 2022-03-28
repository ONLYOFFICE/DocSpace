import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import withCultureNames from "@appserver/common/hoc/withCultureNames";
import CustomizationNavbar from "./customization-navbar";
import LanguageAndTimeZone from "./language-and-time-zone";
import CustomTitles from "./custom-titles";
import PortalRenaming from "./portal-renaming";
import { Base } from "@appserver/components/themes";
import { isSmallTablet } from "@appserver/components/utils/device";

import commonSettingsStyles from "../../utils/commonSettingsStyles";
const StyledComponent = styled.div`
  .combo-button-label {
    max-width: 100%;
  }

  .settings-block {
    margin-bottom: 24px;
  }

  .category-description {
    line-height: 20px;
    color: #657077;
    margin-bottom: 20px;
  }

  .category-item-wrapper:not(:last-child) {
    border-bottom: 1px solid #eceef1;
    margin-bottom: 24px;
  }

  .category-item-description {
    color: ${(props) => props.theme.studio.settings.common.descriptionColor};
    font-size: 12px;
    max-width: 1024px;
  }

  .category-item-heading {
    display: flex;
    align-items: center;
    padding-bottom: 16px;
  }

  .category-item-title {
    font-weight: bold;
    font-size: 16px;
    line-height: 22px;
    margin-right: 4px;
  }

  @media (min-width: 600px) {
    .settings-block {
      max-width: 350px;
      height: auto;
    }
  }
`;

StyledComponent.defaultProps = { theme: Base };

const Customization = ({ t }) => {
  const [mobileView, setMobileView] = useState();

  const checkInnerWidth = () => {
    if (window.innerWidth < 600) {
      setMobileView(true);
    } else {
      setMobileView(false);
    }
  };

  useEffect(() => {
    window.addEventListener("resize", checkInnerWidth);
    return () => window.removeEventListener("resize", checkInnerWidth);
  }, [checkInnerWidth]);

  return isSmallTablet() || mobileView ? (
    <CustomizationNavbar />
  ) : (
    <StyledComponent>
      <div className="category-description">{`${t(
        "Settings:CustomizationDescription"
      )}`}</div>
      <div className="category-item-wrapper">
        <LanguageAndTimeZone />
      </div>
      {/* <div className="category-item-wrapper">
              <CustomTitles />
            </div>
            <div className="category-item-wrapper">
              <PortalRenaming />
            </div> */}
    </StyledComponent>
  );
};

export default withCultureNames(
  withTranslation(["Settings", "Common"])(Customization)
);
