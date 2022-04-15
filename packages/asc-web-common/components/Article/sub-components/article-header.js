import React from "react";
import PropTypes from "prop-types";
import Loaders from "@appserver/common/components/Loaders";
import { isTablet as isTabletUtils } from "@appserver/components/utils/device";
import { isTablet } from "react-device-detect";

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
  isLoading = false,
  ...rest
}) => {
  const heightLoader = isTabletUtils() || isTablet ? "20px" : "32px";

  return isLoading ? (
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

export default React.memo(ArticleHeader);
