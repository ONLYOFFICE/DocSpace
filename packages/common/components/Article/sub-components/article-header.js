import React from "react";
import PropTypes from "prop-types";
import { useHistory } from "react-router";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet as isTabletUtils } from "@docspace/components/utils/device";
import { Link } from "react-router-dom";
import { isTablet, isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";
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
  whiteLabelLogoUrls,
  ...rest
}) => {
  const history = useHistory();

  const isTabletView = (isTabletUtils() || isTablet) && !isMobileOnly;
  const onLogoClick = () => history.push("/");

  const isSvgLogo = whiteLabelLogoUrls[0].includes(".svg");

  if (isMobileOnly) return <></>;
  return (
    <StyledArticleHeader showText={showText} {...rest}>
      {isTabletView && isBurgerLoading ? (
        <Loaders.ArticleHeader height="28px" width="28px" />
      ) : (
        <StyledIconBox name="article-burger" showText={showText}>
          <img src={whiteLabelLogoUrls[5]} onClick={onLogoClick} />
        </StyledIconBox>
      )}

      {!isTabletView && isBurgerLoading ? (
        <Loaders.ArticleHeader height="28px" width="211px" />
      ) : (
        <StyledHeading showText={showText} size="large">
          {isTabletView ? (
            isSvgLogo ? (
              <ReactSVG
                className="logo-icon_svg"
                src={whiteLabelLogoUrls[0]}
                onClick={onLogoClick}
              />
            ) : (
              <img
                className="logo-icon_svg"
                src={whiteLabelLogoUrls[0]}
                onClick={onLogoClick}
              />
            )
          ) : (
            <Link to="/">
              {isSvgLogo ? (
                <ReactSVG
                  className="logo-icon_svg"
                  src={whiteLabelLogoUrls[0]}
                />
              ) : (
                <img className="logo-icon_svg" src={whiteLabelLogoUrls[0]} />
              )}
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
  const { isBurgerLoading, whiteLabelLogoUrls } = settingsStore;
  return {
    isBurgerLoading,
    whiteLabelLogoUrls,
  };
})(observer(ArticleHeader));
