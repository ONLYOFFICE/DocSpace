import React from "react";
import { connect } from "react-redux";
import {
  Text,
  IconButton,
  ContextMenuButton,
  toastr
} from "asc-web-components";
import { withRouter } from "react-router";
import { isAdmin, isMe } from "../../../../../store/auth/selectors";
import { getUserStatus } from "../../../../../store/people/selectors";

const wrapperStyle = {
  display: "flex",
  alignItems: "center"
};

const textStyle = {
  marginLeft: "16px",
  marginRight: "16px"
};

const SectionHeaderContent = props => {
  const { profile, history, settings, isAdmin, viewer } = props;

  const onEditClick = user => {
    history.push(`${settings.homepage}/edit/${user.userName}`);
  };

  const onChangePasswordClick = () => {
    toastr.success("Context action: Change password");
  };

  const onChangePhoneClick = () => {
    toastr.success("Context action: Change phone");
  };

  const onChangeEmailClick = () => {
    toastr.success("Context action: Change e-mail");
  };

  const onDisableClick = () => {
    toastr.success("Context action: Disable");
  };

  const onEditPhoto = () => {
    toastr.success("Context action: Edit Photo");
  };

  const onEnableClick = () => {
    toastr.success("Context action: Enable");
  };
  
  const onReassignDataClick = () => {
    toastr.success("Context action: Reassign data");
  };
  
  const onDeletePersonalDataClick = user => {
    toastr.success("Context action: Delete personal data");
  };
  
  const onDeleteProfileClick = () => {
    toastr.success("Context action: Delete profile");
  };
  
  const onInviteAgainClick = () => {
    toastr.success("Context action: Invite again");
  };

  const getUserContextOptions = (user, viewer) => {

    let status = "";

    if(isAdmin|| (!isAdmin && isMe(user, viewer.userName))) {
      status = getUserStatus(user); 
    }

    switch (status) {
      case "normal":
      case "unknown":
        return [
          {
            key: "edit",
            label: "Edit profile",
            onClick: onEditClick.bind(this, user)
          },
          {
            key: "edit-photo",
            label: "Edit Photo",
            onClick: onEditPhoto
          },
          {
            key: "change-email",
            label: "Change e-mail",
            onClick: onChangeEmailClick
          },
          {
            key: "change-phone",
            label: "Change phone",
            onClick: onChangePhoneClick
          },
          {
            key: "change-password",
            label: "Change password",
            onClick: onChangePasswordClick
          },
          {
            key: "disable",
            label: "Disable",
            onClick: onDisableClick
          }
        ];
      case "disabled":
        return [
          {
            key: "enable",
            label: "Enable",
            onClick: onEnableClick
          },
          {
            key: "edit-photo",
            label: "Edit photo",
            onClick: onEditPhoto
          },
          {
            key: "reassign-data",
            label: "Reassign data",
            onClick: onReassignDataClick
          },
          {
            key: "delete-personal-data",
            label: "Delete personal data",
            onClick: onDeletePersonalDataClick.bind(this, user)
          },
          {
            key: "delete-profile",
            label: "Delete profile",
            onClick: onDeleteProfileClick
          }
        ];
      case "pending":
        return [
          {
            key: "edit",
            label: "Edit",
            onClick: onEditClick.bind(this, user)
          },
          {
            key: "edit-photo",
            label: "Edit Photo",
            onClick: onEditPhoto
          },
          {
            key: "invite-again",
            label: "Invite again",
            onClick: onInviteAgainClick
          },
          {
            key: "key5",
            label: "Disable",
            onClick: onDisableClick
          }
        ];
      default:
        return [];
    }
  };

  const contextOptions = () => getUserContextOptions(profile, viewer);

  return (
    <div style={wrapperStyle}>
      <div style={{ width: "16px" }}>
        <IconButton
          iconName={"ArrowPathIcon"}
          color="#A3A9AE"
          size="16"
          onClick={() => history.push(settings.homepage)}
        />
      </div>
      <Text.ContentHeader truncate={true} style={textStyle}>
        {profile.displayName}
        {profile.isLDAP && " (LDAP)"}
      </Text.ContentHeader>
      {(isAdmin || isMe(viewer, profile.userName)) && (
        <ContextMenuButton
          directionX="right"
          title="Actions"
          iconName="VerticalDotsIcon"
          size={16}
          color="#A3A9AE"
          getData={contextOptions}
          isDisabled={false}
        />
      )}
    </div>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    viewer: state.auth.user,
    isAdmin: isAdmin(state.auth.user)
  };
}

export default connect(mapStateToProps)(withRouter(SectionHeaderContent));
