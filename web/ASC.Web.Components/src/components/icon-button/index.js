import React from "react";
import PropTypes from "prop-types";
import styled from 'styled-components';
import { Icons } from '../icons';

const StyledOuter = styled.div`
    width: ${props => props.size ? Math.abs(parseInt(props.size)) + "px" : "20px"};
    cursor: ${props => props.isDisabled ? 'default' : 'pointer'};
    line-height: 0;
`;

const IconButton = (props) =>  {
    const { color, isFill, iconName, size, isDisabled } = props;
    return (
        <StyledOuter 
            size={size} 
            isDisabled={isDisabled} 
            onClick={!props.isDisabled ? props.onClick : undefined}
            onMouseEnter={!props.isDisabled ? props.onMouseEnter : undefined}
            onMouseLeave={!props.isDisabled ? props.onMouseLeave : undefined}
            onMouseOver={!props.isDisabled ? props.onMouseOver : undefined}
            onMouseOut={!props.isDisabled ? props.onMouseOut : undefined}

        >
            {React.createElement(Icons[iconName], {size: "scale", color: color, isfill: isFill})}
        </StyledOuter>
    );
};

IconButton.propTypes = {
    color: PropTypes.string,
    size: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
    isFill: PropTypes.bool,
    isDisabled: PropTypes.bool,
    iconName: PropTypes.string.isRequired,
    onClick:PropTypes.func
};

IconButton.defaultProps = {
    color: "#d0d5da",
    size: 25,
    isFill: true,
    iconName: "AZSortingIcon",
    isDisabled: false
};

export default IconButton;