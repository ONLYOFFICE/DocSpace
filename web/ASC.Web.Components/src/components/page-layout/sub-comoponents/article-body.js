import React from 'react'
import styled from 'styled-components'
import Scrollbar from '../../scrollbar'

const StyledArticleBody = styled.div`
  margin: 16px 0;
  ${props => props.displayBorder && `outline: 1px dotted;`}
  flex-grow: 1;
`;

const ArticleBody = React.memo(props => { 
  console.log("PageLayout ArticleBody render");
  const { children } = props;

  return (
    <StyledArticleBody>
      <Scrollbar>
        {children}
      </Scrollbar>
    </StyledArticleBody>
  );
});

export default ArticleBody;