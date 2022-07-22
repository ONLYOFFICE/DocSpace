import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { isMobile, isMobileOnly } from "react-device-detect";
import { Resizable } from "re-resizable";

import {
  isDesktop as isDesktopUtils,
  isTablet as isTabletUtils,
  isMobile as isMobileUtils,
} from "@docspace/components/utils/device";

import SubArticleBackdrop from "./sub-components/article-backdrop";
import SubArticleHeader from "./sub-components/article-header";
import SubArticleMainButton from "./sub-components/article-main-button";
import SubArticleBody from "./sub-components/article-body";
import ArticleProfile from "./sub-components/article-profile";

import { StyledArticle } from "./styled-article";

const enable = {
  top: false,
  right: false,
  bottom: false,
  left: false,
};

const Article = ({
  showText,
  setShowText,
  articleOpen,
  toggleShowText,
  toggleArticleOpen,
  setIsMobileArticle,
  isLoadedPage,
  children,

  isBannerVisible,
  hideProfileBlock,
  ...rest
}) => {
  const [articleHeaderContent, setArticleHeaderContent] = React.useState(null);
  const [
    articleMainButtonContent,
    setArticleMainButtonContent,
  ] = React.useState(null);
  const [articleBodyContent, setArticleBodyContent] = React.useState(null);

  React.useEffect(() => {
    if (isMobileOnly) {
      window.addEventListener("popstate", hideText);
      return () => window.removeEventListener("popstate", hideText);
    }
  }, [hideText]);

  React.useEffect(() => {
    window.addEventListener("resize", sizeChangeHandler);
    return () => window.removeEventListener("resize", sizeChangeHandler);
  }, []);

  React.useEffect(() => {
    sizeChangeHandler();
  }, []);

  React.useEffect(() => {
    React.Children.forEach(children, (child) => {
      const childType =
        child && child.type && (child.type.displayName || child.type.name);

      switch (childType) {
        case Article.Header.displayName:
          setArticleHeaderContent(child);
          break;
        case Article.MainButton.displayName:
          setArticleMainButtonContent(child);
          break;
        case Article.Body.displayName:
          setArticleBodyContent(child);
          break;
        default:
          break;
      }
    });
  }, [children]);

  const sizeChangeHandler = React.useCallback(() => {
    if (isMobileOnly || isMobileUtils() || window.innerWidth === 375) {
      setShowText(true);
      setIsMobileArticle(true);
    }
    if (
      ((isTabletUtils() && window.innerWidth !== 375) || isMobile) &&
      !isMobileOnly
    ) {
      setShowText(false);
      setIsMobileArticle(true);
    }
    if (isDesktopUtils() && !isMobile) {
      setShowText(true);
      setIsMobileArticle(false);
    }
  }, [setShowText, setIsMobileArticle]);

  const hideText = React.useCallback(
    (event) => {
      event.preventDefault;
      setShowText(false);
    },
    [setShowText]
  );

  return (
    <>
      <StyledArticle
        id={"article-container"}
        showText={showText}
        articleOpen={articleOpen}
        isBannerVisible={isBannerVisible}
        {...rest}
      >
        <SubArticleHeader
          isLoadedPage={isLoadedPage}
          showText={showText}
          onClick={toggleShowText}
        >
          {articleHeaderContent ? articleHeaderContent.props.children : null}
        </SubArticleHeader>
        {articleMainButtonContent && !isMobileOnly && !isMobileUtils() ? (
          <SubArticleMainButton showText={showText}>
            {articleMainButtonContent.props.children}
          </SubArticleMainButton>
        ) : null}
        <SubArticleBody showText={showText}>
          {articleBodyContent ? articleBodyContent.props.children : null}
          {!hideProfileBlock && !isMobileOnly && (
            <ArticleProfile showText={showText} />
          )}
        </SubArticleBody>
      </StyledArticle>
      {articleOpen && (isMobileOnly || window.innerWidth <= 375) && (
        <>
          <SubArticleBackdrop onClick={toggleArticleOpen} />
        </>
      )}
      {articleMainButtonContent && (isMobileOnly || isMobileUtils()) ? (
        <SubArticleMainButton showText={showText}>
          {articleMainButtonContent.props.children}
        </SubArticleMainButton>
      ) : null}
    </>
  );
};

Article.propTypes = {
  showText: PropTypes.bool,
  setShowText: PropTypes.func,
  articleOpen: PropTypes.bool,
  toggleArticleOpen: PropTypes.func,
  setIsMobileArticle: PropTypes.func,
  children: PropTypes.any,
  hideProfileBlock: PropTypes.bool,
};

Article.Header = () => {
  return null;
};
Article.Header.displayName = "Header";

Article.MainButton = () => {
  return null;
};
Article.MainButton.displayName = "MainButton";

Article.Body = () => {
  return null;
};
Article.Body.displayName = "Body";

export default inject(({ auth, bannerStore }) => {
  const { settingsStore } = auth;

  const { isBannerVisible } = bannerStore;

  const {
    showText,
    setShowText,
    articleOpen,
    setIsMobileArticle,
    toggleShowText,
    toggleArticleOpen,
  } = settingsStore;

  return {
    showText,
    setShowText,
    articleOpen,
    setIsMobileArticle,
    toggleShowText,
    toggleArticleOpen,

    isBannerVisible,
  };
})(observer(Article));
