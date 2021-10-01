import React from "react";
import styled from "styled-components";
import equal from "fast-deep-equal/react";

const StyledArticleHeader = styled.div`
  height: 64px;

  @media (min-width: 1314px) {
    height: 39px;

    .headline-heading {
      margin-top: -5px;
    }
  }

  @media (max-width: 1313px) {
    display: none;
  }
`;

class ArticleHeader extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout ArticleHeader render");
    return <StyledArticleHeader {...this.props} />;
  }
}

ArticleHeader.displayName = "ArticleHeader";

export default ArticleHeader;
