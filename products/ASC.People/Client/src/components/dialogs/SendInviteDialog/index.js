import React from "react";
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
import ModalDialogContainer from "../ModalDialogContainer";

const { resendUserInvites } = api.people;
const { changeLanguage } = utils;

class SendInviteDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    const { userIds, selectedUsers } = props;

    const listUsers = selectedUsers.map((item, index) => {
      const disabled = userIds.find(x => x === item.id);
      return (selectedUsers[index] = {
        ...selectedUsers[index],
        checked: disabled ? true : false,
        disabled: disabled ? false : true
      });
    });

    this.state = {
      listUsers,
      isRequestRunning: false,
      userIds
    };
  }

  onSendInvite = () => {
    const { t, setSelected, onClose } = this.props;
    const { userIds } = this.state;

    this.setState({ isRequestRunning: true }, () => {
      resendUserInvites(userIds)
        .then(() => toastr.success(t("SuccessSendInvitation")))
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
    const { t, onClose, visible } = this.props;
    const { listUsers, isRequestRunning } = this.state;

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
                <div className="modal-dialog-content">
                  {listUsers.map((item, index) => (
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
                onClick={this.onSendInvite}
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

const SendInviteDialogTranslated = withTranslation()(SendInviteDialogComponent);

const SendInviteDialog = props => (
  <SendInviteDialogTranslated i18n={i18n} {...props} />
);

SendInviteDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired,
  setSelected: PropTypes.func.isRequired
};

export default withRouter(SendInviteDialog);
