import React from "react";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import { utils } from "asc-web-components";

const { tablet } = utils.device;

const StyledArticleMainButton = styled.div`
  margin: 16px 0 0;
  .main-button_drop-down {
    line-height: 36px;
  }
  @media ${tablet} {
    .main-button_drop-down {
      line-height: 40px;
    }
  }
`;

class ArticleMainButton extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout ArticleMainButton render");
    return <StyledArticleMainButton {...this.props} />;
  }
}

ArticleMainButton.displayName = "ArticleMainButton";

export default ArticleMainButton;
