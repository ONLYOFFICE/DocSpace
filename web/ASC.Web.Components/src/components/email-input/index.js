import React from 'react'
import PropTypes from 'prop-types'
import isEqual from "lodash/isEqual";
import TextInput from '../text-input'
import { EmailSettings, parseAddress, convertEmailSettings } from '../../utils/email/';

// eslint-disable-next-line no-unused-vars, react/prop-types
const SimpleInput = ({ onValidateInput, isValidEmail, emailSettings, ...props }) => <TextInput {...props}></TextInput>;

class EmailInput extends React.Component {
  constructor(props) {
    super(props);

    const { value, emailSettings } = this.props;
    const validatedSettings = convertEmailSettings(emailSettings);
    const isValidEmail = this.checkEmail(value, validatedSettings);

    this.state = {
      isValidEmail,
      emailSettings: validatedSettings,
      inputValue: value
    }
  }

  componentDidUpdate(prevProps) {
    const { emailSettings, value, onValidateInput } = this.props;
    if (!EmailSettings.equals(this.props.emailSettings, prevProps.emailSettings)) {
      const validatedSettings = convertEmailSettings(emailSettings);

      this.setState({ emailSettings: validatedSettings }, function () {
        this.checkEmail(this.state.inputValue);
      });
    }

    if (prevProps.value !== value) {
      const isValidEmail = this.checkEmail(value);
      this.setState({
        inputValue: value,
        isValidEmail
      });
      onValidateInput && onValidateInput(isValidEmail);
    }

  }

  checkEmail = (value, emailSettings = this.state.emailSettings) => {
    const { customValidateFunc } = this.props;
    if (customValidateFunc) {
      customValidateFunc(value);
    }
    else {
      const emailObj = parseAddress(value, emailSettings);
      const isValidEmail = emailObj.isValid();
      const parsedErrors = emailObj.parseErrors;
      const errors = parsedErrors ? parsedErrors.map(error => error.errorKey) : [];
      return {
        isValid: isValidEmail,
        errors
      };
    }
  }

  onChange = (e) => {
    const { onChange, onValidateInput } = this.props;
    onChange ? onChange(e) : this.setState({ inputValue: e.target.value });

    const isValidEmail = this.checkEmail(e.target.value);
    this.setState({ isValidEmail });

    onValidateInput && onValidateInput(isValidEmail);
  }

  shouldComponentUpdate(nextProps, nextState) {
    return !isEqual(this.props, nextProps) || !isEqual(this.state, nextState);
  }

  render() {
    //console.log('EmailInput render()');
    // eslint-disable-next-line no-unused-vars
    const { onValidateInput, emailSettings, onChange, value, hasError, ...rest } = this.props;

    const { isValidEmail, inputValue } = this.state;
    const isError = Boolean(inputValue && !isValidEmail.isValid) || hasError;

    return (
      <SimpleInput
        hasError={isError}
        value={inputValue}
        onChange={this.onChange}
        type='text'
        onValidateInput={onValidateInput}
        {...rest}
      />
    );
  }
}

EmailInput.propTypes = {
  className: PropTypes.string,
  customValidateFunc: PropTypes.func,
  emailSettings: PropTypes.oneOfType([PropTypes.instanceOf(EmailSettings), PropTypes.objectOf(PropTypes.bool)]),
  hasError: PropTypes.bool,
  id: PropTypes.string,
  onChange: PropTypes.func,
  onValidateInput: PropTypes.func,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  value: PropTypes.string,
}

EmailInput.defaultProps = {
  autoComplete: 'email',
  className: '',
  hasError: false,
  id: '',
  isDisabled: false,
  isReadOnly: false,
  maxLength: 255,
  name: '',
  placeholder: '',
  scale: false,
  size: 'base',
  title: '',
  value: '',
  withBorder: true,

  emailSettings: new EmailSettings()
}

export default EmailInput;
