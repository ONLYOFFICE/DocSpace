import React, { useRef, useState, useEffect } from "react";
import PropTypes from "prop-types";
import {
  StyledTextarea,
  StyledScrollbar,
  StyledCopyIcon,
  Wrapper,
  Numeration,
} from "./styled-textarea";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import Toast from "@docspace/components/toast";
import toastr from "@docspace/components/toast/toastr";
import { isJSON, beautifyJSON } from "./utils";

import copy from "copy-to-clipboard";

// eslint-disable-next-line react/prop-types, no-unused-vars

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
}) => {
  const areaRef = useRef(null);
  const [isError, setIsError] = useState(hasError);
  const [modifiedValue, setModifiedValue] = useState(value);

  const lineHeight = 1.5;
  const padding = 7;
  const numberOfLines = modifiedValue.split("\n").length;
  const textareaHeight = isJSONField
    ? numberOfLines * fontSize * lineHeight + padding + 4
    : heightTextArea;

  const defaultPaddingLeft = 42;
  const numberOfDigits =
    String(numberOfLines).length - 2 > 0 ? String(numberOfLines).length : 0;
  const paddingLeftProp = isJSONField
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
    if (isJSONField) {
      if (modifiedValue && isJSON(modifiedValue)) {
        setModifiedValue(beautifyJSON(modifiedValue));
      } else {
        setIsError(true);
      }
    }
  }, [isJSONField]);

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
      onFocus={isJSONField ? onTextareaClick : undefined}
    >
      {isJSONField && (
        <WrappedStyledCopyIcon
          onClick={() => {
            copy(modifiedValue);
            toastr.success(copyInfoText);
          }}
          heightScale={heightScale}
          isJSONField={isJSONField}
        />
      )}
      <ColorTheme
        themeId={ThemeType.Textarea}
        className={className}
        style={style}
        stype="preMediumBlack"
        isDisabled={isDisabled}
        hasError={isError}
        heightScale={heightScale}
        heighttextarea={textareaHeight}
      >
        <Toast />

        {isJSONField && (
          <Numeration fontSize={fontSize}>
            {numerationValue.join("\n")}
          </Numeration>
        )}

        <StyledTextarea
          id={id}
          paddingLeftProp={paddingLeftProp}
          isJSONField={isJSONField}
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
          heighttextarea={heightTextArea}
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
};

Textarea.defaultProps = {
  className: "",
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
};

export default Textarea;
