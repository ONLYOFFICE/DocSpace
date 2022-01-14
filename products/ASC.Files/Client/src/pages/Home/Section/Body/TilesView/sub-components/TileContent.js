import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";

const truncateCss = css`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const commonCss = css`
  margin: 0;
  font-family: "Open Sans";
  font-size: 12px;
  font-style: normal;
  font-weight: 600;
`;

const StyledTileContent = styled.div`
  width: 100%;
  display: inline-flex;
`;

const MainContainerWrapper = styled.div`
  ${commonCss};

  display: flex;
  align-self: center;
  margin-right: auto;
`;

const MainContainer = styled.div`
  height: 20px;

  @media (max-width: 1024px) {
    ${truncateCss};
  }
`;

const MainIcons = styled.div`
  align-self: center;
  white-space: nowrap;

  .badges {
    margin: 8px;
  }

  .additional-badges {
    position: absolute;
    top: 0;
    left: 0;
    display: flex;
    flex-direction: row;
    filter: drop-shadow(0px 12px 40px rgba(4, 15, 27, 0.12));

    .icons-group {
      margin-right: 4px;
      background: #ffffff;
      border-radius: 4px;
      padding: 8px;
      height: 16px;
      border: none; // removes transparent border on version badge
    }
  }
`;

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
        mainContainerWidth={
          children[0].props && children[0].props.containerWidth
        }
      >
        <MainContainer className="row-main-container">
          {children[0]}
        </MainContainer>
        <MainIcons className="main-icons">{children[1]}</MainIcons>
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
