import React, { useRef } from "react";
import ToggleBlock from "./ToggleBlock";
import PasswordInput from "@docspace/components/password-input";
import IconButton from "@docspace/components/icon-button";
import Link from "@docspace/components/link";
import RefreshReactSvgUrl from "PUBLIC_DIR/images/refresh.react.svg?url";

const PasswordAccessBlock = (props) => {
  const { t, isChecked } = props;

  const passwordInputRef = useRef(null);

  const onGeneratePasswordClick = () => {
    passwordInputRef.current.onGeneratePassword();
  };

  const onCleanClick = () => {
    alert("onCleanClick");
  };

  const onCopyClick = () => {
    alert("onCopyClick");
  };

  return (
    <ToggleBlock {...props}>
      {isChecked ? (
        <div>
          <div className="edit-link_password-block">
            <PasswordInput
              className="edit-link_password-input"
              ref={passwordInputRef}
              // scale //doesn't work
              simpleView

              // hasError={!isPasswordValid}
              // isDisabled={isLoading}
              // tabIndex={3}
              // simpleView
              // passwordSettings={{ minLength: 0 }}
              // value={passwordValue}
              // onChange={onChangePassword}
            />

            <IconButton
              className="edit-link_generate-icon"
              size="16"
              iconName={RefreshReactSvgUrl}
              onClick={onGeneratePasswordClick}
            />
          </div>
          <div className="edit-link_password-links">
            <Link
              fontSize="13px"
              fontWeight={600}
              isHovered
              type="action"
              onClick={onCleanClick}
            >
              {t("Clean")}
            </Link>
            <Link
              fontSize="13px"
              fontWeight={600}
              isHovered
              type="action"
              onClick={onCopyClick}
            >
              {t("CopyPassword")}
            </Link>
          </div>
        </div>
      ) : (
        <></>
      )}
    </ToggleBlock>
  );
};

export default PasswordAccessBlock;
