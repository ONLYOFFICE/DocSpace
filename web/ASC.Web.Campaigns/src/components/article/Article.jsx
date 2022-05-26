import React from 'react';
import { StyledArticle, ArticleHeader, Header, StyledButtonWrapper, Button, Block, ArticleWrapper } from './styled-article';



export const Article = () => {
    const elements = [];

    for (let i = 0; i < 7; i++) {
        elements.push(<Block key={i} />)
    }
    return  <StyledArticle>
        <ArticleHeader>
            <Header>Documents</Header>
        </ArticleHeader>
        <StyledButtonWrapper>
            <Button>Actions</Button>
        </StyledButtonWrapper>
        <ArticleWrapper>
            {elements}
        </ArticleWrapper>
        
    </StyledArticle>
}