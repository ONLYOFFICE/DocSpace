import React from "react";
import PropTypes from "prop-types";
import isEqual from "lodash/isEqual";
import TextInput from "../text-input";
import {
  EmailSettings,
  parseAddress
} from "../../utils/email/";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const TextInputWrapper = ({
  onValidateInput,
  isValidEmail,
  emailSettings,
  ...props
}) => <TextInput {...props}></TextInput>;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

class EmailInput extends React.Component {
  constructor(props) {
    super(props);

    const { value, emailSettings } = this.props;
    const validatedSettings = EmailSettings.parse(emailSettings);
    const isValidEmail = this.checkEmail(value, validatedSettings);

    this.state = {
      isValidEmail,
      emailSettings: validatedSettings,
      inputValue: value
    };
  }

  shouldComponentUpdate(nextProps, nextState) {
    return !isEqual(this.props, nextProps) || !isEqual(this.state, nextState);
  }

  componentDidUpdate(prevProps) {
    const { emailSettings, value } = this.props;

    if (!EmailSettings.equals(emailSettings, prevProps.emailSettings)) {
      const validatedSettings = EmailSettings.parse(emailSettings);

      this.setState({ emailSettings: validatedSettings }, () => {
        this.validate(this.state.inputValue);
      });
    }

    if (value !== prevProps.value) {
      this.validate(value);
    }
  }

  validate = (value) => {
    const { onValidateInput } = this.props;
    const isValidEmail = this.checkEmail(value);
    this.setState({
      inputValue: value,
      isValidEmail
    });
    onValidateInput && onValidateInput(isValidEmail);
  }

  checkEmail = (value, emailSettings = this.state.emailSettings) => {
    const { customValidate } = this.props;
    if (customValidate) {
      return customValidate(value);
    } else {
      const emailObj = parseAddress(value, emailSettings);
      const isValidEmail = emailObj.isValid();
      const parsedErrors = emailObj.parseErrors;
      const errors = parsedErrors
        ? parsedErrors.map(error => error.errorKey)
        : [];
      return {
        value,
        isValid: isValidEmail,
        errors
      };
    }
  };

  onChange = e => {
    const { onChange, onValidateInput } = this.props;
    onChange ? onChange(e) : this.setState({ inputValue: e.target.value });

    const isValidEmail = this.checkEmail(e.target.value);
    this.setState({ isValidEmail });

    onValidateInput && onValidateInput(isValidEmail);
  };

  render() {
    //console.log('EmailInput render()');
    // eslint-disable-next-line no-unused-vars
    const {
      onValidateInput,
      hasError
    } = this.props;

    const { isValidEmail, inputValue } = this.state;
    const isError =
      typeof hasError === "boolean"
        ? hasError
        : Boolean(inputValue && !isValidEmail.isValid);

    return (
      <TextInputWrapper
        {...this.props}
        hasError={isError}
        value={inputValue}
        onChange={this.onChange}
        type="text"
        onValidateInput={onValidateInput}
      />
    );
  }
}

EmailInput.propTypes = {
  className: PropTypes.string,
  customValidate: PropTypes.func,
  emailSettings: PropTypes.oneOfType([
    PropTypes.instanceOf(EmailSettings),
    PropTypes.objectOf(PropTypes.bool)
  ]),
  hasError: PropTypes.bool,
  id: PropTypes.string,
  onChange: PropTypes.func,
  onValidateInput: PropTypes.func,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  value: PropTypes.string
};

EmailInput.defaultProps = {
  autoComplete: "email",
  className: "",
  hasError: undefined,
  id: "",
  isDisabled: false,
  isReadOnly: false,
  maxLength: 255,
  name: "",
  placeholder: "",
  scale: false,
  size: "base",
  title: "",
  value: "",
  withBorder: true,

  emailSettings: new EmailSettings()
};

export default EmailInput;
