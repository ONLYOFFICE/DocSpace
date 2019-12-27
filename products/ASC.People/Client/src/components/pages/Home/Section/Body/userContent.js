import React, { useCallback } from "react";
import { withRouter } from "react-router";
import { RowContent, Link, LinkWithDropdown, Icons, Text } from "asc-web-components";
import { connect } from "react-redux";
import { getUserStatus } from "../../../../../store/people/selectors";
import { useTranslation } from 'react-i18next';
import { history } from "asc-web-common";

const getFormatedGroups = (user, status) => {
  let temp = [];
  const groups = user.groups;
  const linkColor = status === 'pending' ? '#D0D5DA' : '#A3A9AE';

  if (!groups) temp.push({ key: 0, label: '' });

  groups && groups.map(group =>
    temp.push(
      {
        key: group.id,
        label: group.name,
        onClick: () => history.push(`/products/people/filter?group=${group.id}`)
      }
    )
  );

  if (temp.length <= 1) {
    return (
      <Link
        isTextOverflow={true}
        containerWidth='160px'
        type='action'
        title={temp[0].label}
        fontSize='12px'
        fontWeight={600}
        color={linkColor}
        onClick={temp[0].onClick}
      >
        {temp[0].label}
      </Link>);
  } else {
    return (
      <LinkWithDropdown
        isTextOverflow={true}
        containerWidth='160px'
        title={temp[0].label}
        fontSize='12px'
        fontWeight={600}
        color={linkColor}
        data={temp}
      >
        {temp[0].label}
      </LinkWithDropdown>);
  }
};

const UserContent = ({ user, history, settings }) => {
  const { userName, displayName, title, mobilePhone, email } = user;
  const status = getUserStatus(user);
  const groups = getFormatedGroups(user, status);

  const onUserNameClick = useCallback(
    () => history.push(`${settings.homepage}/view/${userName}`),
    [history, settings.homepage, userName]
  );

  const onPhoneClick = useCallback(
    () => window.open(`sms:${mobilePhone}`),
    [mobilePhone]
  );

  const onEmailClick = useCallback(
    () => window.open(`mailto:${email}`),
    [email]
  );

  const nameColor = status === 'pending' ? '#A3A9AE' : '#333333';
  const sideInfoColor = ((status === 'pending') || (status === 'disabled')) ? '#D0D5DA' : '#A3A9AE';
  //const { t } = useTranslation();

  const headDepartmentStyle = {
    width: '110px'
  }

  return (
    <RowContent
      sideColor={sideInfoColor}
    >
      <Link type='page' title={displayName} fontWeight={600} fontSize='15px' color={nameColor} onClick={onUserNameClick} isTextOverflow={true} >{displayName}</Link>
      <>
        {status === 'pending' && <Icons.SendClockIcon size='small' isfill={true} color='#3B72A7' />}
        {status === 'disabled' && <Icons.CatalogSpamIcon size='small' isfill={true} color='#3B72A7' />}
      </>
      {title
        ?
          <Text
            style={headDepartmentStyle}
            as="div"
            color={sideInfoColor}
            fontSize='12px'
            fontWeight={600}
            title={title}
            truncate={true}
          >
            {title}
          </Text>
        : <div style={headDepartmentStyle}></div>
      }
      {groups}
      <Link type='page' title={mobilePhone} fontSize='12px' fontWeight={600} color={sideInfoColor} onClick={onPhoneClick} isTextOverflow={true}>{mobilePhone}</Link>
      <Link containerWidth='220px' type='page' title={email} fontSize='12px' fontWeight={600} color={sideInfoColor} onClick={onEmailClick} isTextOverflow={true}>{email}</Link>
    </RowContent>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(withRouter(UserContent));
