import { jsx } from '@emotion/core';
import 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';

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

var _jsxFileName = "D:\\GitHub\\CommunityServer-AspNetCore\\web\\ASC.Web.Components\\src\\components\\Button\\index.js";
function _templateObject4() {
  var data = _taggedTemplateLiteral(["\n            border-width: 1px;\n            border-style: solid;\n            border-color: #dadada;\n          "]);
  _templateObject4 = function _templateObject4() {
    return data;
  };
  return data;
}
function _templateObject3() {
  var data = _taggedTemplateLiteral(["\n      &:hover {\n        background-color: ", ";\n        color: ", ";\n      }\n      &:active {\n        background-color: ", ";\n        color: #ffffff;\n\n        ", "\n      }\n\n      \n\n    "]);
  _templateObject3 = function _templateObject3() {
    return data;
  };
  return data;
}
function _templateObject2() {
  var data = _taggedTemplateLiteral(["\n      border-width: 1px;\n      border-style: solid;\n      border-color: ", ";\n    "]);
  _templateObject2 = function _templateObject2() {
    return data;
  };
  return data;
}
function _templateObject() {
  var data = _taggedTemplateLiteral(["\n  /* Adapt the colors based on primary prop */\n  background-color: ", ";\n  color: ", ";\n  font-size: ", ";\n  line-height: ", ";\n\n  height: ", ";\n  padding: ", ";\n\n  border: none;\n  cursor: ", ";\n  display: inline-block;\n  font-family: 'Open Sans', sans-serif;\n  margin: 0;\n  font-weight: normal;\n  text-align: center;\n  text-decoration: none;\n  vertical-align: middle;\n  border-radius: 3px;\n  -moz-border-radius: 3px;\n  -webkit-border-radius: 3px;\n  touch-callout: none;\n  -o-touch-callout: none;\n  -moz-touch-callout: none;\n  -webkit-touch-callout: none;\n  user-select: none;\n  -o-user-select: none;\n  -moz-user-select: none;\n  -webkit-user-select: none;\n\n  ", "\n\n  ", "\n"]);
  _templateObject = function _templateObject() {
    return data;
  };
  return data;
}
var StyledButton = styled.button(_templateObject(), function (props) {
  return !props.disabled ? props.primary ? '#2da7db' : '#ebebeb' : props.primary ? '#a6dcf2' : '#f7f7f7';
}, function (props) {
  return props.primary ? '#ffffff' : !props.disabled ? '#666666' : '#999';
}, function (props) {
  return props.size === 'huge' || props.size === 'big' ? '15px' : '12px';
}, function (props) {
  return props.size === 'huge' ? '15px;' : props.size === 'big' ? '17px;' : '13px;';
}, function (props) {
  return props.size === 'huge' ? '40px' : props.size === 'big' ? '32px' : props.size === 'middle' ? '24px' : '21px';
}, function (props) {
  return props.size === 'huge' ? '12px 30px 13px;' : props.size === 'big' ? '7px 30px 8px;' : props.size === 'middle' ? '5px 24px 6px;' : '4px 13px;';
}, function (props) {
  return !props.disabled ? 'pointer' : 'default';
}, function (props) {
  return !props.primary && css(_templateObject2(), function (props) {
    return !props.disabled ? '#c4c4c4' : '#ebebeb';
  });
}, function (props) {
  return !props.disabled && css(_templateObject3(), function (props) {
    return props.primary ? '#3db8ec' : '#f5f5f5';
  }, function (props) {
    return props.primary ? '#ffffff' : '#666666';
  }, function (props) {
    return props.primary ? '#1f97ca' : '#e2e2e2';
  }, function (props) {
    return !props.primary && css(_templateObject4());
  });
});
var Button = function Button(props) {
  return jsx(StyledButton, _extends({}, props, {
    __source: {
      fileName: _jsxFileName,
      lineNumber: 86
    },
    __self: this
  }));
};
Button.PropTypes = {
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
  primary: PropTypes.bool,
  disabled: PropTypes.bool,
  onClick: PropTypes.func.isRequired
};
Button.defaultProps = {
  primary: false,
  disabled: false,
  size: 'base'
};

var _jsxFileName$1 = "D:\\GitHub\\CommunityServer-AspNetCore\\web\\ASC.Web.Components\\src\\components\\Input\\index.js";
var Input = function Input(props) {
  return jsx("input", {
    value: props.value,
    __source: {
      fileName: _jsxFileName$1,
      lineNumber: 7
    },
    __self: this
  });
};
Input.PropTypes = {
  value: PropTypes.string
};

export { Button, Input };
