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
import { utils } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { updateUserType } from "../../../store/people/actions";

const { changeLanguage } = utils;

class ChangeUserTypeDialogComponent extends React.Component {
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

    this.state = { isRequestRunning: false, userIds, listUsers };
  }

  onChange = e => {
    const { listUsers } = this.state;
    const userIndex = listUsers.findIndex(x => x.id === e.target.value);
    const newUsersList = listUsers;
    newUsersList[userIndex].checked = !newUsersList[userIndex].checked;

    const newUserIds = [];
    for (let item of newUsersList) {
      if (item.checked === true) {
        newUserIds.push(item.id);
      }
    }

    this.setState({ listUsers: newUsersList, userIds: newUserIds });
  };

  onChangeUserType = () => {
    const { onClose, setSelected, t, userType, updateUserType } = this.props;
    const { userIds } = this.state;
    this.setState({ isRequestRunning: true }, () => {
      updateUserType(userType, userIds)
        .then(() => toastr.success(t("SuccessChangeUserType")))
        .catch(error => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => {
            setSelected("close");
            onClose();
          });
        });
    });
  };

  render() {
    const { visible, onClose, t, userType } = this.props;
    const { isRequestRunning, listUsers } = this.state;

    const firstType = userType === 1 ? t("GuestCaption") : t("UserCol");
    const secondType = userType === 1 ? t("UserCol") : t("GuestCaption");
    return (
      <ModalDialogContainer>
        <ModalDialog
          visible={visible}
          onClose={onClose}
          headerContent={t("ChangeUserTypeHeader")}
          bodyContent={
            <>
              <Text>
                {t("ChangeUserTypeMessage", {
                  firstType: firstType,
                  secondType: secondType
                })}
              </Text>
              <Text>{t("ChangeUserTypeMessageWarning")}</Text>

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
                label={t("ChangeUserTypeButton")}
                size="medium"
                primary
                onClick={this.onChangeUserType}
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

const ChangeUserTypeDialogTranslated = withTranslation()(
  ChangeUserTypeDialogComponent
);

const ChangeUserTypeDialog = props => (
  <ChangeUserTypeDialogTranslated i18n={i18n} {...props} />
);

ChangeUserTypeDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  setSelected: PropTypes.func.isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired
};

export default connect(null, { updateUserType })(
  withRouter(ChangeUserTypeDialog)
);
