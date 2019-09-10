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

const RowContainer = styled.div`
  width: 100%;
  display: inline-flex;

  @media ${tablet} {
    display: block;
  }
`;

const MainContainerWrapper = styled.div`
  ${commonCss};

  display: flex;
  align-self: center;
  margin-right: auto;
  min-width: 140px;

  @media ${tablet} {
    min-width: 140px;
    margin-right: 8px;
    margin-top: 6px;
  }
`;

const MainContainer = styled.div`
  ${truncateCss};
  height: 20px;
  margin-right: 8px;
`;

const MainIcons = styled.div`
  align-self: center;
  white-space: nowrap;
 `;

const SideContainerWrapper = styled.div`
  ${truncateCss};
  ${commonCss};

  align-self: center;
  width: ${props => props.containerWidth ? props.containerWidth : '100px'};
  color: ${props => props.color && props.color};

  @media ${tablet} {
    display: none;
  }
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
  const { children } = props;

  const sideInfo = getSideInfo(children);

  return (
    <RowContainer>
      <MainContainerWrapper>
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
            <SideContainerWrapper key={'side-' + index} containerWidth={element.props.containerWidth} >
              {element}
            </SideContainerWrapper>
          );
        }
      })}
      <TabletSideInfo >
        {sideInfo}
      </TabletSideInfo>
    </RowContainer>
  )
};

RowContent.propTypes = {
  children: PropTypes.node.isRequired,
  containerWidth: PropTypes.string
};

export default RowContent;