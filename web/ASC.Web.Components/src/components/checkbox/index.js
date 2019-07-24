import React from 'react'
import ReactDOMServer from 'react-dom/server'
import PropTypes from 'prop-types'
import styled from 'styled-components'
import { Icons } from '../icons'
import { getCssFromSvg } from '../icons/get-css-from-svg'

const activeColor = '#333333',
  disableColor = '#A3A9AE';

var checkboxIcon,
    checkboxСheckedIcon,
    сheckboxDisabledIcon,
    сheckboxHoverIcon,
    сheckboxIndeterminateIcon,
    checkboxCheckedDisabledIcon,
    checkboxCheckedHoverIcon,
    checkboxIndeterminateDisabledIcon,
    checkboxIndeterminateHoverIcon;

(function(){
  checkboxIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.CheckboxIcon />));
  checkboxСheckedIcon= getCssFromSvg(ReactDOMServer.renderToString(<Icons.CheckboxCheckedIcon />));
  сheckboxDisabledIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.CheckboxDisabledIcon />));
  сheckboxHoverIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.CheckboxHoverIcon />));
  сheckboxIndeterminateIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.CheckboxIndeterminateIcon />));

  checkboxCheckedDisabledIcon= getCssFromSvg(ReactDOMServer.renderToString(<Icons.CheckboxCheckedDisabledIcon />));
  checkboxCheckedHoverIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.CheckboxCheckedHoverIcon />));
  checkboxIndeterminateDisabledIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.CheckboxIndeterminateDisabledIcon />));
  checkboxIndeterminateHoverIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.CheckboxIndeterminateHoverIcon />));
}());

const Label = styled.label`
  display: flex;
  align-items: center;
  position: relative;
  margin: 0;

  .checkbox {
    line-height: 16px;
    margin-right: 5px;
    margin-bottom: 2px;
    display: inline-block;
    vertical-align: middle;
    border: 0 none;
    cursor: pointer;
    outline: none;
    width: 16px;
    height: 16px;
    margin: 0 3px;
    background-repeat: no-repeat;
    background-image: url("data:image/svg+xml,${checkboxIcon}");

    &.checked {
      background-image: url("data:image/svg+xml,${checkboxСheckedIcon}");
    }
    &.indeterminate {
      background-image: url("data:image/svg+xml,${сheckboxIndeterminateIcon}");
    }
  }

  ${props => props.isDisabled
    ? `
        cursor: not-allowed;

        .checkbox {
          background-image: url("data:image/svg+xml,${сheckboxDisabledIcon}");
        }
        .checkbox.checked {
          background-image: url("data:image/svg+xml,${checkboxCheckedDisabledIcon}");
        }
        .checkbox.indeterminate {
          background-image: url("data:image/svg+xml,${checkboxIndeterminateDisabledIcon}");
        }
      `
    : `
        cursor: pointer;

        &:hover {
          .checkbox {
            background-image: url("data:image/svg+xml,${сheckboxHoverIcon}");
          }
          .checkbox.checked {
            background-image: url("data:image/svg+xml,${checkboxCheckedHoverIcon}");
          }
          .checkbox.indeterminate {
            background-image: url("data:image/svg+xml,${checkboxIndeterminateHoverIcon}");
          }
        }
      `
    }
`;

const Input = styled.input`
  opacity: 0.0001;
  position: absolute;
  right: 0;
`;

const Text = styled.span`
  margin-left: 8px;
  color: ${props => props.isDisabled ? disableColor : activeColor};
`;

class Checkbox extends React.Component  {

  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      checked: props.isChecked
    };
  }

  componentDidMount() {
    this.ref.current.indeterminate = this.props.isIndeterminate;
  }

  componentDidUpdate(prevProps) {
    if(this.props.isIndeterminate !== prevProps.isIndeterminate) {
      this.ref.current.indeterminate = this.props.isIndeterminate;
    }
    if(this.props.isChecked !== prevProps.isChecked) {
      this.setState({checked: this.props.isChecked});
    }
  }
  render() {
    const cbxClassName = 'checkbox' +
      (this.props.isIndeterminate ? ' indeterminate' : this.state.checked ? ' checked' : '') +
      (this.props.isDisabled ? ' disabled' : '');

    return (
    <Label htmlFor={this.props.id} isDisabled={this.props.isDisabled} >
      <Input type='checkbox' checked={this.state.checked} disabled={this.props.isDisabled} ref={this.ref} {...this.props} onChange={(e) => {
        this.setState({checked: e.target.checked});
        this.props.onChange && this.props.onChange(e);
      }}/>
      <span className={cbxClassName} />
      {
        this.props.label && <Text isDisabled={this.props.isDisabled}>{this.props.label}</Text>
      }
    </Label>
    );
  };
};


Checkbox.propTypes = {
  id: PropTypes.string,
  name: PropTypes.string,
  value: PropTypes.string,
  label: PropTypes.string,

  isChecked: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  isDisabled: PropTypes.bool,

  onChange: PropTypes.func,
};

Checkbox.defaultProps = {
  isChecked: false
};

export default Checkbox
