import React, { memo } from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import {
  ModalDialog,
  Button,
  Text,
  ToggleContent,
  Checkbox,
  CustomScrollbarsVirtualList,
} from "asc-web-components";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import { withTranslation } from "react-i18next";
import { api, utils, toastr } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { createI18N } from "../../../helpers/i18n";
import { connect } from "react-redux";
import { getUsersToInviteIds } from "../../../store/people/selectors";
import { setSelected } from "../../../store/people/actions";

const i18n = createI18N({
  page: "SendInviteDialog",
  localesPath: "dialogs/SendInviteDialog",
});
const { resendUserInvites } = api.people;
const { changeLanguage } = utils;

class SendInviteDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

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
        .then(() => toastr.success(t("SuccessSendInvitation")))
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
    const { t, onClose, visible } = this.props;
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
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("SendInviteAgain")}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text>{t("SendInviteAgainDialog")}</Text>
            <Text>{t("SendInviteAgainDialogMessage")}</Text>
            <ToggleContent
              className="toggle-content-dialog"
              label={t("ShowUsersList")}
            >
              <div style={containerStyles} className="modal-dialog-content">
                <AutoSizer>{renderList}</AutoSizer>
              </div>
            </ToggleContent>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              label={t("OKButton")}
              size="medium"
              primary
              onClick={this.onSendInvite}
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
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const SendInviteDialogTranslated = withTranslation()(SendInviteDialogComponent);

const SendInviteDialog = (props) => (
  <SendInviteDialogTranslated i18n={i18n} {...props} />
);

SendInviteDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired,
  setSelected: PropTypes.func.isRequired,
};

const mapStateToProps = (state) => {
  const { selection } = state.people;
  const usersToInviteIds = getUsersToInviteIds(state);

  return {
    userIds: usersToInviteIds,
    selectedUsers: selection,
  };
};

export default connect(mapStateToProps, { setSelected })(
  withRouter(SendInviteDialog)
);
