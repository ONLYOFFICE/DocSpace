import React from "react";
import styled from "styled-components";
import isEqual from "lodash/isEqual";

const StyledArticleMainButton = styled.div`
  margin: 16px 0 0;
`;

class ArticleMainButton extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout ArticleMainButton render");
    return <StyledArticleMainButton {...this.props} />;
  }
}

ArticleMainButton.displayName = "ArticleMainButton";

export default ArticleMainButton;
