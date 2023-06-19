import React from "react";
import PropTypes from "prop-types";
import {
  StyledContainer,
  StyledBlock,
  StyledRectangleLoader,
} from "./StyledArticleFolderLoader";
import { inject, observer } from "mobx-react";

const ArticleFolderLoader = ({
  id,
  className,
  style,
  showText,
  isVisitor,
  ...rest
}) => {
  return (
    <StyledContainer
      id={id}
      className={className}
      style={style}
      showText={showText}
    >
      {isVisitor ? (
        <>
          <StyledBlock showText={showText}>
            <StyledRectangleLoader
              {...rest}
              className="article-folder-loader"
            />
            <StyledRectangleLoader
              {...rest}
              className="article-folder-loader"
            />
          </StyledBlock>

          <StyledBlock showText={showText}>
            <StyledRectangleLoader
              {...rest}
              className="article-folder-loader"
            />
          </StyledBlock>
        </>
      ) : (
        <>
          <StyledBlock showText={showText}>
            <StyledRectangleLoader
              {...rest}
              className="article-folder-loader"
            />
            <StyledRectangleLoader
              {...rest}
              className="article-folder-loader"
            />
            <StyledRectangleLoader
              {...rest}
              className="article-folder-loader"
            />
          </StyledBlock>

          <StyledBlock showText={showText}>
            <StyledRectangleLoader
              {...rest}
              className="article-folder-loader"
            />
            <StyledRectangleLoader
              {...rest}
              className="article-folder-loader"
            />
            <StyledRectangleLoader
              {...rest}
              className="article-folder-loader"
            />
          </StyledBlock>
        </>
      )}
    </StyledContainer>
  );
};

ArticleFolderLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  showText: PropTypes.bool,
};

ArticleFolderLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default inject(({ auth }) => {
  return {
    showText: auth.settingsStore.showText,
    isVisitor: auth.userStore.user.isVisitor,
  };
})(observer(ArticleFolderLoader));
