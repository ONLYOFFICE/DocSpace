import React, { useCallback } from "react";
import { withRouter } from "react-router";
import { PeopleRow } from "asc-web-components";
import { connect } from "react-redux";
import { getUserStatus } from "../../../../../store/people/selectors";

const UserContent = ({user, history,settings }) => {
  const { userName, displayName, headDepartment, department, mobilePhone, email  } = user;
  const status = getUserStatus(user);

  const onUserNameClick = useCallback(() => {
    console.log("User name action");
    history.push(`${settings.homepage}/view/${userName}`);
  }, [history, settings.homepage, userName]);

  const onHeadDepartmentClick = useCallback(
    () => console.log("Head of department action"),
    []
  );

  const onDepartmentClick = useCallback(
    () => console.log("Department action"),
    []
  );

  const onPhoneClick = useCallback(
    () => console.log("Phone action"),
    []
  );

  const onEmailClick = useCallback(
    () => console.log("Email action"),
    []
  );

  return (
    <PeopleRow
      status={status}
      displayName={displayName}
      department={department}
      phone={mobilePhone}
      email={email}
      onDisplayNameClick={onUserNameClick} 
      onDepartmentClick={onDepartmentClick}
      onPhoneClick={onPhoneClick}
      onEmailClick={onEmailClick}
    />
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(withRouter(UserContent));
