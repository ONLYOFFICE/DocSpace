import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import Text from "../text";

const StyledBadge = styled.div`
  background-color: ${props => props.backgroundColor};
  border-radius: ${props => props.borderRadius};
  padding: ${props => props.padding};
  max-width: ${props => props.maxWidth};
  text-align: center;
  cursor: pointer;
  overflow: hidden;
  text-overflow: ellipsis;
  display: ${props => props.number > 0 ? 'inline-block' : 'none'};
  user-select: none;
  line-height: 1.5;
`;

const Badge = props => {
  //console.log("Badge render");

  const onClick = e => {
    if (props.onClick) {
      e.stopPropagation();
      props.onClick(e);
    }
  };
  
  const { fontSize, color, fontWeight } = props;
  return (
    <StyledBadge {...props} onClick={onClick}>
      <Text fontWeight={fontWeight} color={color} fontSize={fontSize}>
        {props.number}
      </Text>
    </StyledBadge>
  );
};

Badge.propTypes = {
  number: PropTypes.number,
  backgroundColor: PropTypes.string,
  color: PropTypes.string,
  fontSize: PropTypes.number,
  fontWeight: PropTypes.number,
  borderRadius: PropTypes.string,
  padding: PropTypes.string,
  maxWidth: PropTypes.string,
  onClick: PropTypes.func,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

Badge.defaultProps = {
  number: 0,
  backgroundColor: '#ED7309',
  color: '#FFFFFF',
  fontSize: 11,
  fontWeight: 800,
  borderRadius: '11px',
  padding: '0 5px',
  maxWidth: '50px'
}

export default Badge;