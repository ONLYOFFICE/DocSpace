import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import CrossIcon from '@appserver/components/public/static/images/cross.react.svg';

const StyledControlContainer = styled.div`
  background: #9a9ea3;
  width: 24px;
  height: 24px;
  position: absolute;
  top: 30px;
  right: 10px;
  border-radius: 100px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 290;
`;

const StyledCrossIcon = styled(CrossIcon)`
  width: 11.31px;
  height: 11.31px;
  path {
    fill: #ffffff;
  }
`;

const CatalogBackdrop = (props) => {
  const { showText, onClick, ...rest } = props;
  return (
    <StyledControlContainer onClick={onClick} {...rest}>
      <StyledCrossIcon />
    </StyledControlContainer>
  );
};

CatalogBackdrop.propTypes = {
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

export default React.memo(CatalogBackdrop);
