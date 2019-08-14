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

  const getUserContextOptions = user => {
    return [
      {
        key: "key1",
        label: "Edit profile",
        onClick: onEditClick.bind(this, user)
      },
      {
        key: "key2",
        label: "Change e-mail",
        onClick: onChangeEmailClick
      },
      {
        key: "key3",
        label: "Change phone",
        onClick: onChangePhoneClick
      },
      {
        key: "key4",
        label: "Change password",
        onClick: onChangePasswordClick
      },
      {
        key: "key5",
        label: "Disable",
        onClick: onDisableClick
      }
    ];
  };

  const contextOptions = () => getUserContextOptions(profile);

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
