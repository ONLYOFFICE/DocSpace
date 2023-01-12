import React from "react";
import PropTypes from "prop-types";
import {
  StyledContainer,
  StyledBlock,
  StyledRectangleLoader,
} from "./StyledArticleFolderLoader";
import { inject, observer } from "mobx-react";
import RectangleLoader from "../RectangleLoader";
const ArticleFolderLoader = ({
  id,
  className,
  style,
  showText,

  isVisitor,
  isPaymentAlertVisible,
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
          <StyledBlock>
            <StyledRectangleLoader {...rest} />
            <StyledRectangleLoader {...rest} />
          </StyledBlock>

          <StyledBlock>
            <StyledRectangleLoader {...rest} />
          </StyledBlock>
        </>
      ) : (
        <>
          <StyledBlock>
            <StyledRectangleLoader {...rest} />
            <StyledRectangleLoader {...rest} />
            <StyledRectangleLoader {...rest} />
          </StyledBlock>

          <StyledBlock>
            <StyledRectangleLoader {...rest} />
            <StyledRectangleLoader {...rest} />
            <StyledRectangleLoader {...rest} />
          </StyledBlock>

          {showText && isPaymentAlertVisible && (
            <RectangleLoader width="210px" height="88px" />
          )}
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
  const { settingsStore, userStore, isPaymentAlertVisible } = auth;

  const { showText } = settingsStore;
  const { user } = userStore;

  return {
    showText,
    isVisitor: user.isVisitor,
    isPaymentAlertVisible,
  };
})(observer(ArticleFolderLoader));
