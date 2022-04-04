import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { Base } from "@appserver/components/themes";
import { isSmallTablet } from "@appserver/components/utils/device";

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

  /* .category-item-wrapper:not(:last-child) {
    border-bottom: 1px solid #eceef1;
    margin-bottom: 24px;
  } */

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

const SettingsPageLayout = ({ children }) => {
  const [mobileView, setMobileView] = useState(true);

  const checkInnerWidth = () => {
    if (isSmallTablet()) {
      setMobileView(true);
    } else {
      setMobileView(false);
    }
  };

  useEffect(() => {
    window.addEventListener("resize", checkInnerWidth);
    return () => window.removeEventListener("resize", checkInnerWidth);
  }, []);

  const isMobile = !!(isSmallTablet() && mobileView);

  console.log("isMobile", isMobile);
  return <>{children(isMobile)}</>;
};

export default SettingsPageLayout;
