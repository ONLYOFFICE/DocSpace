import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { useHistory, useLocation } from "react-router";
import Loaders from "@appserver/common/components/Loaders";
import { isTablet as isTabletUtils } from "@appserver/components/utils/device";
import { isTablet, isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";
import {
  StyledArticleHeader,
  StyledHeading,
  StyledIconBox,
  StyledMenuIcon,
} from "../styled-article";

const ArticleHeader = ({
  showText,
  children,
  onClick,
  isLoadedPage,
  isLoaded,
  tReady,
  setIsLoadedArticleHeader,
  isBurgerLoading,
  ...rest
}) => {
  const location = useLocation();
  const history = useHistory();

  const isLoadedSetting = isLoaded;

  const commonSettings =
    location.pathname.includes("common/customization") ||
    location.pathname === "/settings";

  useEffect(() => {
    if (isLoadedSetting) setIsLoadedArticleHeader(isLoadedSetting);
  }, [isLoadedSetting]);

  const showLoader = commonSettings ? !isLoadedPage : false;

  const isTabletView = (isTabletUtils() || isTablet) && !isMobileOnly;

  const onLogoClick = () => {
    if (showText && isTabletView) onClick();
    else history.push("/");
  };

  if (isMobileOnly) return <></>;
  return (
    <StyledArticleHeader showText={showText} {...rest}>
      {isTabletView && (isBurgerLoading || showLoader) ? (
        <Loaders.ArticleHeader
          height="20px"
          width="20px"
          style={{ height: "20px" }}
        />
      ) : (
        <StyledIconBox name="article-burger" showText={showText}>
          <ReactSVG
            src="/static/images/logo.icon.react.svg"
            onClick={onClick}
          />
        </StyledIconBox>
      )}

      <StyledHeading showText={showText} size="large">
        <ReactSVG
          src="/static/images/logo.docspace.react.svg"
          onClick={onLogoClick}
        />
      </StyledHeading>
    </StyledArticleHeader>
  );
};

ArticleHeader.propTypes = {
  children: PropTypes.any,
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

ArticleHeader.displayName = "Header";

export default inject(({ common, auth }) => {
  const { isLoaded, setIsLoadedArticleHeader } = common;
  const { settingsStore } = auth;
  const { isBurgerLoading } = settingsStore;
  return {
    isLoaded,
    setIsLoadedArticleHeader,
    isBurgerLoading,
  };
})(observer(ArticleHeader));
