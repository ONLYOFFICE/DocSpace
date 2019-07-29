import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import DropDownItem from '../drop-down-item'

const StyledDropdown = styled.div`
    font-family: Open Sans;
    font-style: normal;
    font-weight: 600;
    font-size: 13px;

    position: absolute;
    ${props => props.manualWidth && `width: ${props.manualWidth};`}
    ${props => (props.directionY === 'top' && css`bottom: ${props => props.manualY ? props.manualY : '100%'};`)}
    ${props => (props.directionY === 'bottom' && css`top: ${props => props.manualY ? props.manualY : '100%'};`)}
    ${props => (props.directionX === 'right' && css`right: 0px;`)}
    ${props => (props.directionX === 'left' && css`left: 0px;`)}
    z-index: 1000;
    margin-top: ${props => (props.isUserPreview ? '6px' : '0px')};
    margin-right: ${props => (props.isUserPreview ? '6px' : '0px')};
    display: ${props => (props.isOpen || props.opened ? 'block' : 'none')};
    background: #FFFFFF;
    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
`;

const Arrow = styled.div`
    position: absolute;
    top: -6px;
    ${props => (props.directionX === 'right' && css`right: 16px;`)}
    ${props => (props.directionX === 'left' && css`left: 16px;`)}
    width: 24px;
    height: 6px;
    background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M9.27954 1.12012C10.8122 -0.295972 13.1759 -0.295971 14.7086 1.12012L18.8406 4.93793C19.5796 5.62078 20.5489 6 21.5551 6H24H0H2.43299C3.4392 6 4.40845 5.62077 5.1475 4.93793L9.27954 1.12012Z' fill='%23206FA4'/%3E%3C/svg%3E");
`;

const DropDown = React.memo(props => {
    //console.log("DropDown render");

    return (
        <StyledDropdown {...props}>
        {props.withArrow && <Arrow directionX={props.directionX} />}
        {React.Children.map(props.children, (child) => 
            <DropDownItem {...child.props}/>
        )}
        </StyledDropdown>
    );
});

DropDown.propTypes = {
    directionX: PropTypes.oneOf(['left', 'right']),
    directionY: PropTypes.oneOf(['bottom', 'top']),
    withArrow: PropTypes.bool,
    manualWidth: PropTypes.string,
    manualY: PropTypes.string
};

DropDown.defaultProps = {
    directionX: 'left',
    directionY: 'bottom',
    withArrow: false
};

export default DropDown