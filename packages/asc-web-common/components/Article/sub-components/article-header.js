import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { useLocation } from "react-router";
import Loaders from "@appserver/common/components/Loaders";
import { isTablet as isTabletUtils } from "@appserver/components/utils/device";
import { isTablet } from "react-device-detect";
import { inject, observer } from "mobx-react";
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
  ...rest
}) => {
  const location = useLocation();

  const isLoadedSetting = isLoaded;

  const commonSettings =
    location.pathname.includes("common") || location.pathname === "/settings";

  useEffect(() => {
    if (isLoadedSetting) setIsLoadedArticleHeader(isLoadedSetting);
  }, [isLoadedSetting]);

  const heightLoader = isTabletUtils() || isTablet ? "20px" : "32px";

  const showLoader = commonSettings ? !isLoadedPage : false;

  return showLoader ? (
    <StyledArticleHeader>
      <Loaders.ArticleHeader height={heightLoader} className="loader" />
    </StyledArticleHeader>
  ) : (
    <StyledArticleHeader showText={showText} {...rest}>
      <StyledIconBox name="article-burger">
        <StyledMenuIcon onClick={onClick} />
      </StyledIconBox>

      <StyledHeading showText={showText} size="large">
        {children}
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

export default inject(({ common }) => {
  const { isLoaded, setIsLoadedArticleHeader } = common;

  return {
    isLoaded,
    setIsLoadedArticleHeader,
  };
})(observer(ArticleHeader));
