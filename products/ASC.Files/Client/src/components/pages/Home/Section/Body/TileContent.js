import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";

const truncateCss = css`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const commonCss = css`
  margin: 0 8px;
  font-family: "Open Sans";
  font-size: 12px;
  font-style: normal;
  font-weight: 600;
`;

const StyledTileContent = styled.div`
  width: 100%;
  display: inline-flex;

  ${(props) =>
    !props.disableSideInfo &&
    `
    @media (max-width: 1024px) {
      display: block;
      height: 56px;
    }
  `};
`;

const MainContainerWrapper = styled.div`
  ${commonCss};

  display: flex;
  align-self: center;
  margin-right: auto;

  ${(props) =>
    !props.disableSideInfo &&
    `
    @media (max-width: 1024px) {
      margin-right: 8px;
      margin-top: 8px;
    }
  `};
`;

const MainContainer = styled.div`
  height: 20px;
  margin-right: 8px;

  @media (max-width: 1024px) {
    ${truncateCss};
  }
`;

const MainIcons = styled.div`
  align-self: center;
  white-space: nowrap;
`;

const SideContainerWrapper = styled.div`
  ${commonCss};

  @media (max-width: 1024px) {
    ${truncateCss};
  }

  align-self: center;
  align-items: center;

  > a {
    vertical-align: middle;
  }

  color: ${(props) => props.color && props.color};

  ${(props) =>
    !props.disableSideInfo &&
    `
    @media (max-width: 1024px) {
      display: none;
    }
  `};
`;

const TabletSideInfo = styled.div`
  display: none;

  @media (max-width: 1024px) {
    display: block;
    color: ${(props) => props.color && props.color};

    ${commonCss};
    ${truncateCss};
  }
`;

const getSideInfo = (content) => {
  let info = "";
  const lastIndex = content.length - 1;

  content.map((element, index) => {
    const delimiter = index === lastIndex ? "" : " | ";
    if (index > 1) {
      info +=
        element.props && element.props.children
          ? element.props.children + delimiter
          : "";
    }
  });

  return info;
};

const TileContent = (props) => {
  //console.log("TileContent render");
  const {
    children,
    disableSideInfo,
    id,
    className,
    style,
    sideColor,
    onClick,
  } = props;

  const sideInfo = getSideInfo(children);

  return (
    <StyledTileContent
      disableSideInfo={disableSideInfo}
      id={id}
      className={className}
      style={style}
      onClick={onClick}
    >
      <MainContainerWrapper
        disableSideInfo={disableSideInfo}
        mainContainerWidth={
          children[0].props && children[0].props.containerWidth
        }
      >
        <MainContainer className="rowMainContainer">
          {children[0]}
        </MainContainer>
        <MainIcons className="mainIcons">{children[1]}</MainIcons>
      </MainContainerWrapper>
      {children.map((element, index) => {
        if (index > 1) {
          return (
            <SideContainerWrapper
              disableSideInfo={disableSideInfo}
              key={"side-" + index}
              containerWidth={element.props && element.props.containerWidth}
              containerMinWidth={
                element.props && element.props.containerMinWidth
              }
            >
              {element}
            </SideContainerWrapper>
          );
        }
      })}
      {!disableSideInfo && (
        <TabletSideInfo color={sideColor}>{sideInfo}</TabletSideInfo>
      )}
    </StyledTileContent>
  );
};

TileContent.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string,
  disableSideInfo: PropTypes.bool,
  id: PropTypes.string,
  onClick: PropTypes.func,
  sideColor: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

TileContent.defaultProps = {
  disableSideInfo: false,
};

export default TileContent;
