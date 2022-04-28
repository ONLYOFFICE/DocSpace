import React, { memo } from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";

import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import ToggleContent from "@appserver/components/toggle-content";
import Checkbox from "@appserver/components/checkbox";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";

import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import { withTranslation } from "react-i18next";
import Filter from "@appserver/common/api/people/filter";
import toastr from "studio/toastr";
import ModalDialogContainer from "../ModalDialogContainer";
import { inject, observer } from "mobx-react";

class DeleteGroupUsersDialogComponent extends React.Component {
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
    const userIndex = this.state.listUsers.findIndex(
      (x) => x.id === e.target.value
    );
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
    const { t, tReady, onClose, visible, theme } = this.props;
    const { isRequestRunning, userIds, listUsers } = this.state;
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

    //console.log("DeleteGroupUsersDialog render");
    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
      >
        <ModalDialog.Header>
          {t("DeleteGroupUsersMessageHeader")}
        </ModalDialog.Header>
        <ModalDialog.Body>
          <Text>{t("DeleteGroupUsersMessage")}</Text>
          <Text>{t("Translations:NotBeUndone")}</Text>
          <br />
          <Text
            color={theme.peopleDialogs.deleteUser.textColor}
            fontSize="18px"
          >
            {t("Common:Warning")}!
          </Text>
          <br />
          <Text>{t("DeleteUserDataConfirmation")}</Text>
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
            label={t("Common:OKButton")}
            size="small"
            primary
            onClick={this.onDeleteGroupUsers}
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

const DeleteUsersDialog = withTranslation([
  "DeleteUsersDialog",
  "Common",
  "Translations",
])(DeleteGroupUsersDialogComponent);

DeleteUsersDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,

  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  filter: PropTypes.instanceOf(Filter).isRequired,

  setSelected: PropTypes.func.isRequired,
  removeUser: PropTypes.func.isRequired,
};

export default withRouter(
  inject(({ peopleStore, auth }) => ({
    filter: peopleStore.filterStore.filter,
    removeUser: peopleStore.usersStore.removeUser,
    selectedUsers: peopleStore.selectionStore.selection,
    setSelected: peopleStore.selectionStore.setSelected,
    userIds: peopleStore.selectionStore.getUsersToRemoveIds,
    theme: auth.settingsStore.theme,
  }))(observer(DeleteUsersDialog))
);
