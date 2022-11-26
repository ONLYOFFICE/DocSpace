import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { isMobile, isMobileOnly } from "react-device-detect";

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
import ArticlePaymentAlert from "./sub-components/article-payment-alert";
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

  isGracePeriod,

  hideProfileBlock,
  isFreeTariff,
  isAvailableArticlePaymentAlert,
  currentColorScheme,
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

  const articleComponent = (
    <>
      <StyledArticle
        id={"article-container"}
        showText={showText}
        articleOpen={articleOpen}
        {...rest}
      >
        <SubArticleHeader showText={showText}>
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
          {isAvailableArticlePaymentAlert &&
            (isFreeTariff || isGracePeriod) &&
            showText && (
              <ArticlePaymentAlert
                isFreeTariff={isFreeTariff}
                toggleArticleOpen={toggleArticleOpen}
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
  const {
    settingsStore,
    currentQuotaStore,
    currentTariffStatusStore,
    userStore,
  } = auth;
  const { isFreeTariff } = currentQuotaStore;
  const { isGracePeriod } = currentTariffStatusStore;

  const { user } = userStore;

  const isAvailableArticlePaymentAlert = user.isOwner || user.isAdmin;

  const {
    showText,
    setShowText,
    articleOpen,
    setIsMobileArticle,
    toggleShowText,
    toggleArticleOpen,
    currentColorScheme,
  } = settingsStore;

  return {
    showText,
    setShowText,
    articleOpen,
    setIsMobileArticle,
    toggleShowText,
    toggleArticleOpen,
    isFreeTariff,
    isGracePeriod,
    isAvailableArticlePaymentAlert,
    currentColorScheme,
  };
})(observer(Article));
