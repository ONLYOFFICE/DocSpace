import React from "react";
import PropTypes from "prop-types";
import {
  StyledContainer,
  StyledRectangleLoader,
} from "./StyledPeopleCatalogLoader";
import { inject, observer } from "mobx-react";

const PeopleCatalogLoader = ({ id, className, style, showText, ...rest }) => {
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

PeopleCatalogLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  showText: PropTypes.bool,
};

PeopleCatalogLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default inject(({ auth }) => ({
  showText: auth.settingsStore.showText,
}))(observer(PeopleCatalogLoader));
