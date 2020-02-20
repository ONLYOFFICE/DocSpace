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
import { utils, constants } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { updateUserStatus } from "../../../store/people/actions";

const { EmployeeStatus } = constants;
const { changeLanguage } = utils;

const SetDisabledDialogComponent = props => {
  const { t, onClose, visible, users, updateUserStatus } = props;
  const usersId = [];
  users.map(item => usersId.push(item.id));

  const [isRequestRunning, setIsRequestRunning] = useState(false);

  changeLanguage(i18n);

  const onSetDisabled = useCallback(() => {
    setIsRequestRunning(true);
    updateUserStatus(EmployeeStatus.Disabled, usersId, true)
      .then(() => toastr.success(t("SuccessChangeUserStatus")))
      .catch(error => toastr.error(error))
      .finally(() => {
        setIsRequestRunning(false);
        onClose();
      });
  }, [t, usersId, updateUserStatus, onClose]);

  //console.log("SendInviteDialog render");
  return (
    <ModalDialogContainer>
      <ModalDialog
        visible={visible}
        onClose={onClose}
        headerContent={t("ChangeUserStatusDialogHeader")}
        bodyContent={
          <>
            <Text>{t("ChangeUserStatusDialog")}</Text>
            <Text>{t("ChangeUserStatusDialogMessage")}</Text>
            <ToggleContent
              className="toggle-content-dialog"
              label={t("DeleteGroupUsersShowUsers")}
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
              onClick={onSetDisabled}
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

const SetDisabledDialogTranslated = withTranslation()(
  SetDisabledDialogComponent
);

const SetDisabledDialog = props => (
  <SetDisabledDialogTranslated i18n={i18n} {...props} />
);

SetDisabledDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  users: PropTypes.arrayOf(PropTypes.object).isRequired
};

export default connect(null, { updateUserStatus })(
  withRouter(SetDisabledDialog)
);
