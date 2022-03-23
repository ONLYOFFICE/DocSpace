import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import { isMobile, isMobileOnly } from "react-device-detect";
import { Resizable } from "re-resizable";

import {
  isDesktop as isDesktopUtils,
  isTablet as isTabletUtils,
  isMobile as isMobileUtils,
} from "@appserver/components/utils/device";

import SubArticleBackdrop from "./sub-components/article-backdrop";
import SubArticleHeader from "./sub-components/article-header";
import SubArticleMainButton from "./sub-components/article-main-button";
import SubArticleBody from "./sub-components/article-body";

import { StyledArticle } from "./styled-article";

const enable = {
  top: false,
  right: !isMobile,
  bottom: false,
  left: false,
};

const Article = ({
  showText,
  setShowText,
  articleOpen,
  toggleShowText,
  toggleArticleOpen,
  children,
  ...rest
}) => {
  const [articleHeaderContent, setArticleHeaderContent] = React.useState(null);
  const [
    articleMainButtonContent,
    setArticleMainButtonContent,
  ] = React.useState(null);
  const [articleBodyContent, setArticleBodyContent] = React.useState(null);

  const refTimer = React.useRef(null);

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
    clearTimeout(refTimer.current);

    refTimer.current = setTimeout(() => {
      if (isMobileOnly || isMobileUtils() || window.innerWidth === 375)
        setShowText(true);
      if (
        ((isTabletUtils() && window.innerWidth !== 375) || isMobile) &&
        !isMobileOnly
      )
        setShowText(false);
      if (isDesktopUtils() && !isMobile) setShowText(true);
    }, 100);
  }, [refTimer.current, setShowText]);

  const hideText = React.useCallback((event) => {
    event.preventDefault;
    setShowText(false);
  }, []);

  return (
    <>
      <StyledArticle showText={showText} articleOpen={articleOpen} {...rest}>
        <Resizable
          defaultSize={{
            width: 256,
          }}
          enable={enable}
          className="resizable-block"
          handleWrapperClass="resizable-border not-selectable"
        >
          <SubArticleHeader showText={showText} onClick={toggleShowText}>
            {articleHeaderContent ? articleHeaderContent.props.children : null}
          </SubArticleHeader>
          {articleMainButtonContent && !isMobileOnly && !isMobileUtils() ? (
            <SubArticleMainButton showText={showText}>
              {articleMainButtonContent.props.children}
            </SubArticleMainButton>
          ) : null}
          <SubArticleBody showText={showText}>
            {articleBodyContent ? articleBodyContent.props.children : null}
          </SubArticleBody>
        </Resizable>
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
  children: PropTypes.any,
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
  const { settingsStore } = auth;

  const {
    showText,
    setShowText,
    articleOpen,
    toggleShowText,
    toggleArticleOpen,
  } = settingsStore;

  return {
    showText,
    setShowText,
    articleOpen,
    toggleShowText,
    toggleArticleOpen,
  };
})(observer(Article));
