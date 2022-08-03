import React from "react";
import PropTypes from "prop-types";
import {
  StyledTileContent,
  MainContainerWrapper,
  MainContainer,
} from "../StyledTileView";

const TileContent = (props) => {
  const { children, id, className, style, onClick } = props;

  return (
    <StyledTileContent
      id={id}
      className={className}
      style={style}
      onClick={onClick}
    >
      <MainContainerWrapper
        mainContainerWidth={children.props && children.props.containerWidth}
      >
        <MainContainer className="row-main-container">{children}</MainContainer>
      </MainContainerWrapper>
    </StyledTileContent>
  );
};

TileContent.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string,
  id: PropTypes.string,
  onClick: PropTypes.func,
  sideColor: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

export default TileContent;
