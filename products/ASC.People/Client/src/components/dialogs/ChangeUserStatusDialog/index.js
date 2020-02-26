import React, { memo } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import {
  toastr,
  ModalDialog,
  Button,
  Text,
  ToggleContent,
  Checkbox,
  CustomScrollbarsVirtualList
} from "asc-web-components";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { utils } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { updateUserStatus } from "../../../store/people/actions";

const { changeLanguage } = utils;

class ChangeUserStatusDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const { userIds, selectedUsers } = props;

    changeLanguage(i18n);

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

  onChangeUserStatus = () => {
    const {
      updateUserStatus,
      userStatus,
      t,
      setSelected,
      onClose
    } = this.props;
    const { userIds } = this.state;
    this.setState({ isRequestRunning: true }, () => {
      updateUserStatus(userStatus, userIds, true)
        .then(() => toastr.success(t("SuccessChangeUserStatus")))
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

  render() {
    const { t, onClose, visible, userStatus } = this.props;
    const { listUsers, isRequestRunning, userIds } = this.state;
    const containerStyles = { height: 220 };
    const itemSize = 25;

    const renderItems = memo(({ data, index, style }) => {
      return (
        <Checkbox
          style={style}
          className="modal-dialog-checkbox"
          value={data[index].id}
          onChange={this.onChange}
          key={`checkbox_${index}`}
          isChecked={data[index].checked}
          label={data[index].displayName}
          isDisabled={data[index].disabled}
        />
      );
    }, areEqual);

    const renderList = ({ height, width }) => (
      <List
        className="List"
        height={height}
        width={width}
        itemSize={itemSize}
        itemCount={listUsers.length}
        itemData={listUsers}
        outerElementType={CustomScrollbarsVirtualList}
      >
        {renderItems}
      </List>
    );

    const statusTranslation =
      userStatus === 1
        ? t("ChangeUsersActiveStatus")
        : t("ChangeUsersDisableStatus");
    const userStatusTranslation =
      userStatus === 1 ? t("DisabledEmployeeTitle") : t("ActiveEmployeeTitle");

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
                <div style={containerStyles} className="modal-dialog-content">
                  <AutoSizer>{renderList}</AutoSizer>
                </div>
              </ToggleContent>
            </>
          }
          footerContent={
            <>
              <Button
                label={t("ChangeUsersStatusButton")}
                size="medium"
                primary
                onClick={this.onChangeUserStatus}
                isLoading={isRequestRunning}
                isDisabled={!userIds.length}
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

const ChangeUserStatusDialogTranslated = withTranslation()(
  ChangeUserStatusDialogComponent
);

const ChangeUserStatusDialog = props => (
  <ChangeUserStatusDialogTranslated i18n={i18n} {...props} />
);

ChangeUserStatusDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  setSelected: PropTypes.func.isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired
};

export default connect(null, { updateUserStatus })(
  withRouter(ChangeUserStatusDialog)
);
