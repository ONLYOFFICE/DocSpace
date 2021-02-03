import React from "react";
import styled from "styled-components";
import { utils } from "asc-web-components";
import equal from "fast-deep-equal/react";
const { tablet } = utils.device;

const StyledArticleHeader = styled.div`
  border-bottom: 1px solid #eceef1;
  height: 56px;

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
