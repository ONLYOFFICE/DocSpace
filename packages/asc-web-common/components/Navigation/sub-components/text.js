import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';

import ExpanderDownIcon from '@appserver/components/public/static/images/expander-down.react.svg';
import commonIconsStyles from '@appserver/components/utils/common-icons-style';

import Headline from '@appserver/common/components/Headline';

import { tablet } from '@appserver/components/utils/device';
import { isMobile } from 'react-device-detect';

const StyledTextContainer = styled.div`
  height: ${isMobile ? '21px !important' : '18px'};
  width: fit-content;
  position: relative;
  display: grid;
  grid-template-columns: auto 14px;

  align-items: center;
  ${(props) => !props.isRootFolder && 'cursor: pointer'};
  @media ${tablet} {
    height: 21px;
  }
`;

const StyledHeadline = styled(Headline)`
  width: 100%;
  font-weight: 700;
  font-size: ${isMobile ? '21px !important' : '18px'};
  line-height: ${isMobile ? '28px !important' : '24px'};
  @media ${tablet} {
    font-size: 21px;
    line-height: 28px;
  }
`;

const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  min-width: 8px !important;
  width: 8px !important;
  min-height: 18px !important;
  padding-left: 6px;

  ${commonIconsStyles};
`;

const StyledExpanderDownIconRotate = styled(ExpanderDownIcon)`
  min-width: 8px !important;
  width: 8px !important;
  min-height: 18px !important;
  padding-right: 6px;
  transform: rotate(-180deg);

  ${commonIconsStyles};
`;

const Text = ({ title, isRootFolder, isOpen, onClick, ...rest }) => {
  return (
    <StyledTextContainer isRootFolder={isRootFolder} onClick={onClick} {...rest}>
      <StyledHeadline type="content" truncate={true}>
        {title}
      </StyledHeadline>
      {!isRootFolder ? (
        isOpen ? (
          <StyledExpanderDownIconRotate />
        ) : (
          <StyledExpanderDownIcon />
        )
      ) : (
        <></>
      )}
    </StyledTextContainer>
  );
};

Text.propTypes = {
  title: PropTypes.string,
  isOpen: PropTypes.bool,
  isRootFolder: PropTypes.bool,
  onCLick: PropTypes.func,
};

export default React.memo(Text);
