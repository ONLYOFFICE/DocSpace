import React, { memo } from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";

import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import ToggleContent from "@appserver/components/toggle-content";
import Checkbox from "@appserver/components/checkbox";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";

import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import { withTranslation } from "react-i18next";
import toastr from "studio/toastr";
import { EmployeeStatus } from "@appserver/common/constants";
import ModalDialogContainer from "../ModalDialogContainer";
import { inject, observer } from "mobx-react";

class ChangeUserStatusDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const { userIds, selectedUsers } = props;

    const listUsers = selectedUsers.map((item, index) => {
      const disabled = userIds.find((x) => x === item.id);
      return (selectedUsers[index] = {
        ...selectedUsers[index],
        checked: disabled ? true : false,
        disabled: disabled ? false : true,
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
      onClose,
    } = this.props;
    const { userIds } = this.state;
    this.setState({ isRequestRunning: true }, () => {
      updateUserStatus(userStatus, userIds, true)
        .then(() => toastr.success(t("Translations:SuccessChangeUserStatus")))
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => {
            setSelected("close");
            onClose();
          });
        });
    });
  };

  onChange = (e) => {
    const { listUsers } = this.state;
    const userIndex = listUsers.findIndex((x) => x.id === e.target.value);
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
    const { t, tReady, onClose, visible, userStatus } = this.props;
    const { listUsers, isRequestRunning, userIds } = this.state;
    const containerStyles = { height: listUsers.length * 25, maxHeight: 220 };
    const itemSize = 25;

    const renderItems = memo(({ data, index, style }) => {
      return (
        <Checkbox
          truncate
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
      userStatus === EmployeeStatus.Active
        ? t("ChangeUsersActiveStatus")
        : t("ChangeUsersDisableStatus");
    const userStatusTranslation =
      userStatus === EmployeeStatus.Active
        ? t("Translations:DisabledEmployeeStatus")
        : t("Common:Active");

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
      >
        <ModalDialog.Header>
          {t("ChangeUserStatusDialogHeader")}
        </ModalDialog.Header>
        <ModalDialog.Body>
          <Text>
            {t("ChangeUserStatusDialog", {
              status: statusTranslation,
              userStatus: userStatusTranslation,
            })}
          </Text>
          <Text>{t("ChangeUserStatusDialogMessage")}</Text>
          <ToggleContent
            className="toggle-content-dialog"
            label={t("Common:ShowUsersList")}
          >
            <div style={containerStyles} className="modal-dialog-content">
              <AutoSizer>{renderList}</AutoSizer>
            </div>
          </ToggleContent>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            label={t("ChangeUsersStatusButton")}
            size="small"
            primary
            onClick={this.onChangeUserStatus}
            isLoading={isRequestRunning}
            isDisabled={!userIds.length}
          />
          <Button
            className="button-dialog"
            label={t("Common:CancelButton")}
            size="small"
            onClick={onClose}
            isDisabled={isRequestRunning}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const ChangeUserStatusDialog = withTranslation([
  "ChangeUserStatusDialog",
  "Common",
  "Translations",
])(ChangeUserStatusDialogComponent);

ChangeUserStatusDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  setSelected: PropTypes.func.isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired,
};

export default withRouter(
  inject(({ peopleStore }, ownProps) => ({
    updateUserStatus: peopleStore.usersStore.updateUserStatus,
    selectedUsers: peopleStore.selectionStore.selection,
    setSelected: peopleStore.selectionStore.setSelected,
    userIds:
      ownProps.userStatus === EmployeeStatus.Active
        ? peopleStore.selectionStore.getUsersToActivateIds
        : peopleStore.selectionStore.getUsersToDisableIds,
  }))(observer(ChangeUserStatusDialog))
);
