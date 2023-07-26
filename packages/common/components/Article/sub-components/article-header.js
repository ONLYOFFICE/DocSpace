import React from "react";
import PropTypes from "prop-types";
import { useNavigate } from "react-router-dom";
import Loaders from "@docspace/common/components/Loaders";
import { isTablet as isTabletUtils } from "@docspace/components/utils/device";
import { Link } from "react-router-dom";
import { isTablet, isMobileOnly } from "react-device-detect";
import { isMobile } from "@docspace/components/utils/device";
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
  onLogoClickAction,
  isBurgerLoading,
  whiteLabelLogoUrls,
  theme,
  ...rest
}) => {
  const navigate = useNavigate();

  const isTabletView =
    (isTabletUtils() || isTablet) && !isMobileOnly && !isMobile();

  const onLogoClick = () => {
    onLogoClickAction && onLogoClickAction();
    navigate("/");
  };

  const burgerLogo = !theme.isBase
    ? getLogoFromPath(whiteLabelLogoUrls[5].path.dark)
    : getLogoFromPath(whiteLabelLogoUrls[5].path.light);
  const logo = !theme.isBase
    ? getLogoFromPath(whiteLabelLogoUrls[0].path.dark)
    : getLogoFromPath(whiteLabelLogoUrls[0].path.light);

  if (isMobileOnly || isMobile()) return <></>;

  const isLoadingComponent = isTabletView ? (
    <Loaders.ArticleHeader height="28px" width={showText ? "100%" : "28px"} />
  ) : (
    <Loaders.ArticleHeader height="28px" width="211px" />
  );

  const mainComponent = (
    <>
      {isTabletView && (
        <StyledIconBox name="article-burger" showText={showText}>
          <img src={burgerLogo} onClick={onLogoClick} />
        </StyledIconBox>
      )}
      <StyledHeading showText={showText} size="large">
        {isTabletView ? (
          <img className="logo-icon_svg" src={logo} onClick={onLogoClick} />
        ) : (
          <Link to="/">
            <img className="logo-icon_svg" src={logo} />
          </Link>
        )}
      </StyledHeading>
    </>
  );

  return (
    <StyledArticleHeader showText={showText} {...rest}>
      {isBurgerLoading ? isLoadingComponent : mainComponent}
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
