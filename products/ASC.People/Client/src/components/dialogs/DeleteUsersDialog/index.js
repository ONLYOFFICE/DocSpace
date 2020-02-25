import React from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import {
  toastr,
  ModalDialog,
  Button,
  Text,
  ToggleContent,
  Checkbox
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { api, utils } from "asc-web-common";
import { removeUser } from "../../../store/people/actions";
import ModalDialogContainer from "../ModalDialogContainer";

const { Filter } = api;
const { changeLanguage } = utils;

class DeleteGroupUsersDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    const { selectedUsers, userIds } = props;

    const listUsers = selectedUsers.map((item, index) => {
      const disabled = userIds.find(x => x === item.id);
      return (selectedUsers[index] = {
        ...selectedUsers[index],
        checked: disabled ? true : false,
        disabled: disabled ? false : true
      });
    });

    this.state = { isRequestRunning: false, listUsers, userIds };
  }

  onDeleteGroupUsers = () => {
    const { removeUser, t, setSelected, onClose, filter } = this.props;
    const { userIds } = this.state;

    this.setState({ isRequestRunning: true }, () => {
      removeUser(userIds, filter)
        .then(() => {
          toastr.success(t("DeleteGroupUsersSuccessMessage"));
        })
        .catch(error => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => {
            setSelected("close");
            onClose();
          });
        });
    });
  };

  onChange = e => {
    const userIndex = this.state.listUsers.findIndex(x => x.id === e.target.value);
    const newUsersList = this.state.listUsers;
    newUsersList[userIndex].checked = !newUsersList[userIndex].checked;

    const newUserIds = [];

    for (let item of newUsersList) {
      if (item.checked === true) {
        newUserIds.push(item.id);
      }
    }

    this.setState({ listUsers: newUsersList, userIds: newUserIds });
  };

  render() {
    const { t, onClose, visible, selectedUsers } = this.props;
    const { isRequestRunning } = this.state;

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
                <div className="modal-dialog-content">
                  {selectedUsers.map((item, index) => (
                    <Checkbox
                      className="modal-dialog-checkbox"
                      value={item.id}
                      onChange={this.onChange}
                      key={`checkbox_${index}`}
                      isChecked={item.checked}
                      label={item.displayName}
                      isDisabled={item.disabled}
                    />
                  ))}
                </div>
              </ToggleContent>
            </>
          }
          footerContent={
            <>
              <Button
                label={t("OKButton")}
                size="medium"
                primary
                onClick={this.onDeleteGroupUsers}
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
  }
}

const DeleteGroupUsersDialogTranslated = withTranslation()(
  DeleteGroupUsersDialogComponent
);

const DeleteUsersDialog = props => (
  <DeleteGroupUsersDialogTranslated i18n={i18n} {...props} />
);

DeleteUsersDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  setSelected: PropTypes.func.isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  filter: PropTypes.instanceOf(Filter).isRequired,
  removeUser: PropTypes.func.isRequired
};

export default connect(null, { removeUser })(
  withRouter(DeleteUsersDialog)
);
