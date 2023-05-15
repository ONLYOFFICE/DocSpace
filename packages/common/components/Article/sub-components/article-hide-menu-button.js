import React from "react";
import styled, { css } from "styled-components";
import Text from "@docspace/components/text";
import { ReactSVG } from "react-svg";
import { desktop, mobile, tablet } from "@docspace/components/utils/device";
import { isTablet, isMobileOnly } from "react-device-detect";
import { useTranslation } from "react-i18next";
import Base from "@docspace/components/themes/base";
import ArticleHideMenuReactSvgUrl from "PUBLIC_DIR/images/article-hide-menu.react.svg?url";
import ArticleShowMenuReactSvgUrl from "PUBLIC_DIR/images/article-show-menu.react.svg?url";

const StyledHideArticleMenuButton = styled.div`
  display: flex;
  align-items: center;
  position: fixed;
  height: 44px;
  z-index: 209;
  bottom: 89px;
  left: 0;
  cursor: pointer;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  min-width: ${({ showText }) => (showText ? "243px" : "60px")};
  max-width: ${({ showText }) => (showText ? "243px" : "60px")};

  @media ${desktop} {
    ${!isTablet &&
    css`
      display: none;
    `}
  }

  @media ${mobile} {
    display: none;
  }

  ${isMobileOnly &&
  css`
    display: none;
  `}

  .article-hide-menu-container {
    align-items: center;
    margin-left: 16px;

    .article-hide-menu-text {
      margin-left: 8px;
      color: ${({ currentColorScheme }) => currentColorScheme.main.accent};
    }

    @media ${tablet} {
      display: ${({ showText }) => (showText ? "flex" : "none")};
    }

    ${isTablet &&
    css`
      display: ${({ showText }) => (showText ? "flex" : "none")};
    `}
  }

  .article-show-menu-container {
    justify-content: center;
    width: 100%;

    @media ${tablet} {
      display: ${({ showText }) => (showText ? "none" : "flex")};
    }

    ${isTablet &&
    css`
      display: ${({ showText }) => (showText ? "none" : "flex")};
    `}
  }

  .article-hide-menu-icon_svg,
  .article-show-menu-icon_svg {
    height: 28px;
  }

  .article-hide-menu-icon_svg {
    svg {
      path {
        fill: ${({ currentColorScheme }) => currentColorScheme.main.accent};
      }
    }
  }

  .article-show-menu-icon_svg {
    svg {
      path {
        fill: ${(props) => props.theme.article.catalogShowText};
      }
    }
  }
`;

StyledHideArticleMenuButton.defaultProps = { theme: Base };

const HideArticleMenuButton = ({
  showText,
  toggleShowText,
  currentColorScheme,
}) => {
  const { t } = useTranslation("Common");

  return (
    <StyledHideArticleMenuButton
      showText={showText}
      onClick={toggleShowText}
      currentColorScheme={currentColorScheme}
    >
      {showText ? (
        <div
          className="article-hide-menu-container"
          id="document_catalog-hide-menu"
        >
          <ReactSVG
            className="article-hide-menu-icon_svg"
            src={ArticleHideMenuReactSvgUrl}
          />
          <Text
            className="article-hide-menu-text"
            fontWeight={600}
            fontSize="12px"
            noSelect
            truncate
          >
            {t("Common:HideArticleMenu")}
          </Text>
        </div>
      ) : (
        <div
          className="article-show-menu-container"
          id="document_catalog-show-menu"
        >
          <ReactSVG
            className="article-show-menu-icon_svg"
            src={ArticleShowMenuReactSvgUrl}
          />
        </div>
      )}
    </StyledHideArticleMenuButton>
  );
};

export default HideArticleMenuButton;
