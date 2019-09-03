import React, { useCallback } from "react";
import {
  GroupButtonsMenu,
  DropDownItem,
  Text,
  toastr,
  ContextMenuButton
} from "asc-web-components";
import { connect } from "react-redux";
import {
  getSelectedGroup,
  getSelectionIds
} from "../../../../../store/people/selectors";
import { isAdmin } from "../../../../../store/auth/selectors";
import { withTranslation } from "react-i18next";
import {
  updateUserStatus,
  updateUserType,
  fetchPeople
} from "../../../../../store/people/actions";
import { EmployeeStatus, EmployeeType } from "../../../../../helpers/constants";
import {
  typeUser,
  typeGuest
} from "../../../../../helpers/../helpers/customNames";
import {
  resendUserInvites,
  deleteUsers
} from "../../../../../store/services/api";

const contextOptions = t => {
  return [
    {
      key: "edit-group",
      label: t("EditButton"),
      onClick: toastr.success.bind(this, "Edit group action")
    },
    {
      key: "delete-group",
      label: t("DeleteButton"),
      onClick: toastr.success.bind(this, "Delete group action")
    }
  ];
};

const wrapperStyle = {
  display: "flex",
  alignItems: "center"
};

const SectionHeaderContent = props => {
  const {
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
    onCheck,
    onSelect,
    onClose,
    group,
    isAdmin,
    t,
    selection,
    updateUserStatus,
    updateUserType,
    onLoading, 
    filter
  } = props;

  const selectedUserIds = getSelectionIds(selection);
  //console.log("SectionHeaderContent render", selection, selectedUserIds);

  const onSetActive = useCallback(() => {
    updateUserStatus(EmployeeStatus.Active, selectedUserIds);
    toastr.success(t("SuccessChangeUserStatus"));
  }, [selectedUserIds, updateUserStatus, t]);

  const onSetDisabled = useCallback(() => {
    updateUserStatus(EmployeeStatus.Disabled, selectedUserIds);
    toastr.success(t("SuccessChangeUserStatus"));
  }, [selectedUserIds, updateUserStatus, t]);

  const onSetEmployee = useCallback(() => {
    updateUserType(EmployeeType.User, selectedUserIds);
    toastr.success(t("SuccessChangeUserType"));
  }, [selectedUserIds, updateUserType, t]);

  const onSetGuest = useCallback(() => {
    updateUserType(EmployeeType.Guest, selectedUserIds);
    toastr.success(t("SuccessChangeUserType"));
  }, [selectedUserIds, updateUserType, t]);

  const onSentInviteAgain = useCallback(() => {
    resendUserInvites(selectedUserIds)
      .then(() => toastr.success("The invitation was successfully sent"))
      .catch(e => toastr.error("ERROR"));
  }, [selectedUserIds]);

  const onDelete = useCallback(() => {
    onLoading(true);
    deleteUsers(selectedUserIds)
      .then(() => {
        toastr.success("Users have been removed successfully");
        return fetchPeople(filter);
      })
      .catch(e => toastr.error("ERROR"))
      .finally(() => onLoading(false));
  }, [selectedUserIds, onLoading, filter]);

  const menuItems = [
    {
      label: t("LblSelect"),
      isDropdown: true,
      isSeparator: true,
      isSelect: true,
      fontWeight: "bold",
      children: [
        <DropDownItem key="active" label={t("LblActive")} />,
        <DropDownItem key="disabled" label={t("LblTerminated")} />,
        <DropDownItem key="invited" label={t("LblInvited")} />
      ],
      onSelect: item => onSelect(item.key)
    },
    {
      label: t("CustomMakeUser", { typeUser }),
      disabled: !selection.length,
      onClick: onSetEmployee
    },
    {
      label: t("CustomMakeGuest", { typeGuest }),
      disabled: !selection.length,
      onClick: onSetGuest
    },
    {
      label: t("LblSetActive"),
      disabled: !selection.length,
      onClick: onSetActive
    },
    {
      label: t("LblSetDisabled"),
      disabled: !selection.length,
      onClick: onSetDisabled
    },
    {
      label: t("LblInviteAgain"),
      disabled: !selection.length,
      onClick: onSentInviteAgain
    },
    {
      label: t("LblSendEmail"),
      disabled: !selection.length,
      onClick: toastr.success.bind(this, "Send e-mail action")
    },
    {
      label: t("DeleteButton"),
      disabled: !selection.length,
      onClick: onDelete
    }
  ];

  return isHeaderVisible ? (
    <div style={{ margin: "0 -16px" }}>
      <GroupButtonsMenu
        checked={isHeaderChecked}
        isIndeterminate={isHeaderIndeterminate}
        onChange={onCheck}
        menuItems={menuItems}
        visible={isHeaderVisible}
        moreLabel={t("More")}
        closeTitle={t("CloseButton")}
        onClose={onClose}
        selected={menuItems[0].label}
      />
    </div>
  ) : group ? (
    <div style={wrapperStyle}>
      <Text.ContentHeader>{group.name}</Text.ContentHeader>
      {isAdmin && (
        <ContextMenuButton
          directionX="right"
          title={t("Actions")}
          iconName="VerticalDotsIcon"
          size={16}
          color="#A3A9AE"
          getData={contextOptions.bind(this, t)}
          isDisabled={false}
        />
      )}
    </div>
  ) : (
    <Text.ContentHeader>{t("People")}</Text.ContentHeader>
  );
};

const mapStateToProps = state => {
  return {
    group: getSelectedGroup(state.people.groups, state.people.selectedGroup),
    selection: state.people.selection,
    isAdmin: isAdmin(state.auth.user),
    filter: state.people.filter
  };
};

export default connect(
  mapStateToProps,
  { updateUserStatus, updateUserType, fetchPeople }
)(withTranslation()(SectionHeaderContent));
