import React from 'react'
import styled from 'styled-components'
import device from '../../device'

const StyledArticle = styled.article`
  padding: 0 16px;
  background: #F8F9F9;
  display: flex;
  flex-direction: column;
  width: 264px;
  min-width: 264px;
  transition: width .3s ease-in-out;
  overflow: hidden auto;

  @media ${device.tablet} {
    ${props => props.visible
      ? props.pinned
          ? `
            display: flex;
            width: 240px;
            min-width: 240px;
          `
          : `
            width: 240px;
            min-width: 240px;
            position: fixed;
            height: 100%;
            top: 0;
            left: 0;
            z-index: 400;
          `
      : `
        display: none;
        width: 0px;
      `
    }
  }
`;

const Article = (props) => <StyledArticle {...props}/>

export default Article;