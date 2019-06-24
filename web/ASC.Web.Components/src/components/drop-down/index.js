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
    top: 100%;
    left: 0;
    z-index: 1000;
    margin-top: 0px;
    display: ${props => (props.isOpen || props.opened ? 'block' : 'none')};
    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    background: #FFFFFF;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
`;

const DropDown = props => {
    return (
        <StyledDropdown {...props}>
        {React.Children.map(props.children, (child) => 
            <DropDownItem {...child.props}/>
        )}
        </StyledDropdown>
    );
};

export default DropDown