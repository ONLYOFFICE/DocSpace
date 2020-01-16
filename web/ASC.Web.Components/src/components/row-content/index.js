import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { tablet } from '../../utils/device';

const truncateCss = css`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const commonCss = css`
  margin: 0 8px;
  font-family: 'Open Sans';
  font-size: 12px;
  font-style: normal;
  font-weight: 600;
`;

const StyledRowContent = styled.div`
  width: 100%;
  display: inline-flex;

  ${props => !props.disableSideInfo && `
    @media ${tablet} {
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
  min-width: 140px;
  width: 48%;

  ${props => !props.disableSideInfo && `
    @media ${tablet} {
      min-width: 140px;
      margin-right: 8px;
      margin-top: 8px;
      width: 95%;
    }
  `};
`;

const MainContainer = styled.div`
  height: 20px;
  margin-right: 8px;
  max-width: 86%;

  @media ${tablet} {
    ${truncateCss};
    max-width: 100%;
  }

`;

const MainIcons = styled.div`
  align-self: center;
  white-space: nowrap;
 `;

const SideContainerWrapper = styled.div`
  ${commonCss};

  @media ${tablet} {
    ${truncateCss};
  }

  align-self: center;
  align-items: center;

  > a {
    vertical-align: middle;
  }

  width: ${props => props.containerWidth ? props.containerWidth : '100px'};
  color: ${props => props.color && props.color};

  ${props => !props.disableSideInfo && `
    @media ${tablet} {
      display: none;
    }
  `};
`;

const TabletSideInfo = styled.div`
  display: none;

  @media ${tablet} {
    display: block;
    min-width: 160px;
    margin: 0 8px;
    color: ${props => props.color && props.color};

    ${commonCss};
    ${truncateCss};
  }
`;

const getSideInfo = content => {
  let info = '';
  const lastIndex = content.length - 1;

  content.map((element, index) => {
    const delimiter = (index === lastIndex) ? '' : ' | ';
    if (index > 1) {
      info += (element.props && element.props.children)
        ? element.props.children + delimiter
        : '';
    }
  });

  return info;
};

const RowContent = props => {
  //console.log("RowContent render");
  const { children, disableSideInfo, id, className, style, sideColor } = props;

  const sideInfo = getSideInfo(children);

  return (
    <StyledRowContent
      disableSideInfo={disableSideInfo}
      id={id}
      className={className}
      style={style}>
      <MainContainerWrapper disableSideInfo={disableSideInfo}>
        <MainContainer>
          {children[0]}
        </MainContainer>
        <MainIcons>
          {children[1]}
        </MainIcons>
      </MainContainerWrapper>
      {children.map((element, index) => {
        if (index > 1) {
          return (
            <SideContainerWrapper disableSideInfo={disableSideInfo} key={'side-' + index} containerWidth={element.props && element.props.containerWidth} >
              {element}
            </SideContainerWrapper>
          );
        }
      })}
      {!disableSideInfo &&
        <TabletSideInfo color={sideColor}>
          {sideInfo}
        </TabletSideInfo>
      }
    </StyledRowContent>
  )
};

RowContent.propTypes = {
  children: PropTypes.node.isRequired,
  disableSideInfo: PropTypes.bool,
  sideColor: PropTypes.string,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

RowContent.defaultProps = {
  disableSideInfo: false
};

export default RowContent;