import React, { memo } from "react";
import PropTypes from "prop-types";

import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
//import ToggleContent from "@docspace/components/toggle-content";
import Checkbox from "@docspace/components/checkbox";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";

import { FixedSizeList as List, areEqual } from "react-window";
// import AutoSizer from "react-virtualized-auto-sizer";
import { withTranslation } from "react-i18next";
import { resendUserInvites } from "@docspace/common/api/people";
import toastr from "@docspace/components/toast/toastr";
import ModalDialogContainer from "../ModalDialogContainer";
import { inject, observer } from "mobx-react";

class SendInviteDialogComponent extends React.Component {
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

    this.state = {
      listUsers,
      isRequestRunning: false,
      userIds,
    };
  }

  onSendInvite = () => {
    const { t, setSelected, onClose } = this.props;
    const { userIds } = this.state;

    this.setState({ isRequestRunning: true }, () => {
      resendUserInvites(userIds)
        .then(() =>
          toastr.success(t("PeopleTranslations:SuccessSentInvitation"))
        )
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
    const { t, tReady, onClose, visible } = this.props;
    const { listUsers, isRequestRunning, userIds } = this.state;
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

    //console.log("SendInviteDialog render");
    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
        autoMaxHeight
      >
        <ModalDialog.Header>
          {t("PeopleTranslations:SendInviteAgain")}
        </ModalDialog.Header>
        <ModalDialog.Body>
          <Text>{t("SendInviteAgainDialog")}</Text>
          <Text>{t("SendInviteAgainDialogMessage")}</Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            id="send-inite-again-modal_submit"
            label={t("Common:SendButton")}
            size="normal"
            scale
            primary
            onClick={this.onSendInvite}
            isLoading={isRequestRunning}
            isDisabled={!userIds.length}
          />
          <Button
            id="send-inite-again-modal_cancel"
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

const SendInviteDialog = withTranslation([
  "SendInviteDialog",
  "Common",
  "PeopleTranslations",
])(SendInviteDialogComponent);

SendInviteDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired,
  setSelected: PropTypes.func.isRequired,
};

export default inject(({ peopleStore }) => ({
  selectedUsers: peopleStore.selectionStore.selection,
  setSelected: peopleStore.selectionStore.setSelected,
  userIds: peopleStore.selectionStore.getUsersToInviteIds,
}))(observer(SendInviteDialog));
