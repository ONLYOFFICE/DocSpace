import React from "react";
import PropTypes from "prop-types";
import { useHistory } from "react-router";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet as isTabletUtils } from "@docspace/components/utils/device";
import { Link } from "react-router-dom";
import { isTablet, isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";
import {
  StyledArticleHeader,
  StyledHeading,
  StyledIconBox,
} from "../styled-article";

const ArticleHeader = ({
  showText,
  children,
  onClick,
  isBurgerLoading,
  ...rest
}) => {
  const history = useHistory();

  const isTabletView = (isTabletUtils() || isTablet) && !isMobileOnly;

  const onLogoClick = () => {
    if (showText && isTabletView) onClick();
    else history.push("/");
  };

  if (isMobileOnly) return <></>;
  return (
    <StyledArticleHeader showText={showText} {...rest}>
      {isTabletView && isBurgerLoading ? (
        <Loaders.ArticleHeader height="28px" width="28px" />
      ) : (
        <StyledIconBox name="article-burger" showText={showText}>
          <img src="/static/images/logo.icon.react.svg" onClick={onClick} />
        </StyledIconBox>
      )}

      {!isTabletView && isBurgerLoading ? (
        <Loaders.ArticleHeader height="28px" width="211px" />
      ) : (
        <StyledHeading showText={showText} size="large">
          {isTabletView ? (
            <img
              src="/static/images/logo.docspace.react.svg"
              onClick={onLogoClick}
            />
          ) : (
            <Link to="/">
              <img src="/static/images/logo.docspace.react.svg" />
            </Link>
          )}
        </StyledHeading>
      )}
    </StyledArticleHeader>
  );
};

ArticleHeader.propTypes = {
  children: PropTypes.any,
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

ArticleHeader.displayName = "Header";

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { isBurgerLoading } = settingsStore;
  return {
    isBurgerLoading,
  };
})(observer(ArticleHeader));
