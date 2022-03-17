import React from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";

import InputBlock from "../input-block";
import Link from "../link";
import Text from "../text";
import Tooltip from "../tooltip";

import {
  PasswordProgress,
  StyledInput,
  TooltipStyle,
  StyledTooltipContainer,
  StyledTooltipItem,
} from "./styled-password-input";

class PasswordInput extends React.Component {
  constructor(props) {
    super(props);

    const { inputValue, inputType, clipActionResource, emailInputName } = props;

    this.ref = React.createRef();
    this.refTooltip = React.createRef();

    this.state = {
      type: inputType,
      inputValue: inputValue,
      copyLabel: clipActionResource,
      disableCopyAction: emailInputName ? false : true,
      displayTooltip: false,
      validLength: false,
      validDigits: false,
      validCapital: false,
      validSpecial: false,
    };
  }

  hideTooltip = () => {
    this.hideTooltip && this.refTooltip.current.hideTooltip();
  };

  onBlur = (e) => {
    e.persist();
    this.hideTooltip();
    if (this.props.onBlur) this.props.onBlur(e);
  };

  onKeyDown = (e) => {
    e.persist();
    if (this.props.onKeyDown) this.props.onKeyDown(e);
  };

  changeInputType = () => {
    this.hideTooltip();
    const newType = this.state.type === "text" ? "password" : "text";

    this.setState({
      type: newType,
    });
  };

  testStrength = (value) => {
    const { generatorSpecial, passwordSettings } = this.props;
    const specSymbols = new RegExp("[" + generatorSpecial + "]");

    let capital;
    let digits;
    let special;

    passwordSettings.upperCase
      ? (capital = /[A-Z]/.test(value))
      : (capital = true);

    passwordSettings.digits ? (digits = /\d/.test(value)) : (digits = true);

    passwordSettings.specSymbols
      ? (special = specSymbols.test(value))
      : (special = true);

    return {
      digits: digits,
      capital: capital,
      special: special,
      length: value.trim().length >= passwordSettings.minLength,
    };
  };

  checkPassword = (value) => {
    const passwordValidation = this.testStrength(value);
    const progressScore =
      passwordValidation.digits &&
      passwordValidation.capital &&
      passwordValidation.special &&
      passwordValidation.length;

    this.props.onValidateInput &&
      this.props.onValidateInput(progressScore, passwordValidation);

    this.setState({
      inputValue: value,
      validLength: passwordValidation.length,
      validDigits: passwordValidation.digits,
      validCapital: passwordValidation.capital,
      validSpecial: passwordValidation.special,
    });
  };

  onChangeAction = (e) => {
    this.props.onChange && this.props.onChange(e);

    if (this.props.simpleView) {
      this.setState({
        inputValue: e.target.value,
      });
      return;
    }

    this.checkPassword(e.target.value);
  };

  onGeneratePassword = (e) => {
    if (this.props.isDisabled) return e.preventDefault();

    const newPassword = this.getNewPassword();

    if (this.state.type !== "text") {
      this.setState({
        type: "text",
      });
    }

    this.checkPassword(newPassword);
    this.props.onChange &&
      this.props.onChange({ target: { value: newPassword } });
  };

  getNewPassword = () => {
    const { passwordSettings, generatorSpecial } = this.props;

    const length = passwordSettings.minLength;
    const string = "abcdefghijklmnopqrstuvwxyz";
    const numeric = "0123456789";
    const special = generatorSpecial;

    let password = "";
    let character = "";

    while (password.length < length) {
      const a = Math.ceil(string.length * Math.random() * Math.random());
      const b = Math.ceil(numeric.length * Math.random() * Math.random());
      const c = Math.ceil(special.length * Math.random() * Math.random());

      let hold = string.charAt(a);

      if (passwordSettings.upperCase) {
        hold = password.length % 2 == 0 ? hold.toUpperCase() : hold;
      }

      character += hold;

      if (passwordSettings.digits) {
        character += numeric.charAt(b);
      }

      if (passwordSettings.specSymbols) {
        character += special.charAt(c);
      }

      password = character;
    }

    password = password
      .split("")
      .sort(() => 0.5 - Math.random())
      .join("");

    return password.substr(0, length);
  };

  copyToClipboard = (emailInputName) => {
    const {
      clipEmailResource,
      clipPasswordResource,
      clipActionResource,
      clipCopiedResource,
      isDisabled,
      onCopyToClipboard,
    } = this.props;
    const { disableCopyAction, inputValue } = this.state;

    if (isDisabled || disableCopyAction) return event.preventDefault();

    this.setState({
      disableCopyAction: true,
      copyLabel: clipCopiedResource,
    });

    const textField = document.createElement("textarea");
    const emailValue = document.getElementsByName(emailInputName)[0].value;
    const formattedText =
      clipEmailResource +
      emailValue +
      " | " +
      clipPasswordResource +
      inputValue;

    textField.innerText = formattedText;
    document.body.appendChild(textField);
    textField.select();
    document.execCommand("copy");
    textField.remove();

    onCopyToClipboard && onCopyToClipboard(formattedText);

    setTimeout(() => {
      this.setState({
        disableCopyAction: false,
        copyLabel: clipActionResource,
      });
    }, 2000);
  };

  shouldComponentUpdate(nextProps, nextState) {
    return !equal(this.props, nextProps) || !equal(this.state, nextState);
  }

  componentDidUpdate(prevProps, prevState) {
    if (
      prevProps.clipActionResource !== this.props.clipActionResource &&
      this.state.copyLabel !== this.props.clipCopiedResource
    ) {
      this.setState({ copyLabel: this.props.clipActionResource });
    }
  }

  renderTextTooltip = (settings, length, digits, capital, special) => {
    return (
      <>
        <div className="break"></div>
        <Text
          className="text-tooltip"
          fontSize="10px"
          color="#A3A9AE"
          as="span"
        >
          {settings.minLength ? length : null}{" "}
          {settings.digits ? `, ${digits}` : null}{" "}
          {settings.upperCase ? `, ${capital}` : null}{" "}
          {settings.specSymbols ? `, ${special}` : null}
        </Text>
        <div className="break"></div>
      </>
    );
  };

  renderTextTooltip = () => {
    const {
      tooltipPasswordLength,
      tooltipPasswordDigits,
      tooltipPasswordCapital,
      tooltipPasswordSpecial,
      passwordSettings,
      isTextTooltipVisible,
    } = this.props;
    return isTextTooltipVisible ? (
      <>
        <div className="break"></div>
        <Text
          className="text-tooltip"
          fontSize="10px"
          color="#A3A9AE"
          as="span"
        >
          {passwordSettings.minLength ? tooltipPasswordLength : null}{" "}
          {passwordSettings.digits ? `, ${tooltipPasswordDigits}` : null}{" "}
          {passwordSettings.upperCase ? `, ${tooltipPasswordCapital}` : null}{" "}
          {passwordSettings.specSymbols ? `, ${tooltipPasswordSpecial}` : null}
        </Text>
        <div className="break"></div>
      </>
    ) : null;
  };

  renderTooltipContent = () =>
    !this.props.isDisableTooltip && !this.props.isDisabled ? (
      <TooltipStyle>
        <StyledTooltipContainer
          forwardedAs="div"
          title={this.props.tooltipPasswordTitle}
        >
          {this.props.tooltipPasswordTitle}
          <StyledTooltipItem
            forwardedAs="div"
            title={this.props.tooltipPasswordLength}
            valid={this.state.validLength}
          >
            {this.props.tooltipPasswordLength}
          </StyledTooltipItem>
          {this.props.passwordSettings.digits && (
            <StyledTooltipItem
              forwardedAs="div"
              title={this.props.tooltipPasswordDigits}
              valid={this.state.validDigits}
            >
              {this.props.tooltipPasswordDigits}
            </StyledTooltipItem>
          )}
          {this.props.passwordSettings.upperCase && (
            <StyledTooltipItem
              forwardedAs="div"
              title={this.props.tooltipPasswordCapital}
              valid={this.state.validCapital}
            >
              {this.props.tooltipPasswordCapital}
            </StyledTooltipItem>
          )}
          {this.props.passwordSettings.specSymbols && (
            <StyledTooltipItem
              forwardedAs="div"
              title={this.props.tooltipPasswordSpecial}
              valid={this.state.validSpecial}
            >
              {this.props.tooltipPasswordSpecial}
            </StyledTooltipItem>
          )}

          {this.props.generatePasswordTitle && (
            <div className="generate-btn-container">
              <Link
                className="generate-btn"
                type="action"
                fontWeight="600"
                isHovered={true}
                onClick={this.onGeneratePassword}
              >
                {this.props.generatePasswordTitle}
              </Link>
            </div>
          )}
        </StyledTooltipContainer>
      </TooltipStyle>
    ) : null;

  renderInputGroup = () => {
    const {
      inputName,
      isDisabled,
      scale,
      size,
      hasError,
      hasWarning,
      placeholder,
      tabIndex,
      maxLength,
      id,
      autoComplete,
    } = this.props;

    const { type, inputValue } = this.state;
    const iconName =
      type === "password"
        ? "/static/images/eye.off.react.svg"
        : "/static/images/eye.react.svg";

    return (
      <>
        <InputBlock
          className="input-relative"
          id={id}
          name={inputName}
          hasError={hasError}
          isDisabled={isDisabled}
          iconName={iconName}
          value={inputValue}
          onIconClick={this.changeInputType}
          onChange={this.onChangeAction}
          scale={scale}
          size={size}
          type={type}
          iconSize={16}
          isIconFill={true}
          onBlur={this.onBlur}
          onKeyDown={this.onKeyDown}
          hasWarning={hasWarning}
          placeholder={placeholder}
          tabIndex={tabIndex}
          maxLength={maxLength}
          autoComplete={autoComplete}
        ></InputBlock>

        <Tooltip
          id="tooltipContent"
          effect="solid"
          place="top"
          offsetLeft={this.props.tooltipOffsetLeft}
          offsetTop={this.props.tooltipOffsetTop}
          reference={this.refTooltip}
        >
          {this.renderTooltipContent()}
        </Tooltip>
      </>
    );
  };
  render() {
    //console.log('PasswordInput render()');
    const {
      inputWidth,
      onValidateInput,
      className,
      style,
      simpleView,
      isDisabled,
    } = this.props;

    return (
      <StyledInput
        onValidateInput={onValidateInput}
        className={className}
        style={style}
      >
        {simpleView ? (
          <>
            {this.renderInputGroup()}
            {this.renderTextTooltip()}
          </>
        ) : (
          <>
            <div className="password-field-wrapper">
              <PasswordProgress
                inputWidth={inputWidth}
                data-for="tooltipContent"
                data-tip=""
                data-event="click"
                ref={this.ref}
                isDisabled={isDisabled}
              >
                {this.renderInputGroup()}
              </PasswordProgress>
            </div>
            {this.renderTextTooltip()}
          </>
        )}
      </StyledInput>
    );
  }
}

PasswordInput.propTypes = {
  /** Allows you to set the component id  */
  id: PropTypes.string,
  /** Allows you to set the component auto-complete  */
  autoComplete: PropTypes.string,
  /** It is necessary for correct display of values inside input */
  inputType: PropTypes.oneOf(["text", "password"]),
  /** Input name */
  inputName: PropTypes.string,
  /** Required to associate password field with email field */
  emailInputName: PropTypes.string,
  /** Input value */
  inputValue: PropTypes.string,
  /** Will be triggered whenever an PasswordInput typing  */
  onChange: PropTypes.func,
  onKeyDown: PropTypes.func,
  onBlur: PropTypes.func,
  /** If you need to set input width manually */
  inputWidth: PropTypes.string,
  hasError: PropTypes.bool,
  hasWarning: PropTypes.bool,
  placeholder: PropTypes.string,
  tabIndex: PropTypes.number,
  maxLength: PropTypes.number,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Set input disabled */
  isDisabled: PropTypes.bool,
  size: PropTypes.oneOf(["base", "middle", "big", "huge", "large"]),
  scale: PropTypes.bool,
  /** Allows to hide Tooltip */
  isDisableTooltip: PropTypes.bool,
  /** Allows to show text Tooltip */
  isTextTooltipVisible: PropTypes.bool,
  /** Translation of text for copying email data and password  */
  clipActionResource: PropTypes.string,
  /** Text translation email to copy */
  clipEmailResource: PropTypes.string,
  /** Text translation password to copy */
  clipPasswordResource: PropTypes.string,
  /** Text translation copy action to copy */
  clipCopiedResource: PropTypes.string,
  /** Text translation tooltip */
  tooltipPasswordTitle: PropTypes.string,
  /** Password text translation is long tooltip  */
  tooltipPasswordLength: PropTypes.string,
  /** Digit text translation tooltip */
  tooltipPasswordDigits: PropTypes.string,
  /** Capital text translation tooltip */
  tooltipPasswordCapital: PropTypes.string,
  /** Special text translation tooltip */
  tooltipPasswordSpecial: PropTypes.string,
  /** Set of special characters for password generator and validator */
  generatorSpecial: PropTypes.string,
  NewPasswordButtonVisible: PropTypes.bool,
  /** Set of settings for password generator and validator */
  passwordSettings: PropTypes.object,
  /** Will be triggered whenever an PasswordInput typing, return bool value */
  onValidateInput: PropTypes.func,
  /** Will be triggered if you press copy button, return formatted value */
  onCopyToClipboard: PropTypes.func,

  tooltipOffsetLeft: PropTypes.number,
  /** Set simple view of password input (without tooltips, password progress bar and several additional buttons (copy and generate password) */
  simpleView: PropTypes.bool,
  generatePasswordTitle: PropTypes.string,
};

PasswordInput.defaultProps = {
  inputType: "password",
  inputName: "passwordInput",
  autoComplete: "new-password",
  isDisabled: false,
  size: "base",
  scale: true,

  isDisableTooltip: false,
  isTextTooltipVisible: false,

  clipEmailResource: "E-mail ",
  clipPasswordResource: "Password ",
  clipCopiedResource: "Copied",

  generatorSpecial: "!@#$%^&*",
  className: "",
  tooltipOffsetLeft: 0,
  tooltipOffsetTop: -5,
  simpleView: false,
  passwordSettings: {
    minLength: 8,
    upperCase: false,
    digits: false,
    specSymbols: false,
  },
};

export default PasswordInput;
