import React from "react";
import styled from "styled-components";
import { utils } from "asc-web-components";
const { tablet } = utils.device;

const StyledArticle = styled.article`
  padding: 0 24px;
  background: #f8f9f9;
  display: flex;
  flex-direction: column;
  width: 264px;
  min-width: 264px;
  /*transition: width 0.3s ease-in-out;*/
  overflow: hidden auto;
  box-sizing: border-box;
  resize: horizontal;

  @media ${tablet} {
    padding: 0 16px;

    ${(props) =>
      props.visible
        ? props.pinned
          ? `
            display: flex;
            width: 240px;
            min-width: 240px;
            margin-top: 50px;
          `
          : `
            width: 240px;
            min-width: 240px;
            max-width: 240px;
            position: fixed;
            height: 100%;
            top: 0;
            left: 0;
            z-index: 400;
            resize: none;
          `
        : `
            width: 240px;
            min-width: 240px;
            max-width: 240px;
            position: fixed;
            height: 100%;
            top: 0;
            left: -240px;
            z-index: 400;
            resize: none;
          `}
  }
`;

class Article extends React.Component {
  /*shouldComponentUpdate() {
    return false;
  }*/

  render() {
    //console.log("PageLayout Article render");
    return <StyledArticle {...this.props} />;
  }
}

export default Article;
