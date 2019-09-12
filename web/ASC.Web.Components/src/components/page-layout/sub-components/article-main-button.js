import React from "react";
import styled from "styled-components";

const StyledArticleMainButton = styled.div`
  margin: 16px 0 0;
`;

const ArticleMainButton = React.memo(props => {
  //console.log("PageLayout ArticleMainButton render");
  return <StyledArticleMainButton {...props} />;
});

ArticleMainButton.displayName = "ArticleMainButton";

export default ArticleMainButton;
