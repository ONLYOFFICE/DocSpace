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

  ${({ theme }) =>
    theme.interfaceDirection === "rtl" && "transform: scaleX(-1);"}
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

    ${({ theme }) =>
      theme.interfaceDirection === "rtl" ? `right: 50%;` : `left: 50%;`}
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
  margin-bottom: 8px;
  align-items: center;
`;

export const StyledTooltip = styled.div`
  .subtitle {
    margin-bottom: 10px;
  }
`;

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
    line-height: 20px;
  }

  .page-subtitle {
    color: ${(props) =>
      props.theme.client.settings.security.descriptionColor} !important;
  }

  .learn-subtitle {
    margin-bottom: 10px;
  }
`;

export const StyledBruteForceProtection = styled.div`
  width: 100%;

  .brute-force-protection-input {
    width: 100%;
    max-width: 350px;
  }

  .error-text {
    position: absolute;
    font-size: 10px;
    color: #f21c0e;
  }

  .save-cancel-buttons {
    margin-top: 24px;
  }

  .input-container {
    margin-bottom: 8px;
    margin-right: 8px;
  }

  .mobile-description {
    margin-bottom: 12px;
  }

  .description {
    max-width: 700px;
    padding-bottom: 19px;

    .page-subtitle {
      line-height: 20px;
      padding-right: 8px;
      color: ${(props) =>
        props.theme.client.settings.security.descriptionColor};
      padding-bottom: 7px;
    }

    .link {
      line-height: 15px;
      font-weight: 600;
      color: ${(props) =>
        props.theme.client.settings.security.descriptionColor};
      text-decoration: underline;
    }

    @media (max-width: 600px) {
      padding-bottom: 20px;
    }
  }
`;
