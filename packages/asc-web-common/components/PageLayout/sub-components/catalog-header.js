import React from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import Heading from '@appserver/components/heading';
import { isMobileOnly, isTablet } from 'react-device-detect';
import MenuIcon from '@appserver/components/public/static/images/menu.react.svg';
import { tablet, mobile } from '@appserver/components/utils/device';

const StyledCatalogHeader = styled.div`
  padding: 12px 20px 13px;
  display: flex;
  justify-content: flex-start;
  align-items: center;

  @media ${tablet} {
    padding: 16px 16px 17px;
    margin: 0;
    justify-content: ${(props) => (props.showText ? 'flex-start' : 'center')};
  }

  @media ${mobile} {
    border-bottom: 1px solid #eceef1;
    padding: 8px 14px 17px;
    margin: 0;
  }

  ${isTablet &&
  css`
    padding: 16px 16px 17px;
    justify-content: ${(props) => (props.showText ? 'flex-start' : 'center')};
    margin: 0;
  `}

  ${isMobileOnly &&
  css`
    border-bottom: 1px solid #eceef1 !important;
    padding: 8px 14px 17px !important;
    margin: 0;
  `}
`;

const StyledHeading = styled(Heading)`
  margin: 0;
  padding: 0;
  font-weight: bold;
  line-height: 28px;
  @media ${tablet} {
    display: ${(props) => (props.showText ? 'block' : 'none')};
    margin-left: ${(props) => props.showText && '12px'};
  }

  ${isTablet &&
  css`
    display: ${(props) => (props.showText ? 'block' : 'none')};
    margin-left: ${(props) => props.showText && '12px'};
  `}

  @media ${mobile} {
    margin-left: 0;
  }

  ${isMobileOnly &&
  css`
    margin-left: 0 !important;
  `}
`;

const StyledIconBox = styled.div`
  display: none;
  align-items: center;
  height: 28px;

  @media ${tablet} {
    display: flex;
  }

  @media ${mobile} {
    display: none;
  }

  ${isTablet &&
  css`
    display: flex !important;
  `}

  ${isMobileOnly &&
  css`
    display: none !important;
  `}
`;

const StyledMenuIcon = styled(MenuIcon)`
  display: block;
  width: 20px;
  height: 20px;
  fill: #657077;
  cursor: pointer;
`;

const CatalogHeader = (props) => {
  const { showText, children, onClick, ...rest } = props;

  return (
    <StyledCatalogHeader showText={showText} {...rest}>
      <StyledIconBox>
        <StyledMenuIcon onClick={onClick} />
      </StyledIconBox>

      <StyledHeading showText={showText} color="#333333" size="large">
        {children}
      </StyledHeading>
    </StyledCatalogHeader>
  );
};

CatalogHeader.propTypes = {
  children: PropTypes.any,
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

CatalogHeader.displayName = 'CatalogHeader';

export default React.memo(CatalogHeader);
