import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { tablet, size } from "../../utils/device";

const truncateCss = css`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const commonCss = css`
  margin: 0 6px;
  font-family: "Open Sans";
  font-size: 12px;
  font-style: normal;
  font-weight: 600;
`;

const containerTabletStyle = css`
  display: block;
  height: 56px;
`;

const mainWrapperTabletStyle = css`
  min-width: 140px;
  margin-right: 8px;
  margin-top: 8px;
  width: 95%;
`;

const mainContainerTabletStyle = css`
  ${truncateCss};
  max-width: 100%;
`;

const sideInfoTabletStyle = css`
  display: block;
  min-width: 160px;
  margin: 0 6px;
  ${commonCss};
  color: ${(props) => props.color && props.color};
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const StyledRowContent = styled.div`
  width: 100%;
  display: inline-flex;

  ${(props) =>
    (!props.disableSideInfo &&
      props.widthProp &&
      props.widthProp < size.tablet) ||
    props.isMobile
      ? `${containerTabletStyle}`
      : `
      @media ${tablet} {
        ${containerTabletStyle}
      }
    `}
`;

const MainContainerWrapper = styled.div`
  ${commonCss};

  display: flex;
  align-self: center;
  margin-right: auto;

  width: ${(props) =>
    props.mainContainerWidth ? props.mainContainerWidth : "140px"};
  min-width: 140px;

  ${(props) =>
    (!props.disableSideInfo &&
      props.widthProp &&
      props.widthProp < size.tablet) ||
    props.isMobile
      ? `${mainWrapperTabletStyle}`
      : `
      @media ${tablet} {
        ${mainWrapperTabletStyle}
      }
    `}
`;

const MainContainer = styled.div`
  height: 20px;
  margin-right: 8px;
  max-width: 86%;

  ${(props) =>
    (props.widthProp && props.widthProp < size.tablet) || props.isMobile
      ? `${mainContainerTabletStyle}`
      : `
      @media ${tablet} {
        ${mainContainerTabletStyle}
      }
    `}
`;

const MainIcons = styled.div`
  height: 19px;
  align-self: center;
  white-space: nowrap;
`;

const SideContainerWrapper = styled.div`
  ${commonCss};

  ${(props) =>
    (props.widthProp && props.widthProp < size.tablet) || props.isMobile
      ? `${truncateCss}`
      : `
      @media ${tablet} {
        ${truncateCss}
      }
    `}

  align-self: center;
  align-items: center;

  > a {
    vertical-align: middle;
  }

  width: ${(props) => (props.containerWidth ? props.containerWidth : "40px")};
  min-width: ${(props) =>
    props.containerMinWidth ? props.containerMinWidth : "40px"};
  color: ${(props) => props.color && props.color};

  ${(props) =>
    (!props.disableSideInfo &&
      props.widthProp &&
      props.widthProp < size.tablet) ||
    props.isMobile
      ? `display: none;`
      : `
      @media ${tablet} {
        display: none;
      }
    `}
`;

const TabletSideInfo = styled.div`
  display: none;
  ${(props) => (props.color ? `color: ${props.color};` : null)}
  ${(props) =>
    (props.widthProp && props.widthProp < size.tablet) || props.isMobile
      ? `${sideInfoTabletStyle}`
      : `
      @media ${tablet} {
        ${sideInfoTabletStyle}
      }
    `}
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

const RowContent = (props) => {
  //console.log("RowContent render");
  const {
    children,
    disableSideInfo,
    id,
    className,
    style,
    sideColor,
    onClick,
    sectionWidth,
    isMobile,
  } = props;

  const sideInfo = getSideInfo(children);
  const mainContainerWidth =
    children[0].props && children[0].props.containerWidth;

  return (
    <StyledRowContent
      className={className}
      disableSideInfo={disableSideInfo}
      id={id}
      onClick={onClick}
      style={style}
      widthProp={sectionWidth}
      isMobile={isMobile}
    >
      <MainContainerWrapper
        disableSideInfo={disableSideInfo}
        mainContainerWidth={mainContainerWidth}
        widthProp={sectionWidth}
        isMobile={isMobile}
      >
        <MainContainer
          className="rowMainContainer"
          widthProp={sectionWidth}
          isMobile={isMobile}
        >
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
              widthProp={sectionWidth}
              isMobile={isMobile}
            >
              {element}
            </SideContainerWrapper>
          );
        }
      })}
      {!disableSideInfo && (
        <TabletSideInfo
          color={sideColor}
          widthProp={sectionWidth}
          isMobile={isMobile}
        >
          {sideInfo}
        </TabletSideInfo>
      )}
    </StyledRowContent>
  );
};

RowContent.propTypes = {
  children: PropTypes.node.isRequired,
  className: PropTypes.string,
  disableSideInfo: PropTypes.bool,
  id: PropTypes.string,
  onClick: PropTypes.func,
  sideColor: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  sectionWidth: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  isMobile: PropTypes.bool,
};

RowContent.defaultProps = {
  disableSideInfo: false,
};

export default RowContent;
