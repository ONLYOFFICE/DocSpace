import React from "react";
import styled from "styled-components";
import { utils } from "asc-web-components";
import { Resizable } from "re-resizable";
const { tablet } = utils.device;

const StyledArticle = styled.article`
  padding: 0 24px;
  background: #f8f9f9;
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  overflow: hidden auto;
  box-sizing: border-box;

  @media ${tablet} {
    padding: 0 16px;
    ${(props) =>
      props.visible
        ? props.pinned
          ? `
            display: flex;
            width: 240px;
            min-width: 240px;
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
          `}
  }
`;

const StyledResizable = styled(Resizable)`
  min-width: 265px;
  max-width: calc(100vw - 368px);
  border-right: 1px solid #D0D5DA;
`;

class Article extends React.Component {
  render() {
    //console.log("PageLayout Article render");
    return (
      <StyledResizable>
        <StyledArticle {...this.props} />
      </StyledResizable>
    );
  }
}

export default Article;
