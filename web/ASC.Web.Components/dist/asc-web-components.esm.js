import { jsx } from '@emotion/core';
import { useState } from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Row, Container, Col, Collapse, Navbar } from 'reactstrap';

function _defineProperty(obj, key, value) {
  if (key in obj) {
    Object.defineProperty(obj, key, {
      value: value,
      enumerable: true,
      configurable: true,
      writable: true
    });
  } else {
    obj[key] = value;
  }

  return obj;
}

function _extends() {
  _extends = Object.assign || function (target) {
    for (var i = 1; i < arguments.length; i++) {
      var source = arguments[i];

      for (var key in source) {
        if (Object.prototype.hasOwnProperty.call(source, key)) {
          target[key] = source[key];
        }
      }
    }

    return target;
  };

  return _extends.apply(this, arguments);
}

function _taggedTemplateLiteral(strings, raw) {
  if (!raw) {
    raw = strings.slice(0);
  }

  return Object.freeze(Object.defineProperties(strings, {
    raw: {
      value: Object.freeze(raw)
    }
  }));
}

function _slicedToArray(arr, i) {
  return _arrayWithHoles(arr) || _iterableToArrayLimit(arr, i) || _nonIterableRest();
}

function _arrayWithHoles(arr) {
  if (Array.isArray(arr)) return arr;
}

function _iterableToArrayLimit(arr, i) {
  var _arr = [];
  var _n = true;
  var _d = false;
  var _e = undefined;

  try {
    for (var _i = arr[Symbol.iterator](), _s; !(_n = (_s = _i.next()).done); _n = true) {
      _arr.push(_s.value);

      if (i && _arr.length === i) break;
    }
  } catch (err) {
    _d = true;
    _e = err;
  } finally {
    try {
      if (!_n && _i["return"] != null) _i["return"]();
    } finally {
      if (_d) throw _e;
    }
  }

  return _arr;
}

function _nonIterableRest() {
  throw new TypeError("Invalid attempt to destructure non-iterable instance");
}

var _jsxFileName = "D:\\GitHub\\CommunityServer-AspNetCore\\web\\ASC.Web.Components\\src\\components\\Button\\index.js";
function _templateObject7() {
  var data = _taggedTemplateLiteral(["\n    &:hover {\n      ", "\n    }"]);
  _templateObject7 = function _templateObject7() {
    return data;
  };
  return data;
}
function _templateObject6() {
  var data = _taggedTemplateLiteral(["\n    &:active {\n      ", "\n    }"]);
  _templateObject6 = function _templateObject6() {
    return data;
  };
  return data;
}
function _templateObject5() {
  var data = _taggedTemplateLiteral(["\n      border-width: 1px;\n      border-style: solid;\n      border-color: ", ";\n    "]);
  _templateObject5 = function _templateObject5() {
    return data;
  };
  return data;
}
function _templateObject4() {
  var data = _taggedTemplateLiteral(["\n  height: ", ";\n\n  line-height: ", ";\n\n  font-size: ", ";\n\n  color: ", ";\n\n  background-color: ", ";\n\n  padding: ", ";\n\n  cursor: ", ";\n\n  font-family: 'Open Sans', sans-serif;\n  border: none;\n  margin: 0;\n  display: inline-block;\n  font-weight: normal;\n  text-align: center;\n  text-decoration: none;\n  vertical-align: middle;\n  border-radius: 3px;\n  -moz-border-radius: 3px;\n  -webkit-border-radius: 3px;\n  touch-callout: none;\n  -o-touch-callout: none;\n  -moz-touch-callout: none;\n  -webkit-touch-callout: none;\n  user-select: none;\n  -o-user-select: none;\n  -moz-user-select: none;\n  -webkit-user-select: none;\n  stroke: none;\n\n  ", "\n  \n  ", "\n\n  ", "\n\n  &:focus {\n    outline: none\n  }\n"]);
  _templateObject4 = function _templateObject4() {
    return data;
  };
  return data;
}
function _templateObject3() {
  var data = _taggedTemplateLiteral(["\n  background-color: ", ";\n  color: ", ";\n"]);
  _templateObject3 = function _templateObject3() {
    return data;
  };
  return data;
}
function _templateObject2() {
  var data = _taggedTemplateLiteral(["\n      border-width: 1px;\n      border-style: solid;\n      border-color: #dadada;\n    "]);
  _templateObject2 = function _templateObject2() {
    return data;
  };
  return data;
}
function _templateObject() {
  var data = _taggedTemplateLiteral(["\n  background-color: ", ";\n  color: #ffffff;\n\n  ", "\n\n"]);
  _templateObject = function _templateObject() {
    return data;
  };
  return data;
}
var activatedCss = css(_templateObject(), function (props) {
  return props.primary ? '#1f97ca' : '#e2e2e2';
}, function (props) {
  return !props.primary && css(_templateObject2());
});
var hoveredCss = css(_templateObject3(), function (props) {
  return props.primary ? '#3db8ec' : '#f5f5f5';
}, function (props) {
  return props.primary ? '#ffffff' : '#666666';
});
var StyledButton = styled.button.attrs(function (props) {
  return {
    disabled: props.isDisabled ? 'disabled' : '',
    tabIndex: props.tabIndex
  };
})(_templateObject4(), function (props) {
  return props.size === 'huge' && '40px' || props.size === 'big' && '32px' || props.size === 'middle' && '24px' || props.size === 'base' && '21px';
}, function (props) {
  return props.size === 'huge' && '15px' || props.size === 'big' && '17px' || '13px';
}, function (props) {
  return (props.size === 'huge' || props.size === 'big') && '15px' || (props.size === 'middle' || props.size === 'base') && '12px';
}, function (props) {
  return props.primary && '#ffffff' || (!props.isDisabled ? '#666666' : '#999');
}, function (props) {
  return !props.isDisabled ? props.primary ? '#2da7db' : '#ebebeb' : props.primary ? '#a6dcf2' : '#f7f7f7';
}, function (props) {
  return props.size === 'huge' && (props.primary ? '12px 30px 13px' : '11px 30px 12px') || props.size === 'big' && (props.primary ? '7px 30px 8px' : '6px 30px 7px') || props.size === 'middle' && (props.primary ? '5px 24px 6px' : '4px 24px 5px') || props.size === 'base' && (props.primary ? '4px 13px' : '3px 12px');
}, function (props) {
  return props.isDisabled ? 'default !important' : 'pointer';
}, function (props) {
  return !props.primary && css(_templateObject5(), function (props) {
    return !props.isDisabled ? '#c4c4c4' : '#ebebeb';
  });
}, function (props) {
  return !props.isDisabled && (props.isActivated ? "".concat(activatedCss) : css(_templateObject6(), activatedCss));
}, function (props) {
  return !props.isDisabled && (props.isHovered ? "".concat(hoveredCss) : css(_templateObject7(), hoveredCss));
});
var Button = function Button(props) {
  return jsx(StyledButton, _extends({}, props, {
    __source: {
      fileName: _jsxFileName,
      lineNumber: 102
    },
    __self: this
  }));
};
Button.propTypes = {
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
  primary: PropTypes.bool,
  tabIndex: PropTypes.number,
  isActivated: PropTypes.bool,
  isHovered: PropTypes.bool,
  isDisabled: PropTypes.bool,
  onClick: PropTypes.func.isRequired
};
Button.defaultProps = {
  primary: false,
  isActivated: false,
  isHovered: false,
  isDisabled: false,
  size: 'base',
  tabIndex: -1
};

var _jsxFileName$1 = "D:\\GitHub\\CommunityServer-AspNetCore\\web\\ASC.Web.Components\\src\\components\\TextInput\\index.js";
function _templateObject$1() {
  var data = _taggedTemplateLiteral(["\n  -webkit-appearance: none;\n  border-radius: 3px;\n  box-shadow: none;\n  box-sizing: border-box;\n  border: solid 1px;\n  border-color: ", ";\n  -moz-border-radius: 3px;\n  -webkit-border-radius: 3px;\n  background-color: ", ";\n  color: ", "; ;\n  display: flex;\n  font-family: 'Open Sans', sans-serif;\n  font-size: 18px;  \n  flex: 1 1 0%;\n  outline: none;\n  overflow: hidden;\n  padding: 8px 20px;\n  transition: all 0.2s ease 0s;\n  width: ", ";\n\n    ::-webkit-input-placeholder {\n        color: #b2b2b2;\n        font-family: 'Open Sans',sans-serif\n    }\n\n    :-moz-placeholder {\n        color: #b2b2b2;\n        font-family: 'Open Sans',sans-serif\n    }\n\n    ::-moz-placeholder {\n        color: #b2b2b2;\n        font-family: 'Open Sans',sans-serif\n    }\n\n    :-ms-input-placeholder {\n        color: #b2b2b2;\n        font-family: 'Open Sans',sans-serif\n    }\n\n"]);
  _templateObject$1 = function _templateObject() {
    return data;
  };
  return data;
}
var StyledInput = styled.input.attrs(function (props) {
  return _defineProperty({
    id: props.id,
    name: props.name,
    type: props.type,
    value: props.value,
    placeholder: props.placeholder,
    maxLength: props.maxLength,
    onChange: props.onChange,
    onBlur: props.onBlur,
    onFocus: props.onFocus,
    disabled: props.isDisabled,
    readOnly: props.isReadOnly,
    autoFocus: props.isAutoFocussed,
    autoComplete: props.autoComplete,
    tabIndex: props.tabIndex
  }, "disabled", props.isDisabled ? 'disabled' : '');
})(_templateObject$1(), function (props) {
  return props.hasError && '#c30' || props.hasWarning && '#f1ca92' || '#c7c7c7';
}, function (props) {
  return props.isDisabled ? '#efefef' : '#fff';
}, function (props) {
  return props.isDisabled ? '#666562' : '#434341';
}, function (props) {
  return props.size === 'base' && '135px' || props.size === 'middle' && '300px' || props.size === 'big' && '350px' || props.size === 'huge' && '500px' || props.size === 'scale' && '100%';
});
var TextInput = function TextInput(props) {
  return jsx(StyledInput, _extends({}, props, {
    __source: {
      fileName: _jsxFileName$1,
      lineNumber: 71
    },
    __self: this
  }));
};
TextInput.propTypes = {
  id: PropTypes.string,
  name: PropTypes.string,
  type: PropTypes.oneOf(['text', 'password']),
  value: PropTypes.string.isRequired,
  maxLength: PropTypes.number,
  placeholder: PropTypes.string,
  tabIndex: PropTypes.number,
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge', 'scale']),
  onChange: PropTypes.func,
  onBlur: PropTypes.func,
  onFocus: PropTypes.func,
  isAutoFocussed: PropTypes.bool,
  isDisabled: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  hasError: PropTypes.bool,
  hasWarning: PropTypes.bool,
  autoComplete: PropTypes.string
};
TextInput.defaultProps = {
  type: 'text',
  value: '',
  maxLength: 255,
  size: 'middle',
  tabIndex: -1,
  hasError: false,
  hasWarning: false,
  autoComplete: 'off'
};

var _jsxFileName$2 = "D:\\GitHub\\CommunityServer-AspNetCore\\web\\ASC.Web.Components\\src\\components\\Forms\\Login\\index.js";
function _templateObject$2() {
  var data = _taggedTemplateLiteral(["\n    margin: 23px 0 0;\n"]);
  _templateObject$2 = function _templateObject() {
    return data;
  };
  return data;
}
var FormRow = styled(Row)(_templateObject$2());
var LoginForm = function LoginForm(props) {
  var loginPlaceholder = props.loginPlaceholder,
      passwordPlaceholder = props.passwordPlaceholder,
      buttonText = props.buttonText,
      onSubmit = props.onSubmit,
      errorText = props.errorText;
  var _useState = useState(''),
      _useState2 = _slicedToArray(_useState, 2),
      login = _useState2[0],
      setLogin = _useState2[1];
  var _useState3 = useState(true),
      _useState4 = _slicedToArray(_useState3, 2),
      loginValid = _useState4[0],
      setLoginValid = _useState4[1];
  var _useState5 = useState(''),
      _useState6 = _slicedToArray(_useState5, 2),
      password = _useState6[0],
      setPassword = _useState6[1];
  var _useState7 = useState(true),
      _useState8 = _slicedToArray(_useState7, 2),
      passwordValid = _useState8[0],
      setPasswordValid = _useState8[1];
  var validateAndSubmit = function validateAndSubmit(event) {
    if (!login.trim()) setLoginValid(false);
    if (!password.trim()) setPasswordValid(false);
    if (loginValid && passwordValid) return onSubmit(event, {
      login: login,
      password: password
    });
    return false;
  };
  return jsx(Container, {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 33
    },
    __self: this
  }, jsx(FormRow, {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 34
    },
    __self: this
  }, jsx(Col, {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 35
    },
    __self: this
  }, jsx(TextInput, {
    id: "login",
    name: "login",
    hasError: !loginValid,
    value: login,
    placeholder: loginPlaceholder,
    size: "big",
    isAutoFocussed: true,
    tabIndex: 1,
    onChange: function onChange(event) {
      setLogin(event.target.value);
      setLoginValid(true);
    },
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 36
    },
    __self: this
  }))), jsx(FormRow, {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 51
    },
    __self: this
  }, jsx(Col, {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 52
    },
    __self: this
  }, jsx(TextInput, {
    id: "password",
    name: "password",
    type: "password",
    hasError: !passwordValid,
    value: password,
    placeholder: passwordPlaceholder,
    size: "big",
    tabIndex: 2,
    onChange: function onChange(event) {
      setPassword(event.target.value);
      setPasswordValid(true);
    },
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 53
    },
    __self: this
  }))), jsx(FormRow, {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 68
    },
    __self: this
  }, jsx(Col, {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 69
    },
    __self: this
  }, jsx(Button, {
    primary: true,
    size: "big",
    tabIndex: 3,
    onClick: validateAndSubmit,
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 70
    },
    __self: this
  }, buttonText))), jsx(Collapse, {
    isOpen: errorText,
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 73
    },
    __self: this
  }, jsx(Row, {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 74
    },
    __self: this
  }, jsx(Col, {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 75
    },
    __self: this
  }, jsx("span", {
    __source: {
      fileName: _jsxFileName$2,
      lineNumber: 76
    },
    __self: this
  }, errorText)))));
};
LoginForm.propTypes = {
  loginPlaceholder: PropTypes.string.isRequired,
  passwordPlaceholder: PropTypes.string.isRequired,
  buttonText: PropTypes.string.isRequired,
  errorText: PropTypes.string,
  onSubmit: PropTypes.func.isRequired
};
LoginForm.defaultProps = {
  login: '',
  password: ''
};

var _jsxFileName$3 = "D:\\GitHub\\CommunityServer-AspNetCore\\web\\ASC.Web.Components\\src\\components\\Nav\\index.js";
function _templateObject$3() {
  var data = _taggedTemplateLiteral(["\n    background: #0f4071;\n    color: #c5c5c5;\n    height: 48px;\n    padding-top: 4px;\n    z-index: 1;\n"]);
  _templateObject$3 = function _templateObject() {
    return data;
  };
  return data;
}
var StyledNav = styled(Navbar)(_templateObject$3());
var Nav = function Nav(props) {
  var children = props.children;
  return jsx(StyledNav, {
    dark: true,
    __source: {
      fileName: _jsxFileName$3,
      lineNumber: 19
    },
    __self: this
  }, children);
};
Nav.propTypes = {
  text: PropTypes.string
};

export { Button, LoginForm, Nav, TextInput };
