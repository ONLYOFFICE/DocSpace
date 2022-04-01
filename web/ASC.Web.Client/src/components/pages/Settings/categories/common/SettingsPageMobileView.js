import React from "react";
import styled from "styled-components";
import { Base } from "@appserver/components/themes";

const StyledComponent = styled.div`
  .combo-button-label {
    max-width: 100%;
  }

  .category-item-wrapper {
    padding-top: 20px;

    .category-item-heading {
      padding-bottom: 8px;
      svg {
        padding-bottom: 5px;
      }
    }

    .category-item-description {
      color: #657077;
      font-size: 13px;
      max-width: 1024px;
    }

    .inherit-title-link {
      margin-right: 4px;
      font-size: 16px;
      font-weight: 700;
    }
  }
`;

StyledComponent.defaultProps = { theme: Base };

const SettingsPageMobileView = ({ children }) => {
  return <StyledComponent>{children}</StyledComponent>;
};

export default SettingsPageMobileView;
