import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Scrollbar from "../../scrollbar";

const StyledArticleBody = styled.div`
  margin: 16px 0;
  ${props => props.displayBorder && `outline: 1px dotted;`}
  flex-grow: 1;
  height: 100%;

  .people-tree-menu{
    margin-right: 30px;
  }
`;

const ArticleBody = React.memo(props => {
  //console.log("PageLayout ArticleBody render");
  const { children } = props;

  return (
    <StyledArticleBody>
      <Scrollbar>{children}</Scrollbar>
    </StyledArticleBody>
  );
});

ArticleBody.displayName = "ArticleBody";

ArticleBody.propTypes = {
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ])
};

export default ArticleBody;
