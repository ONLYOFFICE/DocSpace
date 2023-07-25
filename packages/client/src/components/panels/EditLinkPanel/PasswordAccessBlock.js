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
    setIsPasswordValid,
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
    const isPasswordValid = !!passwordValue.trim();
    if (isPasswordValid) {
      copy(passwordValue);
      toastr.success(t("Files:PasswordSuccessfullyCopied"));
    }
  };

  const onChangePassword = (e) => {
    setPasswordValue(e.target.value);
    setIsPasswordValid(true);
  };

  return (
    <ToggleBlock {...props}>
      {isChecked ? (
        <div>
          <div className="edit-link_password-block">
            <FieldContainer
              isVertical
              hasError={!isPasswordValid}
              errorMessage={t("Common:RequiredField")}
              className="edit-link_password-block"
            >
              <PasswordInput
                // scale //doesn't work
                // tabIndex={3}
                // simpleView
                // passwordSettings={{ minLength: 0 }}
                className="edit-link_password-input"
                ref={passwordInputRef}
                simpleView
                isDisabled={isLoading}
                hasError={!isPasswordValid}
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
