import React from 'react';
import { withRouter } from 'react-router';
import _ from "lodash";
import { Text, Avatar, Button, ToggleContent, IconButton, Link } from 'asc-web-components';
import { connect } from 'react-redux';

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

let socialProfiles = [
  { type: "facebook", value: "", icon: "ShareFacebookIcon" },
  { type: "livejournal", value: "", icon: "LivejournalIcon" },
  { type: "myspace", value: "", icon: "MyspaceIcon" },
  { type: "twitter", value: "", icon: "ShareTwitterIcon" },
  { type: "blogger", value: "", icon: "BloggerIcon" },
  { type: "yahoo", value: "", icon: "YahooIcon" }
];

let contacts = [
  { type: "mail", value: "", icon: "MailIcon" },
  { type: "phone", value: "", icon: "PhoneIcon" },
  { type: "mobphone", value: "", icon: "MobileIcon" },
  { type: "gmail", value: "", icon: "GmailIcon" },
  { type: "skype", value: "", icon: "SkypeIcon" },
  { type: "msn", value: "", icon: "WindowsMsnIcon" },
  { type: "icq", value: "", icon: "IcqIcon" },
  { type: "jabber", value: "", icon: "JabberIcon" },
  { type: "aim", value: "", icon: "AimIcon" }
];

let userContacts = [];
let userSocialProfiles = [];

const getUserRole = (user) => {
  if (user.isOwner) return "owner";
  if (user.isAdmin) return "admin";
  if (user.isVisitor) return "guest";
  return "user";
};

const getFormattedDate = (date) => {
  if (!date) return;
  let d = date.slice(0, 10).split('-');
  return d[1] + '.' + d[2] + '.' + d[0];
};

const getFormattedContacts = (profile) => {
  let filledUserContacts = _.merge({}, contacts, profile.contacts);
  userContacts = _.reject(filledUserContacts, (o) => { return o.icon === undefined; });

  let filledSocialProfiles = _.merge({}, socialProfiles, profile.contacts);
  userSocialProfiles = _.reject(filledSocialProfiles, (o) => { return o.icon === undefined; });
};

const getFormattedDepartments = (departments) => {
  let splittedDepartments = departments.split(',');
  const departmentsLength = splittedDepartments.length - 1;
  return splittedDepartments.map((department, index) => {
    return (
      <span key={index}>
        <Link type="action" fontSize={13} isHovered={true} >
          {department.trim()}
        </Link>
        {(departmentsLength !== index) ? ', ' : ''}
      </span>
    )
  });
};

const sendMail = (email) => {
  window.open('mailto:' + email);
};

const capitalizeFirstLetter = (string) => {
  if (!string) return;
  return string.charAt(0).toUpperCase() + string.slice(1);
};

const createContacts = (contacts) => {
  return contacts.map((contact, index) => {
    if (contact.value)
      return (
        <div key={index} style={{ display: "inline-flex", width: "300px" }}>
          <IconButton color="#333333" size={16} iconName={contact.icon} isFill={true} onClick={() => { }} />
          <div style={textTruncate}>{contact.value}</div>
        </div>
      );
  })
};

const SectionBodyContent = (props) => {
  const { profile, history, isSelf, settings} = props;

  getFormattedContacts(profile);

  console.log(props);

  return (
    <div style={profileWrapper}>
      <div style={avatarWrapper}>
        <Avatar
          size="max"
          role={getUserRole(profile)}
          source={profile.avatarMax}
          userName={profile.displayName}
        />
        <Button style={{ marginTop: "16px", width: '160px' }} size="big" label="Edit profile" onClick={() => history.push(`${settings.homepage}/edit/${profile.userName}`)} />
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
          {isSelf && <Text.Body style={{ marginTop: "24px" }} color='lightGray' title='Affiliate status'>Affiliate status:</Text.Body>}
        </div>
        <div>
          <Text.Body style={restMargins}>{profile.isVisitor ? "Guest" : "Employee"}</Text.Body>
          <Text.Body style={restMargins}><Link type="page" fontSize={13} isHovered={true} onClick={() => sendMail(profile.email)} >{profile.email}</Link>{profile.activationStatus === 2 && ' (Pending)'}</Text.Body>
          <Text.Body style={restMargins}>{getFormattedDepartments(profile.department)}</Text.Body>
          <Text.Body style={restMargins}>{profile.title}</Text.Body>
          <Text.Body style={restMargins}>{profile.mobilePhone}</Text.Body>
          <Text.Body style={restMargins}>{capitalizeFirstLetter(profile.sex)}</Text.Body>
          <Text.Body style={restMargins}>{getFormattedDate(profile.workFrom)}</Text.Body>
          <Text.Body style={restMargins}>{getFormattedDate(profile.birthday)}</Text.Body>
          <Text.Body style={restMargins}>{profile.location}</Text.Body>
          {isSelf && <Text.Body style={restMargins}>{profile.cultureName}</Text.Body>}
          {isSelf && <Button style={{ marginTop: "22px" }} size="base" label="Become our Affiliate" onClick={() => console.log('Become our Affiliate onClick()')} />}
        </div>
      </div>
      {isSelf &&
        <div style={{ width: "100%", marginBottom: "24px" }}>
          <ToggleContent label="Login settings" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              Two-factor authentication via code generating application was enabled for all users by cloud service administrator.
              <div style={{ marginTop: "10px" }}>
                <Link type="action" isBold={true} isHovered={true} fontSize={13} >{'Reset application'}</Link>
                <Link style={{ marginLeft: "18px" }} type="action" isBold={true} isHovered={true} fontSize={13} >{'Show backup codes'}</Link>
              </div>
            </Text.Body>
          </ToggleContent>
        </div>
      }
      {isSelf &&
        <div style={{ width: "100%", marginBottom: "24px" }}>
          <ToggleContent label="Subscriptions" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              <Button size="big" label="Edit subscriptions" primary={true} onClick={() => console.log('Edit subscriptions onClick()')} />
            </Text.Body>
          </ToggleContent>
        </div>
      }
      {profile.notes &&
        <div style={{ width: "100%" }}>
          <ToggleContent label="Comment" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              {profile.notes}
            </Text.Body>
          </ToggleContent>
        </div>
      }
      {profile.contacts &&
        <div style={{ width: "100%", marginTop: "24px" }}>
          <ToggleContent label="Contact information" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              {createContacts(userContacts)}
            </Text.Body>
          </ToggleContent>
        </div>
      }
      {profile.contacts &&
        <div style={{ width: "100%", marginTop: "24px" }}>
          <ToggleContent label="Social profiles" style={notesWrapper} isOpen={true}>
            <Text.Body tag="span">
              {createContacts(userSocialProfiles)}
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