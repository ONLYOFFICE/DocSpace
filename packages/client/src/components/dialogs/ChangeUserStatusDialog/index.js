import React, { memo } from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";

import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import ToggleContent from "@docspace/components/toggle-content";
import Checkbox from "@docspace/components/checkbox";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";

import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import { EmployeeStatus } from "@docspace/common/constants";
import ModalDialogContainer from "../ModalDialogContainer";
import { inject, observer } from "mobx-react";

class ChangeUserStatusDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    const { userIds } = props;

    this.state = { isRequestRunning: false, userIds };
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
      updateUserStatus(userStatus, userIds)
        .then(() =>
          toastr.success(t("PeopleTranslations:SuccessChangeUserStatus"))
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

  render() {
    const { t, tReady, onClose, visible, userStatus } = this.props;
    const { isRequestRunning, userIds } = this.state;

    const statusTranslation =
      userStatus === EmployeeStatus.Active
        ? t("ChangeUsersActiveStatus")
        : t("ChangeUsersDisableStatus");
    const userStatusTranslation =
      userStatus === EmployeeStatus.Active
        ? t("PeopleTranslations:DisabledEmployeeStatus")
        : t("Common:Active");

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
        autoMaxHeight
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
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            label={t("ChangeUsersStatusButton")}
            size="normal"
            primary
            scale
            onClick={this.onChangeUserStatus}
            isLoading={isRequestRunning}
            isDisabled={userIds.length === 0}
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

const ChangeUserStatusDialog = withTranslation([
  "ChangeUserStatusDialog",
  "Common",
  "PeopleTranslations",
])(ChangeUserStatusDialogComponent);

ChangeUserStatusDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  setSelected: PropTypes.func.isRequired,
  userIds: PropTypes.arrayOf(PropTypes.string).isRequired,
  selectedUsers: PropTypes.arrayOf(PropTypes.object).isRequired,
};

export default withRouter(
  inject(({ peopleStore }, ownProps) => {
    const updateUserStatus = peopleStore.usersStore.updateUserStatus;

    const selectedUsers = peopleStore.selectionStore.selection;
    const setSelected = peopleStore.selectionStore.setSelected;
    const userIds =
      ownProps.userStatus === EmployeeStatus.Active
        ? peopleStore.selectionStore.getUsersToActivateIds
        : peopleStore.selectionStore.getUsersToDisableIds;

    return {
      updateUserStatus,
      selectedUsers,
      setSelected,
      userIds,
    };
  })(observer(ChangeUserStatusDialog))
);
