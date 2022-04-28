import React from "react";
import PropTypes from "prop-types";
import {
  StyledContainer,
  StyledRectangleLoader,
} from "./StyledArticleGroupsLoader";
import { inject, observer } from "mobx-react";

const ArticleGroupsLoader = ({ id, className, style, showText, ...rest }) => {
  return (
    <StyledContainer
      id={id}
      className={className}
      style={style}
      showText={showText}
    >
      <StyledRectangleLoader {...rest} />
      <StyledRectangleLoader {...rest} />
      <StyledRectangleLoader {...rest} />
      <StyledRectangleLoader {...rest} />
      <StyledRectangleLoader {...rest} />
      <StyledRectangleLoader {...rest} />
      <StyledRectangleLoader {...rest} />
      <StyledRectangleLoader {...rest} />
    </StyledContainer>
  );
};

ArticleGroupsLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  showText: PropTypes.bool,
};

ArticleGroupsLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default inject(({ auth }) => ({
  showText: auth.settingsStore.showText,
}))(observer(ArticleGroupsLoader));
