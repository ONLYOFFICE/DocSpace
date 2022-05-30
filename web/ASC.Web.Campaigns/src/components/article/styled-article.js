import styled from "styled-components";

const StyledArticle = styled.div`
    width: 302px;
    background-color: ${props => props.theme.article};
`;

const ArticleHeader = styled.div`
    padding: 11px 20px 14px;
`;

const Header = styled.div`
    font-size: 26px;
    font-weight: 700;
    padding: 0;
    margin: 0;
    color: ${props => props.theme.color};
`;

const StyledButtonWrapper = styled.div`
    padding: 0px 20px 16px;
`;

const Button = styled.button`
    padding: 6px 10px;
    width: 100%;
    text-align: left;
    border: none;
    background-color: #ED7309;
    box-sizing: border-box;
    color: ${props => props.theme.articleButton};
    font-weight: 900;
    border-radius: 3px;
`;


const Block = styled.div`
    height: 28px;
    width: 100%;
    background-color: ${props => props.theme.loaders};
    margin-top: 10px;
`;

const ArticleWrapper = styled.div`
    padding: 0 20px;
`;


export {StyledArticle, ArticleHeader, Header, StyledButtonWrapper, Button, Block, ArticleWrapper};