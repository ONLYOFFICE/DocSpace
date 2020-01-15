import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import Text from "../text";

const StyledBadge = styled.div`
  display: ${props => (props.label.length > 0 || props.label != '0') ? 'inline-block' : 'none'};
  border: 1px solid transparent;
  border-radius: ${props => props.borderRadius};
  width: fit-content;
  padding: 2px;
  line-height: 0.8;
  cursor: pointer;
  overflow: hidden;

  :hover {
    border-color: ${props => props.backgroundColor};
  }
`;

const StyledInner = styled.div`
  background-color: ${props => props.backgroundColor};
  border-radius: ${props => props.borderRadius};
  padding: ${props => props.padding};
  max-width: ${props => props.maxWidth};
  text-align: center;
  user-select: none;
  line-height: 1.5;
`;

const Badge = props => {
  //console.log("Badge render");

  const onClick = e => {
    if (!props.onClick) return;

    e.preventDefault();
    e.stopPropagation();
    props.onClick(e);
  };

  const {
    fontSize,
    color,
    fontWeight,
    backgroundColor,
    borderRadius,
    padding,
    maxWidth
  } = props;

  return (
    <StyledBadge
      {...props}
      onClick={onClick}
    >
      <StyledInner
        backgroundColor={backgroundColor}
        borderRadius={borderRadius}
        padding={padding}
        maxWidth={maxWidth}
      >
        <Text
          fontWeight={fontWeight}
          color={color}
          fontSize={fontSize}>
          {props.label}
        </Text>
      </StyledInner>
    </StyledBadge>
  );
};

Badge.propTypes = {
  label: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  backgroundColor: PropTypes.string,
  color: PropTypes.string,
  fontSize: PropTypes.string,
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
  label: 0,
  backgroundColor: '#ED7309',
  color: '#FFFFFF',
  fontSize: "11px",
  fontWeight: 800,
  borderRadius: '11px',
  padding: '0 5px',
  maxWidth: '50px'
}

export default Badge;