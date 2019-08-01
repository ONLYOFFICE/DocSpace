import React, { useCallback } from 'react';
import { withRouter } from 'react-router';
import { Text, Avatar, Button, ToggleContent, IconButton, Link } from 'asc-web-components';
import { connect } from 'react-redux';
import { getUserRole, getContacts } from '../../../../../store/people/selectors';

const profileWrapper = {
  display: "flex",
  alignItems: "flex-start",
  flexDirection: "row",
  flexWrap: "wrap"
};

const avatarWrapper = {
  marginRight: "32px",
  marginBottom: "24px"
};

const editButtonWrapper = {
  marginTop: "16px",
  width: "160px"
};

const infoWrapper = {
  display: "inline-flex",
  marginBottom: "24px"
};

const textTruncate = {
  padding: "0 8px",
  whiteSpace: "nowrap",
  overflow: "hidden",
  textOverflow: "ellipsis"
};

const titlesWrapper = {
  marginRight: "8px"
};

const restMargins = {
  marginBottom: "0",
  marginBlockStart: "5px",
  marginBlockEnd: "0px",
};

const notesWrapper = {
  display: "block",
  marginTop: "24px",
  width: "100%"
};

const marginTop24 = {
  marginTop: "24px"
};

const marginTop22 = {
  marginTop: "22px"
};

const marginTop10 = {
  marginTop: "10px"
};

const marginLeft18 = {
  marginLeft: "18px"
};

const selfToggleWrapper = {
  width: "100%",
  marginBottom: "24px"
}

const contactsToggleWrapper = {
  width: "100%",
  marginTop: "24px"
};

const notesToggleWrapper = {
  width: "100%"
};

const contactWrapper = {
  display: "inline-flex",
  width: "300px"
};

const getFormattedDate = (date) => {
  if (!date) return;
  let d = date.slice(0, 10).split('-');
  return d[1] + '.' + d[2] + '.' + d[0];
};

const getFormattedDepartments = (departments) => {
  const splittedDepartments = departments.split(',');
  const departmentsLength = splittedDepartments.length - 1;
  const formattedDepartments = splittedDepartments.map((department, index) => {
    return (
      <span key={index}>
        <Link type="action" fontSize={13} isHovered={true} >
          {department.trim()}
        </Link>
        {(departmentsLength !== index) ? ', ' : ''}
      </span>
    )
  });

  return formattedDepartments;
};

const capitalizeFirstLetter = (string) => {
  if (!string) return;
  return string.charAt(0).toUpperCase() + string.slice(1);
};

const createContacts = (contacts) => {
  return contacts.map((contact, index) => {
    return (
      <div key={index} style={contactWrapper}>
        <IconButton color="#333333" size={16} iconName={contact.icon} isFill={true} />
        <div style={textTruncate}>{contact.value}</div>
      </div>
    );
  });
};

const SectionBodyContent = (props) => {
  const { profile, history, isSelf, settings } = props;
  const contacts = getContacts(profile.contacts);

  const onEmailClick = useCallback(
    () => window.open('mailto:' + profile.email),
    [profile.email]
  );

  const onEditSubscriptionsClick = useCallback(
    () => console.log('Edit subscriptions onClick()'),
    []
  );

  const onBecomeAffiliateClick = useCallback(
    () => console.log('Become our Affiliate onClick()'),
    []
  );

  const onEditProfileClick = useCallback(
    () => history.push(`${settings.homepage}/edit/${profile.userName}`),
    [history, settings.homepage, profile.userName]
  );

  console.log(profile);

  return (
    <div style={profileWrapper}>
      <div style={avatarWrapper}>
        <Avatar
          size="max"
          role={getUserRole(profile)}
          source={profile.avatarMax}
          userName={profile.displayName}
        />
        <Button style={editButtonWrapper} size="big" label="Edit profile" onClick={onEditProfileClick} />
      </div>
      <div style={infoWrapper}>
        <div style={titlesWrapper}>
          <Text.Body style={restMargins} color='lightGray' title='Type'>Type:</Text.Body>
          {profile.email && <Text.Body style={restMargins} color='lightGray' title='E-mail'>E-mail:</Text.Body>}
          {profile.department && <Text.Body style={restMargins} color='lightGray' title='Department'>Department:</Text.Body>}
          {profile.title && <Text.Body style={restMargins} color='lightGray' title='Position'>Position:</Text.Body>}
          {profile.mobilePhone && <Text.Body style={restMargins} color='lightGray' title='Phone'>Phone:</Text.Body>}
          {profile.sex && <Text.Body style={restMargins} color='lightGray' title='Sex'>Sex:</Text.Body>}
          {profile.workFrom && <Text.Body style={restMargins} color='lightGray' title='Employed since'>Employed since:</Text.Body>}
          {profile.birthday && <Text.Body style={restMargins} color='lightGray' title='Date of birth'>Date of birth:</Text.Body>}
          {profile.location && <Text.Body style={restMargins} color='lightGray' title='Location'>Location:</Text.Body>}
          {isSelf && <Text.Body style={restMargins} color='lightGray' title='Language'>Language:</Text.Body>}
          {isSelf && <Text.Body style={marginTop24} color='lightGray' title='Affiliate status'>Affiliate status:</Text.Body>}
        </div>
        <div>
          <Text.Body style={restMargins}>{profile.isVisitor ? "Guest" : "Employee"}</Text.Body>
          <Text.Body style={restMargins}><Link type="page" fontSize={13} isHovered={true} onClick={onEmailClick} >{profile.email}</Link>{profile.activationStatus === 2 && ' (Pending)'}</Text.Body>
          <Text.Body style={restMargins}>{getFormattedDepartments(profile.department)}</Text.Body>
          <Text.Body style={restMargins}>{profile.title}</Text.Body>
          <Text.Body style={restMargins}>{profile.mobilePhone}</Text.Body>
          <Text.Body style={restMargins}>{capitalizeFirstLetter(profile.sex)}</Text.Body>
          <Text.Body style={restMargins}>{getFormattedDate(profile.workFrom)}</Text.Body>
          <Text.Body style={restMargins}>{getFormattedDate(profile.birthday)}</Text.Body>
          <Text.Body style={restMargins}>{profile.location}</Text.Body>
          {isSelf && <Text.Body style={restMargins}>{profile.cultureName}</Text.Body>}
          {isSelf && <Button style={marginTop22} size="base" label="Become our Affiliate" onClick={onBecomeAffiliateClick} />}
        </div>
      </div>
      {isSelf &&
        <div style={selfToggleWrapper}>
          <ToggleContent label="Login settings" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              Two-factor authentication via code generating application was enabled for all users by cloud service administrator.
              <div style={marginTop10}>
                <Link type="action" isBold={true} isHovered={true} fontSize={13} >{'Reset application'}</Link>
                <Link style={marginLeft18} type="action" isBold={true} isHovered={true} fontSize={13} >{'Show backup codes'}</Link>
              </div>
            </Text.Body>
          </ToggleContent>
        </div>
      }
      {isSelf &&
        <div style={selfToggleWrapper}>
          <ToggleContent label="Subscriptions" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              <Button size="big" label="Edit subscriptions" primary={true} onClick={onEditSubscriptionsClick} />
            </Text.Body>
          </ToggleContent>
        </div>
      }
      {profile.notes &&
        <div style={notesToggleWrapper}>
          <ToggleContent label="Comment" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              {profile.notes}
            </Text.Body>
          </ToggleContent>
        </div>
      }
      {profile.contacts &&
        <div style={contactsToggleWrapper}>
          <ToggleContent label="Contact information" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              {createContacts(contacts.contact)}
            </Text.Body>
          </ToggleContent>
        </div>
      }
      {profile.contacts &&
        <div style={contactsToggleWrapper}>
          <ToggleContent label="Social profiles" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              {createContacts(contacts.social)}
            </Text.Body>
          </ToggleContent>
        </div>
      }
    </div>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.settings
  };
}

export default connect(mapStateToProps)(withRouter(SectionBodyContent));