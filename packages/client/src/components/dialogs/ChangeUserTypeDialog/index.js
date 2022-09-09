import React, { memo } from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";

import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import ToggleContent from "@docspace/components/toggle-content";
import Checkbox from "@docspace/components/checkbox";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";

import { withTranslation } from "react-i18next";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import toastr from "@docspace/components/toast/toastr";
import { EmployeeType } from "@docspace/common/constants";
import ModalDialogContainer from "../ModalDialogContainer";

import { inject, observer } from "mobx-react";

class ChangeUserTypeDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const { selectedUsers, userIds } = props;

    const listUsers = selectedUsers.map((item, index) => {
      const disabled = userIds.find((x) => x === item.id);
      return (selectedUsers[index] = {
        ...selectedUsers[index],
        checked: disabled ? true : false,
        disabled: disabled ? false : true,
      });
    });

    this.state = { isRequestRunning: false, userIds, listUsers };
  }

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

  onChangeUserType = () => {
    const {
      onClose,
      setSelected,
      t,
      userType,
      updateUserType,
      filter,
    } = this.props;
    const { userIds } = this.state;
    this.setState({ isRequestRunning: true }, () => {
      updateUserType(userType, userIds, filter)
        .then(() => toastr.success(t("SuccessChangeUserType")))
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => {
            setSelected("close");
            onClose();
          });
        });
    });
  };

  render() {
    const { visible, onClose, t, tReady, userType } = this.props;
    const { isRequestRunning, listUsers, userIds } = this.state;
    const itemSize = 25;
    const containerStyles = { height: listUsers.length * 25, maxHeight: 220 };

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

    const firstType = userType === 1 ? t("Common:Guest") : t("Common:User");
    const secondType = userType === 1 ? t("Common:User") : t("Common:Guest");
    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
        autoMaxHeight
      >
        <ModalDialog.Header>{t("ChangeUserTypeHeader")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text>
            {t("ChangeUserTypeMessage", {
              firstType: firstType,
              secondType: secondType,
            })}
          </Text>
          <Text>{t("ChangeUserTypeMessageWarning")}</Text>

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
            label={t("ChangeUserTypeButton")}
            size="normal"
            scale
            primary
            onClick={this.onChangeUserType}
            isLoading={isRequestRunning}
            isDisabled={!userIds.length}
          />
          <Button
            label={t("Common:CancelButton")}
            size="normal"
            scale
            onClick={onClose}
            isDisabled={isRequestRunning}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const ChangeUserTypeDialog = withTranslation([
  "ChangeUserTypeDialog",
  "Common",
])(ChangeUserTypeDialogComponent);

ChangeUserTypeDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  setSelected: PropTypes.func.isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired,
};

export default withRouter(
  inject(({ peopleStore }, ownProps) => ({
    filter: peopleStore.filterStore.filter,
    updateUserType: peopleStore.usersStore.updateUserType,
    selectedUsers: peopleStore.selectionStore.selection,
    setSelected: peopleStore.selectionStore.setSelected,
    userIds:
      ownProps.userType === EmployeeType.User
        ? peopleStore.selectionStore.getUsersToMakeEmployeesIds
        : peopleStore.selectionStore.getUsersToMakeGuestsIds,
  }))(observer(ChangeUserTypeDialog))
);
