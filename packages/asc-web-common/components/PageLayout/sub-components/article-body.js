import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import Scrollbar from "@appserver/components/scrollbar";
import { tablet, smallTablet } from "@appserver/components/utils/device";
import { isMobile } from "react-device-detect";

const StyledArticleBody = styled.div`
  ${(props) => props.displayBorder && `outline: 1px dotted;`}
  flex-grow: 1;
  height: 100%;

  .custom-scrollbar {
    width: calc(100% + 24px) !important;
  }

  @media ${tablet} {
    height: ${(props) =>
      props.isDesktop ? "calc(100% - 104px)" : "calc(100% - 44px)"};
    display: table;
    width: calc(100% + 16px);

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
    ${(props) => isMobile && props.pinned && `margin-bottom: 56px`}
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
  @media ${tablet} {
    margin-bottom: 60px;
  }
`;

class ArticleBody extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout ArticleBody render");
    const { children, pinned, isDesktop } = this.props;

    return (
      <StyledArticleBody pinned={pinned} isDesktop={isDesktop}>
        <Scrollbar
          id="articleScrollBar"
          className="custom-scrollbar"
          stype="mediumBlack"
        >
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
