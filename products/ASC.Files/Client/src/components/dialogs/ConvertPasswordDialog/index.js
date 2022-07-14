import React, { useState, useCallback, useEffect } from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import SimulatePassword from "../../SimulatePassword";
import StyledComponent from "./StyledConvertPasswordDialog";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../package.json";
import { openDocEditor } from "../../../helpers/utils";
import combineUrl from "@appserver/common/utils/combineUrl";
import toastr from "@appserver/components/toast/toastr";
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
    editCompleteAction,
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
      !isDesktop && formCreationInfo.fileInfo.fileExst && formCreationInfo.open
        ? window.open(
            combineUrl(AppServerConfig.proxyURL, config.homepage, "/doceditor"),
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
            if (err.indexOf("password") == -1) {
              toastr.error(err, t("Common:Warning"));
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
            editCompleteAction(actionId, fileInfo, false);
          })
          .catch((err) => {
            if (err.indexOf("password") == -1) {
              toastr.error(err, t("Common:Warning"));
              return;
            }

            toastr.error(t("CreationError"), t("Common:Warning"));
            open && openDocEditor(null, null, tab);
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
    <ModalDialog visible={visible} onClose={onClose}>
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
        <StyledComponent isTabletView={isTabletView}>
          <div className="convert-password_footer">
            <Button
              id="convert-password-dialog_button-accept"
              className="convert-password-dialog_button"
              key="ContinueButton"
              label={t("Common:SaveButton")}
              size="small"
              primary
              onClick={onConvert}
              isLoading={isLoading}
            />
            <Button
              className="convert-password-dialog_button"
              key="CloseButton"
              label={t("Common:CloseButton")}
              size="small"
              onClick={onClose}
            />
          </div>
        </StyledComponent>
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
    const { editCompleteAction } = filesActionsStore;
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
      editCompleteAction,
    };
  }
)(observer(ConvertPasswordDialog));
