import React from "react";
import styled, { css } from "styled-components";
import Text from "@docspace/components/text";
import { ReactSVG } from "react-svg";
import { desktop, mobile, tablet } from "@docspace/components/utils/device";
import { isTablet, isMobileOnly } from "react-device-detect";
import { useTranslation } from "react-i18next";

const StyledHideArticleMenuButton = styled.div`
  display: flex;
  align-items: center;
  position: fixed;
  height: 44px;
  z-index: 210;
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
`;

const HideArticleMenuButton = ({ showText, toggleShowText }) => {
  const { t } = useTranslation("Common");

  return (
    <StyledHideArticleMenuButton showText={showText} onClick={toggleShowText}>
      {showText ? (
        <div className="article-hide-menu-container">
          <ReactSVG
            className="article-hide-menu-icon_svg"
            src="/static/images/article-hide-menu.react.svg"
          />
          <Text
            className="article-hide-menu-text"
            fontWeight={600}
            fontSize="12px"
            noSelect
            truncate
            color="#3B72A7"
          >
            {t("HideArticleMenu")}
          </Text>
        </div>
      ) : (
        <div className="article-show-menu-container">
          <ReactSVG
            className="article-show-menu-icon_svg"
            src="/static/images/article-show-menu.react.svg"
          />
        </div>
      )}
    </StyledHideArticleMenuButton>
  );
};

export default HideArticleMenuButton;
