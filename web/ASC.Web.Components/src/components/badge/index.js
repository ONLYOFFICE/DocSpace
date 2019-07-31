import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';

const StyledBadge = styled.div`
  background-color: ${props => props.backgroundColor};
  color: ${props => props.color};
  font-size: ${props => props.fontSize};
  font-weight: ${props => props.fontWeight};
  border-radius: ${props => props.borderRadius};
  padding: ${props => props.padding};
  max-width: ${props => props.maxWidth};
  text-align: center;
  cursor: pointer;
  overflow: hidden;
  text-overflow: ellipsis;
  display: ${props => props.number > 0 ? 'inline-block' : 'none'};
  user-select: none;
`;

const Badge = props => {
  //console.log("Badge render");

  const onClick = e => {
    if (props.onClick) {
      e.stopPropagation();
      props.onClick(e);
    }
  };

  return (<StyledBadge {...props} onClick={onClick}>{props.number}</StyledBadge>);
};

Badge.propTypes = {
  number: PropTypes.number,
  backgroundColor: PropTypes.string,
  color: PropTypes.string,
  fontSize: PropTypes.string,
  fontWeight: PropTypes.number,
  borderRadius: PropTypes.string,
  padding: PropTypes.string,
  maxWidth: PropTypes.string,
  onClick: PropTypes.func
};

Badge.defaultProps = {
  number: 0,
  backgroundColor: '#ED7309',
  color: '#FFFFFF',
  fontSize: '11px',
  fontWeight: 800,
  borderRadius: '11px',
  padding: '0 5px',
  maxWidth: '50px'
}

export default Badge;