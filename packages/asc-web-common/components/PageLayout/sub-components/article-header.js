import React from "react";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import { tablet } from "@appserver/components/utils/device";

const StyledArticleHeader = styled.div`
  height: 39px;

  @media ${tablet} {
    height: 39px;

    .headline-heading {
      margin-top: -5px;
    }
  }

  @media ${tablet} {
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
