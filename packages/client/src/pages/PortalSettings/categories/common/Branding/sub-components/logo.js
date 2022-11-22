import React from "react";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

const Logo = (props) => {
  const {
    title,
    src,
    onChange,
    isSettingPaid,
    onChangeText,
    inputId,
    imageClass,
    isEditor,
  } = props;
  return (
    <div>
      <div className="logo-item">
        {title && (
          <Text fontSize="13px" fontWeight="600">
            {title}
          </Text>
        )}
        {isEditor ? (
          <div className="logos-editor-wrapper">
            <img className="logo-docs-editor background-green" src={src} />
            <img className="logo-docs-editor background-blue" src={src} />
            <img className="logo-docs-editor background-orange" src={src} />
          </div>
        ) : (
          <img className={imageClass} src={src} />
        )}
      </div>
      <label>
        <input
          id={inputId}
          type="file"
          className="hidden"
          //onChange={onChange}
          disabled={!isSettingPaid}
        />
        <Link
          fontWeight="600"
          isHovered
          type="action"
          className="settings_unavailable"
        >
          {onChangeText}
        </Link>
      </label>
    </div>
  );
};

export default Logo;
