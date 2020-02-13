import React from "react";
import styled from "styled-components";

const StyledArticleHeader = styled.div`
  border-bottom: 1px solid #eceef1;
  height: 56px;
  /*display: none;*/
`;

const ArticleHeader = React.memo(props => {
  //console.log("PageLayout ArticleHeader render");
  return <StyledArticleHeader {...props} />;
});

ArticleHeader.displayName = "ArticleHeader";

export default ArticleHeader;
