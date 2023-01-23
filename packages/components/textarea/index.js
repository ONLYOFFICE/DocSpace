import React, { useRef, useState, useEffect } from "react";
import PropTypes from "prop-types";
import {
  StyledTextarea,
  StyledScrollbar,
  StyledCopyIcon,
} from "./styled-textarea";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import Toast from "@docspace/components/toast";
import toastr from "@docspace/components/toast/toastr";
import { isJSON, beautifyJSON } from "./utils";

import copy from "copy-to-clipboard";

import copyIcon from "./svg/copyIcon.svg";

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

  return (
    <ColorTheme
      themeId={ThemeType.Textarea}
      className={className}
      style={style}
      stype="preMediumBlack"
      isDisabled={isDisabled}
      hasError={isError}
      heightScale={heightScale}
      heighttextarea={heightTextArea}
    >
      <Toast />
      {isJSONField && (
        <StyledCopyIcon
          src={copyIcon}
          onClick={() => {
            copy(modifiedValue);
            toastr.success(copyInfoText);
          }}
        />
      )}

      <StyledTextarea
        id={id}
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
  /** Max Length of value */
  maxLength: PropTypes.number,
  /** Used as HTML `name` property  */
  name: PropTypes.string,
  /** Allow you to handle changing events of component */
  onChange: PropTypes.func,
  /** Placeholder for Textarea  */
  placeholder: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Used as HTML `tabindex` property */
  tabIndex: PropTypes.number,
  /** Value for Textarea */
  value: PropTypes.string,
  /** Value for font-size */
  fontSize: PropTypes.number,
  /** Value for height text-area */
  heightTextArea: PropTypes.number,
  /** Specifies the text color */
  color: PropTypes.string,
  autoFocus: PropTypes.bool,
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
