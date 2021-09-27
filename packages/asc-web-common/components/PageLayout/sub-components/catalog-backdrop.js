import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import Backdrop from '@appserver/components/backdrop';
import { mobile } from '@appserver/components/utils/device';
import CrossIcon from '@appserver/components/public/static/images/cross.react.svg';

const StyledBackdrop = styled(Backdrop)`
  display: none;
  width: 100vw;
  height: 64px;
  position: fixed;
  top: 0;
  left: 0;
  background: rgba(6, 22, 38, 0.15);
  backdrop-filter: blur(18px);
  cursor: initial;
  @media ${mobile} {
    display: block;
  }
`;

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
    <StyledBackdrop visible={showText} {...rest}>
      <StyledControlContainer onClick={onClick}>
        <StyledCrossIcon />
      </StyledControlContainer>
    </StyledBackdrop>
  );
};

CatalogBackdrop.propTypes = {
  showText: PropTypes.bool,
  onClick: PropTypes.func,
};

export default React.memo(CatalogBackdrop);
