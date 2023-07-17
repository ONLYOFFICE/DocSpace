import React, { useRef, useState, useEffect } from "react";
import PropTypes from "prop-types";
import {
  StyledTextarea,
  StyledScrollbar,
  StyledCopyIcon,
  CopyIconWrapper,
  Wrapper,
  Numeration,
} from "./styled-textarea";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import Toast from "@docspace/components/toast";
import toastr from "@docspace/components/toast/toastr";
import { isJSON, beautifyJSON } from "./utils";

import copy from "copy-to-clipboard";

// eslint-disable-next-line react/prop-types, no-unused-vars

const jsonify = (value, isJSONField) => {
  if (isJSONField && value && isJSON(value)) {
    return beautifyJSON(value);
  }
  return value;
};

const Textarea = ({
  className,
  id,
  isDisabled,
  isReadOnly,
  hasError,
  heightScale,
  maxLength,
  name,
  onChange,
  placeholder,
  style,
  tabIndex,
  value,
  fontSize,
  heightTextArea,
  color,
  theme,
  autoFocus,
  areaSelect,
  isJSONField,
  copyInfoText,
  enableCopy,
  hasNumeration,
  isFullHeight,
  classNameCopyIcon,
}) => {
  const areaRef = useRef(null);
  const [isError, setIsError] = useState(hasError);
  const modifiedValue = jsonify(value, isJSONField);

  const lineHeight = 1.5;
  const padding = 7;
  const numberOfLines = modifiedValue.split("\n").length;
  const textareaHeight = isFullHeight
    ? numberOfLines * fontSize * lineHeight + padding + 4
    : heightTextArea;

  const defaultPaddingLeft = 42;
  const numberOfDigits =
    String(numberOfLines).length - 2 > 0 ? String(numberOfLines).length : 0;
  const paddingLeftProp = hasNumeration
    ? fontSize < 13
      ? `${defaultPaddingLeft + numberOfDigits * 6}px`
      : `${((defaultPaddingLeft + numberOfDigits * 4) * fontSize) / 13}px`
    : "8px";

  const numerationValue = [];

  for (let i = 1; i <= numberOfLines; i++) {
    numerationValue.push(i);
  }

  function onTextareaClick() {
    areaRef.current.select();
  }

  useEffect(() => {
    hasError !== isError && setIsError(hasError);
  }, [hasError]);

  useEffect(() => {
    setIsError(isJSONField && (!value || !isJSON(value)));
  }, [isJSONField, value]);

  useEffect(() => {
    if (areaSelect && areaRef.current) {
      areaRef.current.select();
    }
  }, [areaSelect]);

  const WrappedStyledCopyIcon = ({ heightScale, isJSONField, ...props }) => (
    <StyledCopyIcon {...props} />
  );

  return (
    <Wrapper
      className="textarea-wrapper"
      isJSONField={isJSONField}
      onFocus={enableCopy ? onTextareaClick : undefined}
    >
      {isJSONField && (
        <CopyIconWrapper
          className={classNameCopyIcon}
          isJSONField={isJSONField}
          onClick={() => {
            copy(modifiedValue);
            toastr.success(copyInfoText);
          }}
        >
          <WrappedStyledCopyIcon heightScale={heightScale} />
        </CopyIconWrapper>
      )}
      <ColorTheme
        themeId={ThemeType.Textarea}
        className={className}
        style={style}
        stype="preMediumBlack"
        isDisabled={isDisabled}
        hasError={isError}
        heightScale={heightScale}
        heightTextArea={textareaHeight}
      >
        <Toast />

        {hasNumeration && (
          <Numeration fontSize={fontSize}>
            {numerationValue.join("\n")}
          </Numeration>
        )}

        <StyledTextarea
          id={id}
          paddingLeftProp={paddingLeftProp}
          isJSONField={isJSONField}
          enableCopy={enableCopy}
          placeholder={placeholder}
          onChange={(e) => onChange && onChange(e)}
          maxLength={maxLength}
          name={name}
          tabIndex={tabIndex}
          isDisabled={isDisabled}
          disabled={isDisabled}
          readOnly={isReadOnly}
          value={isJSONField ? modifiedValue : value}
          fontSize={fontSize}
          color={color}
          autoFocus={autoFocus}
          ref={areaRef}
        />
      </ColorTheme>
    </Wrapper>
  );
};

Textarea.propTypes = {
  /** Class name */
  className: PropTypes.string,
  /** Used as HTML `id` property  */
  id: PropTypes.string,
  /** Indicates that the field cannot be used */
  isDisabled: PropTypes.bool,
  /** Indicates that the field is displaying read-only content */
  isReadOnly: PropTypes.bool,
  /** Indicates the input field has an error  */
  hasError: PropTypes.bool,
  /** Indicates the input field has scale */
  heightScale: PropTypes.bool,
  /** Max value length */
  maxLength: PropTypes.number,
  /** Used as HTML `name` property  */
  name: PropTypes.string,
  /** Sets a callback function that allows handling the component's changing events */
  onChange: PropTypes.func,
  /** Placeholder for Textarea  */
  placeholder: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Used as HTML `tabindex` property */
  tabIndex: PropTypes.number,
  /** Textarea value */
  value: PropTypes.string,
  /** Font-size value */
  fontSize: PropTypes.number,
  /** Text-area height value */
  heightTextArea: PropTypes.number,
  /** Specifies the text color */
  color: PropTypes.string,
  /** Default input property */
  autoFocus: PropTypes.bool,
  /** Allows selecting the textarea */
  areaSelect: PropTypes.bool,
  /** Prettifies Json and adds lines numeration */
  isJSONField: PropTypes.bool,
  /** Indicates the text of toast/informational alarm */
  copyInfoText: PropTypes.string,
  /** Shows copy icon */
  enableCopy: PropTypes.bool,
  /** Inserts numeration */
  hasNumeration: PropTypes.bool,
  /** Calculating height of content depending on number of lines */
  isFullHeight: PropTypes.bool,
};

Textarea.defaultProps = {
  isDisabled: false,
  isReadOnly: false,
  hasError: false,
  heightScale: false,
  placeholder: "",
  tabIndex: -1,
  value: "",
  fontSize: 13,
  isAutoFocussed: false,
  areaSelect: false,
  isJSONField: false,
  copyInfoText: "Content was copied successfully!",
  enableCopy: false,
  hasNumeration: false,
  isFullHeight: false,
};

export default Textarea;
