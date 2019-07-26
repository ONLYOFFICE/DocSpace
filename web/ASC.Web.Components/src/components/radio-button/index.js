import React from 'react';
import ReactDOMServer from 'react-dom/server';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import { Icons } from '../icons';
import { getCssFromSvg } from '../icons/get-css-from-svg';
import { Text } from '../text';

const activeColor = '#000000',
  disableColor = '#A3A9AE';

var radiobuttonIcon,
  radiobuttonHoveredIcon,
  radiobuttonСheckedIcon,
  radiobuttonCheckedHoveredIcon,
  radiobuttonDisabledIcon,
  radiobuttonDisabledCheckedIcon;

(function () {
  radiobuttonIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.RadiobuttonIcon />));
  radiobuttonHoveredIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.RadiobuttonHoverIcon />));
  radiobuttonСheckedIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.RadiobuttonCheckedIcon />));
  radiobuttonCheckedHoveredIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.RadiobuttonHoverCheckedIcon />));
  radiobuttonDisabledIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.RadiobuttonDisabledIcon />));
  radiobuttonDisabledCheckedIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.RadiobuttonDisabledCheckedIcon />));
}());

const Label = styled.label`
  display: flex;
  align-items: center;
  position: relative;
  margin: 0;
  user-select: none;

  .radiobutton {
    line-height: 16px;
    margin-bottom: 4px;
    display: inline-block;
    vertical-align: middle;
    border: 0 none;
    cursor: pointer;
    outline: none;
    width: 16px;
    height: 16px;
    background-repeat: no-repeat;
    background-image: url("data:image/svg+xml,${radiobuttonIcon}");

    &:hover {
      background-image: url("data:image/svg+xml,${radiobuttonHoveredIcon}");
    }

    &.checked {
      background-image: url("data:image/svg+xml,${radiobuttonСheckedIcon}");

        &:hover {
          background-image: url("data:image/svg+xml,${radiobuttonCheckedHoveredIcon}");
                }

        &.disabled {
          cursor: default;
          background-image: url("data:image/svg+xml,${radiobuttonDisabledCheckedIcon}");
                  }
    }

    &.disabled {
      cursor: default;
      background-image: url("data:image/svg+xml,${radiobuttonDisabledIcon}");
    }
  }
`;

const Input = styled.input`
  position: absolute;
  z-index: -1;
  opacity: 0.0001;
`;

const TextBody = ({ isDisabled, ...props }) => <Text.Body {...props} />;

const StyledText = styled(TextBody)`
  margin-left: 4px;
  color: ${props => props.isDisabled ? disableColor : activeColor};
`;

const StyledSpan = styled.span`
  &:not(:first-child) {
    margin-left: ${props => props.spacing}px;
  }
`;

class RadioButton extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      isChecked: this.props.isChecked

    };
  }

  componentDidUpdate(prevProps) {
    if (this.props.isChecked !== prevProps.isChecked) {
      this.setState({ isChecked: this.props.isChecked });
    }
  };


  render() {
    const rbtnClassName = 'radiobutton' +
      (this.state.isChecked ? ' checked' : '') +
      (this.props.isDisabled ? ' disabled' : '');

    return (
      <StyledSpan spacing={this.props.spacing}>
        <Label>
          <span>
            <Input type='radio'
              name={this.props.name}
              value={this.props.value}
              checked={this.props.isChecked}
              onChange={this.props.onChange ? this.props.onChange : (e) => {
                this.setState({ isChecked: true })
                this.props.onClick && this.props.onClick(e);
              }}
              disabled={this.props.isDisabled} />

            <span className={rbtnClassName} />
            <StyledText tag='span' isDisabled={this.props.isDisabled}>{this.props.label || this.props.value}</StyledText>
          </span>
        </Label>
      </StyledSpan>
    );
  };
};

RadioButton.propTypes = {
  isChecked: PropTypes.bool,
  isDisabled: PropTypes.bool,
  label: PropTypes.string,
  name: PropTypes.string.isRequired,
  onChange: PropTypes.func,
  onClick: PropTypes.func,
  value: PropTypes.string.isRequired,
}

RadioButton.defaultProps = {
  isChecked: false,
  isDisabled: false,
}

export default RadioButton;
