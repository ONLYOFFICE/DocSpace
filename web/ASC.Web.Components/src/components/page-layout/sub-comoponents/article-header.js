import React from 'react'
import styled from 'styled-components'
import device from '../../device'

const StyledArticleHeader = styled.div`
  border-bottom: 1px solid #ECEEF1;
  height: 56px;

  @media ${device.tablet} {
    display: ${props => props.visible ? 'block' : 'none'};
  }
`;

const ArticleHeader = React.memo(props => { 
  console.log("PageLayout ArticleHeader render");
  return (<StyledArticleHeader {...props}/>) 
});

export default ArticleHeader;