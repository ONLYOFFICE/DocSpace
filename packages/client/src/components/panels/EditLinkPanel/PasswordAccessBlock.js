import React, { useRef } from "react";
import ToggleBlock from "./ToggleBlock";
import PasswordInput from "@docspace/components/password-input";
import IconButton from "@docspace/components/icon-button";
import Link from "@docspace/components/link";
import RefreshReactSvgUrl from "PUBLIC_DIR/images/refresh.react.svg?url";
import FieldContainer from "@docspace/components/field-container";
import copy from "copy-to-clipboard";
import toastr from "@docspace/components/toast/toastr";

const PasswordAccessBlock = (props) => {
  const {
    t,
    isLoading,
    isChecked,
    passwordValue,
    setPasswordValue,
    isPasswordValid,
  } = props;

  const passwordInputRef = useRef(null);

  const onGeneratePasswordClick = () => {
    passwordInputRef.current.onGeneratePassword();
  };

  const onCleanClick = () => {
    passwordInputRef.current.state.inputValue = ""; //TODO: PasswordInput bug
    setPasswordValue("");
  };

  const onCopyClick = () => {
    copy(passwordValue);
    toastr.success(t("Files:PasswordSuccessfullyCopied"));
  };

  const onChangePassword = (e) => {
    setPasswordValue(e.target.value);
  };

  return (
    <ToggleBlock {...props}>
      {isChecked ? (
        <div>
          <div className="edit-link_password-block">
            <FieldContainer
              labelText={t("Common:Password")}
              isRequired
              isVertical
              hasError={!isPasswordValid}
              errorMessage={t("Common:RequiredField")}
              className="edit-link_password-block"
            >
              <PasswordInput
                className="edit-link_password-input"
                ref={passwordInputRef}
                // scale //doesn't work
                simpleView
                isDisabled={isLoading}
                // tabIndex={3}
                // simpleView
                // passwordSettings={{ minLength: 0 }}
                inputValue={passwordValue}
                onChange={onChangePassword}
              />
            </FieldContainer>

            <IconButton
              className="edit-link_generate-icon"
              size="16"
              isDisabled={isLoading}
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
              isDisabled={isLoading}
              onClick={onCleanClick}
            >
              {t("Clean")}
            </Link>
            <Link
              fontSize="13px"
              fontWeight={600}
              isHovered
              type="action"
              isDisabled={isLoading}
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
