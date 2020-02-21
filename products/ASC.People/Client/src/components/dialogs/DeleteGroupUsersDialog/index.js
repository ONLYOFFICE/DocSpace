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
import { api, utils } from "asc-web-common";
import { fetchPeople, removeUser } from "../../../store/people/actions";
import ModalDialogContainer from "../ModalDialogContainer";

const { Filter } = api;
const { changeLanguage } = utils;

const DeleteGroupUsersDialogComponent = props => {
  const {
    t,
    filter,
    fetchPeople,
    onClose,
    removeUser,
    visible,
    users,
    setSelected
  } = props;
  const usersId = [];
  users.map(item => usersId.push(item.id));

  const [isRequestRunning, setIsRequestRunning] = useState(false);

  changeLanguage(i18n);

  const onDeleteGroupUsers = useCallback(() => {
    setIsRequestRunning(true);
    removeUser(usersId, filter)
      .then(() => {
        toastr.success(t("DeleteGroupUsersSuccessMessage"));
        return fetchPeople(filter);
      })
      .catch(error => toastr.error(error))
      .finally(() => {
        setIsRequestRunning(false);
        setSelected("close");
        onClose();
      });
  }, [removeUser, fetchPeople, filter, setSelected, onClose, t, usersId]);

  //console.log("DeleteGroupUsersDialog render");
  return (
    <ModalDialogContainer>
      <ModalDialog
        visible={visible}
        onClose={onClose}
        headerContent={t("DeleteGroupUsersMessageHeader")}
        bodyContent={
          <>
            <Text>{t("DeleteGroupUsersMessage")}</Text>
            <Text>{t("NotBeUndone")}</Text>
            <br />
            <Text color="#c30" fontSize="18px">
              {t("Warning")}
            </Text>
            <br />
            <Text>{t("DeleteUserDataConfirmation")}</Text>
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
              onClick={onDeleteGroupUsers}
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

const DeleteGroupUsersDialogTranslated = withTranslation()(
  DeleteGroupUsersDialogComponent
);

const DeleteGroupUsersDialog = props => (
  <DeleteGroupUsersDialogTranslated i18n={i18n} {...props} />
);

DeleteGroupUsersDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  setSelected: PropTypes.func.isRequired,
  users: PropTypes.arrayOf(PropTypes.object).isRequired,
  filter: PropTypes.instanceOf(Filter).isRequired,
  fetchPeople: PropTypes.func.isRequired,
  removeUser: PropTypes.func.isRequired
};

export default connect(null, { fetchPeople, removeUser })(
  withRouter(DeleteGroupUsersDialog)
);
