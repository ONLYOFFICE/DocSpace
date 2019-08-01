import React, { Component } from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import { Icons } from '../icons'

const disableColor = "#ECEFF1";
const DisableCss = css`
    rect {
        fill: ${disableColor};
    }
`;

const ToggleContainer = styled.label`

    position: absolute;
    -webkit-appearance: none;
    outline: none;
    margin: 0;
    display: flex;
    align-items: center;

    user-select: none;
    -moz-user-select: none;
    -o-user-select: none;

    ${props => props.isDisabled ? 
    css`
        cursor: default !important;
        svg {
            ${DisableCss}
          }
    `  
    : css` 
        cursor: pointer;
        `
    };
`;

const HiddenInput = styled.input`
    opacity: 0.0001;
    position: absolute;
    right: 0;
    z-index: -1;
`;

const CheckboxIcon = ({ isChecked }) => {
   
    const iconName = isChecked ? "ToggleButtonCheckedIcon" : "ToggleButtonIcon";

    return <>{React.createElement(Icons[iconName])}</>;
};

class ToggleButton extends Component {
    constructor(props) {
        super(props);
        this.state = {
            checked: this.props.isChecked
        };
    }

    componentDidUpdate(prevProps) {
        if (this.props.isChecked !== prevProps.isChecked) {
            this.setState({ checked: this.props.isChecked });
        }
    }

    onInputChange = (e) => {
        this.setState({ checked: e.target.checked });
        this.props.onChange 
    }

    render() {
        return (
            <ToggleContainer isDisabled={this.props.isDisabled}>
                <HiddenInput
                    type="checkbox"
                    checked={this.state.checked}
                    disabled={this.props.isDisabled}
                    onChange={this.onInputChange}
                    {...this.props}
                />
                <CheckboxIcon {...this.props} />
            </ToggleContainer>
        )
    }
}

ToggleButton.propTypes = {
    value: PropTypes.string.isRequired,
    isChecked: PropTypes.bool.isRequired,
    isDisabled: PropTypes.bool,
    onChange: PropTypes.func
};

export default ToggleButton;