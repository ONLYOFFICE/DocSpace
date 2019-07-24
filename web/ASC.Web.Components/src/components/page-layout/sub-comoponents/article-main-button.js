import React from 'react'
import styled from 'styled-components'

const StyledArticleMainButton = styled.div`
  margin: 16px 0 0;
`;

const ArticleMainButton = (props) => {
  //console.log("ArticleMainButton render");
  return (<StyledArticleMainButton {...props} />);
};

export default ArticleMainButton;