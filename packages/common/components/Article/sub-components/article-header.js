import React from "react";
import PropTypes from "prop-types";
import { useHistory } from "react-router-dom";
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
import { getLogoFromPath } from "../../../utils";

const ArticleHeader = ({
  showText,
  children,
  onClick,
  isBurgerLoading,
  whiteLabelLogoUrls,
  theme,
  ...rest
}) => {
  const history = useHistory();

  const isTabletView = (isTabletUtils() || isTablet) && !isMobileOnly;
  const onLogoClick = () => history.push("/");

  const burgerLogo = !theme.isBase
    ? getLogoFromPath(whiteLabelLogoUrls[5].path.dark)
    : getLogoFromPath(whiteLabelLogoUrls[5].path.light);
  const logo = !theme.isBase
    ? getLogoFromPath(whiteLabelLogoUrls[0].path.dark)
    : getLogoFromPath(whiteLabelLogoUrls[0].path.light);

  if (isMobileOnly) return <></>;
  return (
    <StyledArticleHeader showText={showText} {...rest}>
      {isTabletView && isBurgerLoading ? (
        <Loaders.ArticleHeader height="28px" width="28px" />
      ) : (
        <StyledIconBox name="article-burger" showText={showText}>
          <img src={burgerLogo} onClick={onLogoClick} />
        </StyledIconBox>
      )}

      {!isTabletView && isBurgerLoading ? (
        <Loaders.ArticleHeader height="28px" width="211px" />
      ) : (
        <StyledHeading showText={showText} size="large">
          {isTabletView ? (
            <img className="logo-icon_svg" src={logo} onClick={onLogoClick} />
          ) : (
            <Link to="/">
              <img className="logo-icon_svg" src={logo} />
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
  const { isBurgerLoading, whiteLabelLogoUrls, theme } = settingsStore;

  return {
    isBurgerLoading,
    whiteLabelLogoUrls,
    theme,
  };
})(observer(ArticleHeader));
