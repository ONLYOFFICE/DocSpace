import React from "react";
import { getLogoFromPath } from "@docspace/common/utils";

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
    linkId,
    imageClass,
    isEditor,
  } = props;

  const currentLogo = getLogoFromPath(src);

  return (
    <div>
      <div className="logo-item">
        {title && (
          <Text
            fontSize="13px"
            fontWeight="600"
            className="settings_unavailable"
          >
            {title}
          </Text>
        )}
        {isEditor ? (
          <div className="logos-editor-wrapper">
            <img
              className="logo-docs-editor background-green"
              src={currentLogo}
            />
            <img
              className="logo-docs-editor background-blue"
              src={currentLogo}
            />
            <img
              className="logo-docs-editor background-orange"
              src={currentLogo}
            />
          </div>
        ) : (
          <img className={imageClass} src={currentLogo} />
        )}
      </div>
      <label>
        <input
          id={inputId}
          type="file"
          className="hidden"
          onChange={onChange}
          disabled={!isSettingPaid}
        />
        <Link
          id={linkId}
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
