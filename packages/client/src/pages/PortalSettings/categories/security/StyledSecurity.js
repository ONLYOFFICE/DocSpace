import styled from "styled-components";
import ArrowRightIcon from "PUBLIC_DIR/images/arrow.right.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { Base } from "@docspace/components/themes";
import { mobile } from "@docspace/components/utils/device";

export const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.client.settings.security.arrowFill};
  }
`;

StyledArrowRightIcon.defaultProps = { theme: Base };

export const MainContainer = styled.div`
  width: 100%;

  .subtitle {
    margin-bottom: 20px;
    color: ${(props) => props.theme.client.settings.common.descriptionColor};
  }

  .settings_tabs {
    padding-bottom: 16px;
  }

  .page_loader {
    position: fixed;
    left: 50%;
  }

  .category-item-description {
    margin-top: 8px;
    max-width: 700px;

    .link-learn-more {
      display: block;
      margin: 4px 0 16px 0;
      font-weight: 600;
    }

    p,
    a {
      color: ${(props) => props.theme.client.settings.common.descriptionColor};
    }

    @media ${mobile} {
      padding-right: 8px;
    }
  }
`;

MainContainer.defaultProps = { theme: Base };

export const StyledCategoryWrapper = styled.div`
  display: flex;
  flex-direction: row;
  gap: 4px;
  margin-bottom: 16px;
  align-items: center;
`;

export const StyledTooltip = styled.div`
  .subtitle {
    margin-bottom: 10px;
  }
`;

export const StyledMobileCategoryWrapper = styled.div`
  margin-bottom: 20px;

  .category-item-heading {
    display: flex;
    align-items: center;
    margin-bottom: 5px;
  }

  .category-item-subheader {
    font-size: 13px;
    font-weight: 600;
    margin-bottom: 5px;
  }

  .category-item-description {
    color: ${(props) => props.theme.client.settings.security.descriptionColor};
    font-size: 13px;
    max-width: 1024px;
  }

  .inherit-title-link {
    margin-right: 7px;
    font-size: 16px;
    font-weight: 600;
  }

  .link-text {
    margin: 0;
  }
`;

StyledMobileCategoryWrapper.defaultProps = { theme: Base };

export const LearnMoreWrapper = styled.div`
  display: none;

  .link-learn-more {
    font-weight: 600;
  }

  p,
  a {
    color: ${(props) => props.theme.client.settings.common.descriptionColor};
  }

  @media (max-width: 600px) {
    display: flex;
    flex-direction: column;
    margin-bottom: 20px;
    padding-right: 8px;
  }

  .learn-subtitle {
    margin-bottom: 10px;
  }
`;
