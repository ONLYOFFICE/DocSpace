import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { isMobile, isMobileOnly, isTablet } from "react-device-detect";

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
import ArticleAlerts from "./sub-components/article-alerts";
import ArticleLiveChat from "./sub-components/article-live-chat";
import ArticleApps from "./sub-components/article-apps";
import { StyledArticle } from "./styled-article";
import HideArticleMenuButton from "./sub-components/article-hide-menu-button";
import Portal from "@docspace/components/portal";

const Article = ({
  showText,
  setShowText,
  articleOpen,
  toggleShowText,
  toggleArticleOpen,
  setIsMobileArticle,
  children,

  withMainButton,

  hideProfileBlock,

  currentColorScheme,
  setArticleOpen,
  withSendAgain,
  mainBarVisible,
  isBannerVisible,

  isLiveChatAvailable,

  onLogoClickAction,
  theme,

  ...rest
}) => {
  const [articleHeaderContent, setArticleHeaderContent] = React.useState(null);
  const [articleMainButtonContent, setArticleMainButtonContent] =
    React.useState(null);
  const [articleBodyContent, setArticleBodyContent] = React.useState(null);
  const [correctTabletHeight, setCorrectTabletHeight] = React.useState(null);

  React.useEffect(() => {
    if (isMobileOnly) {
      window.addEventListener("popstate", onMobileBack);
      return () => window.removeEventListener("popstate", onMobileBack);
    }
  }, [onMobileBack]);

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
    const showArticle = JSON.parse(localStorage.getItem("showArticle"));

    if (isMobileOnly || isMobileUtils() || window.innerWidth === 375) {
      setShowText(true);
      setIsMobileArticle(true);
    }
    if (
      ((isTabletUtils() && window.innerWidth !== 375) || isMobile) &&
      !isMobileOnly
    ) {
      setIsMobileArticle(true);

      if (showArticle) return;

      setShowText(false);
    }
    if (isDesktopUtils() && !isMobile) {
      setShowText(true);
      setIsMobileArticle(false);
    }
  }, [setShowText, setIsMobileArticle]);

  const onMobileBack = React.useCallback(() => {
    //close article

    setArticleOpen(false);
  }, [setArticleOpen]);

  // TODO: make some better
  const onResize = React.useCallback(() => {
    let correctTabletHeight = window.innerHeight;

    if (mainBarVisible)
      correctTabletHeight -= window.innerHeight <= 768 ? 62 : 90;

    const isTouchDevice =
      "ontouchstart" in window ||
      navigator.maxTouchPoints > 0 ||
      navigator.msMaxTouchPoints > 0;

    const path = window.location.pathname.toLowerCase();

    if (
      isBannerVisible &&
      isMobile &&
      isTouchDevice &&
      (path.includes("rooms") || path.includes("files"))
    )
      correctTabletHeight -= 80;

    setCorrectTabletHeight(correctTabletHeight);
  }, [mainBarVisible, isBannerVisible]);

  React.useEffect(() => {
    if (isTablet) {
      onResize();
      window.addEventListener("resize", onResize);
    }

    return () => {
      window.removeEventListener("resize", onResize);
    };
  }, [onResize]);

  const articleComponent = (
    <>
      <StyledArticle
        id={"article-container"}
        showText={showText}
        articleOpen={articleOpen}
        $withMainButton={withMainButton}
        correctTabletHeight={correctTabletHeight}
        {...rest}
      >
        <SubArticleHeader
          showText={showText}
          onLogoClickAction={onLogoClickAction}
        >
          {articleHeaderContent ? articleHeaderContent.props.children : null}
        </SubArticleHeader>

        {articleMainButtonContent &&
        withMainButton &&
        !isMobileOnly &&
        !isMobileUtils() ? (
          <SubArticleMainButton showText={showText}>
            {articleMainButtonContent.props.children}
          </SubArticleMainButton>
        ) : null}

        <SubArticleBody showText={showText}>
          {articleBodyContent ? articleBodyContent.props.children : null}
          <HideArticleMenuButton
            showText={showText}
            toggleShowText={toggleShowText}
            currentColorScheme={currentColorScheme}
          />
          {!hideProfileBlock && !isMobileOnly && (
            <ArticleProfile showText={showText} />
          )}

          <ArticleAlerts />
          <ArticleApps showText={showText} theme={theme} />
          {!isMobile && isLiveChatAvailable && (
            <ArticleLiveChat
              currentColorScheme={currentColorScheme}
              withMainButton={withMainButton && !!articleMainButtonContent}
            />
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

  const renderPortalArticle = () => {
    const rootElement = document.getElementById("root");

    return (
      <Portal
        element={articleComponent}
        appendTo={rootElement}
        visible={true}
      />
    );
  };

  // console.log("Article render", {
  //   articleMainButton: !!articleMainButtonContent,
  //   withMainButton,
  // });

  return isMobileOnly ? renderPortalArticle() : articleComponent;
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

export default inject(({ auth }) => {
  const { settingsStore, userStore, isLiveChatAvailable, bannerStore } = auth;

  const { withSendAgain } = userStore;

  const { isBannerVisible } = bannerStore;

  const {
    showText,
    setShowText,
    articleOpen,
    setIsMobileArticle,
    toggleShowText,
    toggleArticleOpen,
    currentColorScheme,
    setArticleOpen,
    mainBarVisible,
    theme,
  } = settingsStore;

  return {
    showText,
    setShowText,
    articleOpen,
    setIsMobileArticle,
    toggleShowText,
    toggleArticleOpen,

    currentColorScheme,
    setArticleOpen,
    withSendAgain,
    mainBarVisible,
    isBannerVisible,

    isLiveChatAvailable,

    theme,
  };
})(observer(Article));
