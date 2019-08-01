import React, { Component } from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import { Icons } from '../icons';
import { Text } from "../text";

const ToggleContainer = styled.label`
    position: absolute;
    -webkit-appearance: none;
    margin: 0;
    display: flex;
    align-items: center;
    outline: none;

    user-select: none;
    -moz-user-select: none;
    -o-user-select: none;
    -webkit-user-select: none;

    cursor: ${props => props.isDisabled ? 'default !important' : 'pointer'}
    svg {
        margin-right: 8px; 
        ${props => props.isDisabled ? 'rect { fill: #ECEFF1}' : ''}
    }
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
        this.props.hasOwnProperty("onChange") && this.props.onChange(e);
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
                {this.props.label && (
                    <Text.Body
                        tag="span"
                        isDisabled={this.props.isDisabled}
                        disableColor={'#A3A9AE'}
                    >
                        {this.props.label}
                    </Text.Body>
                )}
            </ToggleContainer>
        )
    }
}

ToggleButton.propTypes = {
    isChecked: PropTypes.bool.isRequired,
    isDisabled: PropTypes.bool,
    onChange: PropTypes.func,
    label: PropTypes.label
};

export default ToggleButton;