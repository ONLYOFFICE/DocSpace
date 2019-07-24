import React from 'react'
import styled from 'styled-components'
import Scrollbar from '../../scrollbar'

const StyledArticleBody = styled.div`
  margin: 16px 0;
  outline: 1px dotted;
  flex-grow: 1;
`;

const ArticleBody = (props) => { 
  //console.log("ArticleBody render");
  const { children } = props;

  return (
    <StyledArticleBody>
      <Scrollbar>
        {children}
      </Scrollbar>
    </StyledArticleBody>
  );
}

export default ArticleBody;