import styled from "styled-components";
import ArrowRightIcon from "@appserver/studio/public/images/arrow.right.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { Base } from "@appserver/components/themes";

export const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.studio.settings.security.arrowFill};
  }
`;

StyledArrowRightIcon.defaultProps = { theme: Base };

export const MainContainer = styled.div`
  width: 100%;

  .subtitle {
    margin-bottom: 20px;
  }

  .settings_tabs {
    padding-bottom: 16px;
  }

  .page_loader {
    position: fixed;
    left: 50%;
  }

  .category-item-wrapper {
    margin-bottom: 40px;

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
      color: ${(props) =>
        props.theme.studio.settings.security.descriptionColor};
      font-size: 12px;
      max-width: 1024px;
    }

    .inherit-title-link {
      margin-right: 7px;
      font-size: 19px;
      font-weight: 600;
    }

    .link-text {
      margin: 0;
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
