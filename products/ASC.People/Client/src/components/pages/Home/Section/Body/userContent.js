import React, { useCallback } from "react";
import { withRouter } from "react-router";
import { RowContent, Link, Icons } from "asc-web-components";
import { connect } from "react-redux";
import { getUserStatus } from "../../../../../store/people/selectors";
import { useTranslation } from 'react-i18next';
import { headOfDepartment } from './../../../../../helpers/customNames';

const UserContent = ({ user, history, settings }) => {
  const { userName, displayName, headDepartment, department, mobilePhone, email } = user;
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

  const nameColor = status === 'pending' ? '#A3A9AE' : '#333333';
  const sideInfoColor = status === 'pending' ? '#D0D5DA' : '#A3A9AE';
  const t = useTranslation();

  return (
    <RowContent>
      <Link type='page' title={displayName} isBold={true} fontSize={15} color={nameColor} onClick={onUserNameClick} >{displayName}</Link>
      <>
        {status === 'pending' && <Icons.SendClockIcon size='small' isfill={true} color='#3B72A7' />}
        {status === 'disabled' && <Icons.CatalogSpamIcon size='small' isfill={true} color='#3B72A7' />}
      </>
      {headDepartment
        ? <Link type='page' title={t('CustomHeadOfDepartment', { headOfDepartment })} fontSize={12} color={sideInfoColor} onClick={onHeadDepartmentClick} >{t('CustomHeadOfDepartment', { headOfDepartment })}</Link>
        : <></>
      }
      <Link type='action' title={department} fontSize={12} color={sideInfoColor} onClick={onDepartmentClick} >{department}</Link>
      <Link type='page' title={mobilePhone} fontSize={12} color={sideInfoColor} onClick={onPhoneClick} >{mobilePhone}</Link>
      <Link type='page' title={email} fontSize={12} color={sideInfoColor} onClick={onEmailClick} >{email}</Link>
    </RowContent>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(withRouter(UserContent));
