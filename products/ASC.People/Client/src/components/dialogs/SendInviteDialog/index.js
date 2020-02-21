import React, { useCallback, useState } from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import {
  toastr,
  ModalDialog,
  Button,
  Text,
  ToggleContent
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { api, utils } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";

const { resendUserInvites } = api.people;
const { changeLanguage } = utils;

const SendInviteDialogComponent = props => {
  const { t, onClose, visible, users, setSelected } = props;
  const usersId = [];
  users.map(item => usersId.push(item.id));

  const [isRequestRunning, setIsRequestRunning] = useState(false);

  changeLanguage(i18n);

  const onSendInvite = useCallback(() => {
    setIsRequestRunning(true);
    resendUserInvites(usersId)
      .then(() => toastr.success(t("SuccessSendInvitation")))
      .catch(error => toastr.error(error))
      .finally(() => {
        setIsRequestRunning(false);
        setSelected("close");
        onClose();
      });
  }, [t, setSelected, onClose, usersId]);

  //console.log("SendInviteDialog render");
  return (
    <ModalDialogContainer>
      <ModalDialog
        visible={visible}
        onClose={onClose}
        headerContent={t("SendInviteAgain")}
        bodyContent={
          <>
            <Text>{t("SendInviteAgainDialog")}</Text>
            <Text>{t("SendInviteAgainDialogMessage")}</Text>
            <ToggleContent
              className="toggle-content-dialog"
              label={t("ShowUsersList")}
            >
              {users.map((item, index) => (
                <Text key={index}>{item.displayName}</Text>
              ))}
            </ToggleContent>
          </>
        }
        footerContent={
          <>
            <Button
              label={t("OKButton")}
              size="medium"
              primary
              onClick={onSendInvite}
              isLoading={isRequestRunning}
            />
            <Button
              className="button-dialog"
              label={t("CancelButton")}
              size="medium"
              onClick={onClose}
              isDisabled={isRequestRunning}
            />
          </>
        }
      />
    </ModalDialogContainer>
  );
};

const SendInviteDialogTranslated = withTranslation()(SendInviteDialogComponent);

const SendInviteDialog = props => (
  <SendInviteDialogTranslated i18n={i18n} {...props} />
);

SendInviteDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  users: PropTypes.arrayOf(PropTypes.object).isRequired,
  setSelected: PropTypes.func.isRequired
};

export default withRouter(SendInviteDialog);
