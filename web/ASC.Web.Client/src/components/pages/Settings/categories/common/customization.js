import React from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import withCultureNames from "@appserver/common/hoc/withCultureNames";
import CustomizationNavbar from "./customization-navbar";
import LanguageAndTimeZone from "./language-and-time-zone";
import CustomTitles from "./custom-titles";
import PortalRenaming from "./portal-renaming";
import { Base } from "@appserver/components/themes";
import { Consumer } from "@appserver/components/utils/context";
import { isMobile } from "react-device-detect";

const StyledComponent = styled.div`
  .margin-top {
    margin-top: 20px;
  }

  .margin-left {
    margin-left: 20px;
  }

  .settings-block {
    margin-bottom: 24px;
  }

  .field-container-width {
    max-width: 500px;
  }

  .combo-button-label {
    max-width: 100%;
  }

  .category-description {
    color: #657077;
  }

  .category-border {
    border-bottom: 1px solid #eceef1;
  }

  .category-item-wrapper {
    /* .category-item-heading {
      display: flex;
      align-items: center;
      margin-bottom: 16px;
    } */

    .category-item-subheader {
      font-size: 13px;
      font-weight: 600;
      margin-bottom: 5px;
    }

    .category-item-description {
      color: ${(props) => props.theme.studio.settings.common.descriptionColor};
      font-size: 12px;
      max-width: 1024px;
    }

    /* .inherit-title-link {
      margin-right: 7px;
      font-size: 19px;
      font-weight: 600;
    } */

    .link-text {
      margin: 0;
    }

    /* .category-item-title {
      font-weight: bold;
      font-size: 16px;
      line-height: 22px;
      margin-right: 4px;
    } */
  }
`;

StyledComponent.defaultProps = { theme: Base };

const Customization = ({ t }) => {
  return (
    <Consumer>
      {(context) =>
        `${context.sectionWidth}` <= 375 || isMobile ? (
          <CustomizationNavbar />
        ) : (
          <StyledComponent>
            <div className="category-description">{`${t(
              "Settings:CustomizationDescription"
            )}`}</div>
            <div className="category-item-wrapper">
              <LanguageAndTimeZone />
            </div>
            <div className="category-border"></div>
            <div className="category-item-wrapper">
              <CustomTitles sectionWidth={context.sectionWidth} />
            </div>
            <div className="category-border"></div>
            <div className="category-item-wrapper">
              <PortalRenaming sectionWidth={context.sectionWidth} />
            </div>
          </StyledComponent>
        )
      }
    </Consumer>
  );
};

export default withCultureNames(
  withTranslation(["Settings", "Common"])(Customization)
);
