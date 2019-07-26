import React from 'react';
import PropTypes from 'prop-types';
import RadioButton from './radio-button';
import styled from 'styled-components';

const StyledDiv = styled.div`
  display: flex;
`;

class RadioButtonGroup extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      selectedOption: this.props.selected

    };
  }


  handleOptionChange = changeEvent => {
    this.setState({
      selectedOption: changeEvent.target.value
    });
  };

  componentDidUpdate(prevProps) {
    if (this.props.selected !== prevProps.selected) {
      this.setState({ selectedOption: this.props.selected });
    }
  };

  render() {
    const options = this.props.options;
    return (
      <StyledDiv>
        {options.map(option => {
          return (
            <RadioButton
              key={option.value}
              name={this.props.name}
              value={option.value}
              checked={this.state.selectedOption === option.value}
              onChange={(e) => {
                this.handleOptionChange(e);
                this.props.onClick && this.props.onClick(e);
              }}

              disabled={this.props.isDisabled || option.disabled}
              label={option.label}
              spacing={this.props.spacing}
            />
          )
        }
        )
        }
      </StyledDiv>
    );
  };
};

RadioButtonGroup.propTypes = {
  isDisabled: PropTypes.bool,
  name: PropTypes.string.isRequired,
  onClick: PropTypes.func,
  options: PropTypes.arrayOf(PropTypes.shape({
    value: PropTypes.string.isRequired,
    label: PropTypes.string,
    disabled: PropTypes.bool
  })).isRequired,
  selected: PropTypes.string.isRequired,
  spacing: PropTypes.number
}

RadioButtonGroup.defaultProps = {
  isDisabled: false,
  selected: undefined,
  spacing: 33
}

export default RadioButtonGroup;
