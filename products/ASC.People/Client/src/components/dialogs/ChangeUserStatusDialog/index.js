import React, { useCallback, useState } from "react";
import { connect } from "react-redux";
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
import { utils } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { updateUserStatus } from "../../../store/people/actions";

const { changeLanguage } = utils;

const ChangeUserStatusDialogComponent = props => {
  const { t, onClose, visible, users, userStatus, updateUserStatus } = props;
  const usersId = [];
  users.map(item => usersId.push(item.id));

  const [isRequestRunning, setIsRequestRunning] = useState(false);

  changeLanguage(i18n);

  const onChangeUserStatus = useCallback(() => {
    setIsRequestRunning(true);
    updateUserStatus(userStatus, usersId, true)
      .then(() => toastr.success(t("SuccessChangeUserStatus")))
      .catch(error => toastr.error(error))
      .finally(() => {
        setIsRequestRunning(false);
        onClose();
      });
  }, [t, usersId, userStatus, updateUserStatus, onClose]);

  const statusTranslation =
    userStatus === 1
      ? t("ChangeUsersActiveStatus")
      : t("ChangeUsersDisableStatus");
  const userStatusTranslation =
    userStatus === 1
      ? t("ChangeUsersDisableUserStatus")
      : t("ChangeUsersActiveUserStatus");

  return (
    <ModalDialogContainer>
      <ModalDialog
        visible={visible}
        onClose={onClose}
        headerContent={t("ChangeUserStatusDialogHeader")}
        bodyContent={
          <>
            <Text>
              {t("ChangeUserStatusDialog", {
                status: statusTranslation,
                userStatus: userStatusTranslation
              })}
            </Text>
            <Text>{t("ChangeUserStatusDialogMessage")}</Text>
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
              label={t("ChangeUsersStatusButton")}
              size="medium"
              primary
              onClick={onChangeUserStatus}
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

const ChangeUserStatusDialogTranslated = withTranslation()(
  ChangeUserStatusDialogComponent
);

const ChangeUserStatusDialog = props => (
  <ChangeUserStatusDialogTranslated i18n={i18n} {...props} />
);

ChangeUserStatusDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  users: PropTypes.arrayOf(PropTypes.object).isRequired
};

export default connect(null, { updateUserStatus })(
  withRouter(ChangeUserStatusDialog)
);
