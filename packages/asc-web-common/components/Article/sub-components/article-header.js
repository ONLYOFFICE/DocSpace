import React from "react";
import PropTypes from "prop-types";

import {
  StyledArticleHeader,
  StyledHeading,
  StyledIconBox,
  StyledMenuIcon,
} from "../styled-article";

const ArticleHeader = ({ showText, children, onClick, ...rest }) => {
  return (
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
