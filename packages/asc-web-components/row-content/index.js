import React from "react";
import PropTypes from "prop-types";
import { isMobile } from "react-device-detect";

import {
  TabletSideInfo,
  SideContainerWrapper,
  MainContainer,
  MainIcons,
  MainContainerWrapper,
  StyledRowContent,
} from "./styled-row-content";

const getSideInfo = (content, convert) => {
  let info = "";
  let child = null;
  const lastIndex = content.length - 1;

  content.map((element, index) => {
    const delimiter = index === lastIndex ? "" : " | ";
    if (index > 1) {
      if (!convert && index === lastIndex) {
        child = element;
      } else {
        info +=
          element.props && element.props.children
            ? element.props.children + delimiter
            : "";
      }
    }
  });
  return (
    <>
      {info}
      {child}
    </>
  );
};

const RowContent = (props) => {
  const {
    children,
    disableSideInfo,
    id,
    className,
    style,
    sideColor,
    onClick,
    sectionWidth,
    convertSideInfo,
  } = props;

  const sideInfo = getSideInfo(children, convertSideInfo);
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
          className="row-content_tablet-side-info"
          color={sideColor}
          widthProp={sectionWidth}
          isMobile={isMobile}
          convertSideInfo={convertSideInfo}
        >
          {sideInfo}
        </TabletSideInfo>
      )}
    </StyledRowContent>
  );
};

RowContent.propTypes = {
  /** Components displayed inside RowContent */
  children: PropTypes.node.isRequired,
  /** Accepts class */
  className: PropTypes.string,
  /** If you do not need SideElements */
  disableSideInfo: PropTypes.bool,
  /** Accepts id */
  id: PropTypes.string,
  onClick: PropTypes.func,
  /** Need for change side information color */
  sideColor: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  sectionWidth: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  isMobile: PropTypes.bool,
  convertSideInfo: PropTypes.bool,
};

RowContent.defaultProps = {
  disableSideInfo: false,
  convertSideInfo: true,
};

export default RowContent;
