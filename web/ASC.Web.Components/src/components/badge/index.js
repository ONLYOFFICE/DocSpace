import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';

const StyledBadge = styled.div`
  background-color: ${props => props.backgroundColor};
  color: ${props => props.color};
  font-size: ${props => props.fontSize};
  font-weight: ${props => props.fontWeight};
  border-radius: ${props => props.fontSize};
  padding: 0 ${props => Math.floor(parseInt(props.fontSize)/2)}px;
  text-align: center;
  cursor: pointer;
  overflow: hidden;
  max-width: 50px;
  text-overflow: ellipsis;
  display: inline-block;
`;

const Badge = props => {
  return (
    props.number > 0
      ? <StyledBadge {...props}>{props.number}</StyledBadge>
      : ""
  );
};

Badge.propTypes = {
  number: PropTypes.number,
  backgroundColor: PropTypes.string,
  color: PropTypes.string,
  fontSize: PropTypes.string,
  fontWeight: PropTypes.number,
  onClick: PropTypes.func
};

Badge.defaultProps = {
  number: 0,
  backgroundColor: '#ED7309',
  color: '#FFFFFF',
  fontSize: '11px',
  fontWeight: 800
}

export default Badge;