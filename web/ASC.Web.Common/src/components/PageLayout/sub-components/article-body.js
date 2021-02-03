import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import { Scrollbar, utils } from "asc-web-components";
const { tablet, smallTablet } = utils.device;

const StyledArticleBody = styled.div`
  ${(props) => props.displayBorder && `outline: 1px dotted;`}
  flex-grow: 1;
  height: 100%;

  @media ${tablet} {
    height: calc(100% - 104px);
    display: table;
    width: 100%;

    .custom-scrollbar {
      display: table-cell;
    }
  }

  @media ${smallTablet} {
    display: flex;
    height: 100%;
  }

  .people-tree-menu {
    margin-right: 0;
  }

  .custom-scrollbar {
    .nav-thumb-vertical {
      opacity: 0;
      transition: opacity 200ms ease;
    }
  }

  :hover {
    .custom-scrollbar {
      .nav-thumb-vertical {
        opacity: 1;
      }
    }
  }
`;

const StyledArticleWrapper = styled.div`
  margin: 16px 0;
`;

class ArticleBody extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout ArticleBody render");
    const { children } = this.props;

    return (
      <StyledArticleBody>
        <Scrollbar className="custom-scrollbar" stype="mediumBlack">
          <StyledArticleWrapper>{children}</StyledArticleWrapper>
        </Scrollbar>
      </StyledArticleBody>
    );
  }
}

ArticleBody.displayName = "ArticleBody";

ArticleBody.propTypes = {
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
};

export default ArticleBody;
