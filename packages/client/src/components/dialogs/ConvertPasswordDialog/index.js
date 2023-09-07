import React, { useState, useCallback, useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import SimulatePassword from "../../SimulatePassword";
import StyledComponent from "./StyledConvertPasswordDialog";
import config from "PACKAGE_FILE";
import { openDocEditor } from "@docspace/client/src/helpers/filesUtils";
import combineUrl from "@docspace/common/utils/combineUrl";
import toastr from "@docspace/components/toast/toastr";
let tab, _isMounted;
const ConvertPasswordDialogComponent = (props) => {
  const {
    t,
    visible,
    setConvertPasswordDialogVisible,
    isTabletView,
    copyAsAction,
    formCreationInfo,
    setFormCreationInfo,
    setPasswordEntryProcess,
    isDesktop,
    completeAction,
    fileCopyAs,
  } = props;
  const inputRef = React.useRef(null);

  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [passwordValid, setPasswordValid] = useState(true);

  const makeForm =
    formCreationInfo.fromExst === ".docxf" &&
    formCreationInfo.toExst === ".oform";

  const dialogHeading = makeForm
    ? t("Common:MakeForm")
    : t("Translations:CreateMasterFormFromFile");

  const onClose = () => {
    setConvertPasswordDialogVisible(false);
    setFormCreationInfo(null);
  };
  const onConvert = () => {
    let hasError = false;

    const pass = password.trim();
    if (!pass) {
      hasError = true;
      setPasswordValid(false);
    }

    if (hasError) return;

    tab =
      !isDesktop &&
      window.DocSpaceConfig?.editor?.openOnNewPage &&
      formCreationInfo.fileInfo.fileExst &&
      formCreationInfo.open
        ? window.open(
            combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              config.homepage,
              "/doceditor"
            ),
            "_blank"
          )
        : null;

    setIsLoading(true);
  };

  const onKeyDown = (e) => {
    if (e.key === "Enter") {
      onConvert();
    }
  };

  const focusInput = () => {
    if (inputRef) {
      inputRef.current.focus();
    }
  };

  useEffect(() => {
    const { newTitle, fileInfo, open, actionId } = formCreationInfo;
    const { id, folderId } = fileInfo;

    if (isLoading) {
      if (makeForm) {
        copyAsAction(id, newTitle, folderId, false, password)
          .then(() =>
            toastr.success(t("SuccessfullyCreated", { fileTitle: newTitle }))
          )
          .then(() => {
            onClose();
          })
          .catch((err) => {
            let errorMessage = "";
            if (typeof err === "object") {
              errorMessage =
                err?.response?.data?.error?.message ||
                err?.statusText ||
                err?.message ||
                "";
            } else {
              errorMessage = err;
            }

            if (errorMessage.indexOf("password") == -1) {
              toastr.error(errorMessage, t("Common:Warning"));
              return;
            }

            toastr.error(t("CreationError"), t("Common:Warning"));
            if (_isMounted) {
              setPasswordValid(false);
              focusInput();
            }
          })
          .finally(() => {
            _isMounted && setIsLoading(false);
          });
      } else {
        fileCopyAs(id, newTitle, folderId, false, password)
          .then((file) => {
            toastr.success(t("SuccessfullyCreated", { fileTitle: newTitle }));
            onClose();

            open && openDocEditor(file.id, file.providerKey, tab);
          })
          .then(() => {
            completeAction(fileInfo);
          })
          .catch((err) => {
            let errorMessage = "";
            if (typeof err === "object") {
              errorMessage =
                err?.response?.data?.error?.message ||
                err?.statusText ||
                err?.message ||
                "";
            } else {
              errorMessage = err;
            }
            if (errorMessage.indexOf("password") == -1) {
              toastr.error(errorMessage, t("Common:Warning"));
              return;
            }

            toastr.error(t("CreationError"), t("Common:Warning"));

            // open && openDocEditor(null, null, tab);
            if (_isMounted) {
              setPasswordValid(false);
              focusInput();
            }
          })
          .finally(() => {
            _isMounted && setIsLoading(false);
          });
      }
    }
  }, [isLoading]);

  useEffect(() => {
    _isMounted = true;
    setPasswordEntryProcess(true);

    return () => {
      _isMounted = false;
      setPasswordEntryProcess(false);
    };
  }, []);

  const onChangePassword = useCallback(
    (password) => {
      !passwordValid && setPasswordValid(true);
      setPassword(password);
    },
    [onChangePassword, passwordValid]
  );

  return (
    <ModalDialog
      visible={visible}
      onClose={onClose}
      isLarge
      autoMaxHeight
      autoMaxWidth
    >
      <ModalDialog.Header>{dialogHeading}</ModalDialog.Header>
      <ModalDialog.Body>
        <StyledComponent>
          <div className="convert-password-dialog_content">
            <div className="convert-password-dialog_caption">
              <Text>
                {makeForm
                  ? t("Translations:FileProtected").concat(
                      ". ",
                      t("ConversionPasswordMasterFormCaption")
                    )
                  : t("Translations:FileProtected").concat(
                      ". ",
                      t("ConversionPasswordFormCaption")
                    )}
              </Text>
            </div>
            <div className="password-input">
              <SimulatePassword
                inputMaxWidth={"512px"}
                inputBlockMaxWidth={"536px"}
                onChange={onChangePassword}
                onKeyDown={onKeyDown}
                hasError={!passwordValid}
                isDisabled={isLoading}
                forwardedRef={inputRef}
              />
            </div>
          </div>
        </StyledComponent>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          id="convert-password-dialog_button-accept"
          className="convert-password-dialog_button"
          key="ContinueButton"
          label={t("Common:SaveButton")}
          size="normal"
          scale
          primary
          onClick={onConvert}
          isLoading={isLoading}
        />
        <Button
          className="convert-password-dialog_button"
          key="CloseButton"
          label={t("Common:CloseButton")}
          scale
          size="normal"
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

const ConvertPasswordDialog = withTranslation([
  "ConvertPasswordDialog",
  "Common",
  "Translations",
])(ConvertPasswordDialogComponent);

export default inject(
  ({ filesStore, filesActionsStore, auth, dialogsStore, uploadDataStore }) => {
    const {
      convertPasswordDialogVisible: visible,
      setConvertPasswordDialogVisible,
      setFormCreationInfo,
      formCreationInfo,
    } = dialogsStore;
    const { copyAsAction, fileCopyAs } = uploadDataStore;
    const { setPasswordEntryProcess } = filesStore;
    const { completeAction } = filesActionsStore;
    const { settingsStore } = auth;
    const { isTabletView, isDesktopClient } = settingsStore;

    return {
      visible,
      setConvertPasswordDialogVisible,
      isTabletView,
      copyAsAction,
      fileCopyAs,
      formCreationInfo,
      setFormCreationInfo,
      setPasswordEntryProcess,
      isDesktop: isDesktopClient,
      completeAction,
    };
  }
)(observer(ConvertPasswordDialog));
