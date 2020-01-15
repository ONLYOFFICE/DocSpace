import {
  Avatar,
  Button,
  IconButton,
  Text,
  ToggleContent,
  Link
} from "asc-web-components";
import { getUserContacts, getUserRole } from "../../../../../store/people/selectors";

import ProfileInfo from "./ProfileInfo/ProfileInfo";
import React from "react";
import { connect } from "react-redux";
import { store } from 'asc-web-common';
import styled from 'styled-components';
import { updateProfileCulture } from "../../../../../store/profile/actions";
import { withRouter } from "react-router";
import { withTranslation } from 'react-i18next';

const { isAdmin, isMe } = store.auth.selectors;

const ProfileWrapper = styled.div`
  display: flex;
  align-items: flex-start;
  flex-direction: row;
  flex-wrap: wrap;
`;

const AvatarWrapper = styled.div`
  margin-right: 32px;
  margin-bottom: 24px;
`;

const EditButtonWrapper = styled.div`
  margin-top: 16px;
  width: 160px;

  & > button {
    padding: 8px 20px 9px 20px;
  }
`;

const ToggleWrapper = styled.div`
  width: 100%;
  ${props => props.isSelf && `margin-bottom: 24px;`}
  ${props => props.isContacts && `margin-top: 24px;`}
  max-width: 1024px;
`;

const ContactWrapper = styled.div`
  display: inline-flex;
  width: 300px;
  margin-bottom: 12px;

  .contact-link {
    padding: 0 8px;
  }
`;

const createContacts = contacts => {
  const styledContacts = contacts.map((contact, index) => {
    let url = null;
    if (contact.link && contact.link.length > 0) {
      url = stringFormat(contact.link, [contact.value]);
    }
    return (
      <ContactWrapper key={index}>
        <IconButton color="#333333" size={16} iconName={contact.icon} isFill={true} />
        <Link
          className='contact-link'
          isTextOverflow
          href={url}
        >
          {contact.value}
        </Link>
      </ContactWrapper>
    );
  });

  return styledContacts;
};

const stringFormat = (string, data) => string.replace(/\{(\d+)\}/g, (m, n) => data[n] || m);

class SectionBodyContent extends React.PureComponent {

  onEditSubscriptionsClick = () => console.log("Edit subscriptions onClick()");

  onEditProfileClick = () => this.props.history.push(`${this.props.settings.homepage}/edit/${this.props.profile.userName}`);

  render() {
    const { profile, updateProfileCulture, settings, isAdmin, viewer, t } = this.props;

    const contacts = profile.contacts && getUserContacts(profile.contacts);
    const role = getUserRole(profile);
    const socialContacts = (contacts && contacts.social && contacts.social.length > 0 && createContacts(contacts.social)) || null;
    const infoContacts = contacts && createContacts(contacts.contact);
    const isSelf = isMe(viewer, profile.userName);

    return (
      <ProfileWrapper>
        <AvatarWrapper>
          <Avatar
            size="max"
            role={role}
            source={profile.avatarMax}
            userName={profile.displayName}
          />
          {(isAdmin || isSelf) && (
            <EditButtonWrapper>
              <Button
                size="big"
                scale={true}
                label={t("EditUserDialogTitle")}
                title={t("EditUserDialogTitle")}
                onClick={this.onEditProfileClick}
              />
            </EditButtonWrapper>
          )}
        </AvatarWrapper>
        <ProfileInfo profile={profile} updateProfileCulture={updateProfileCulture} isSelf={isSelf} isAdmin={isAdmin} t={t} cultures={settings.cultures} culture={settings.culture} />
        {(isSelf && false) && (
          <ToggleWrapper isSelf={true} >
            <ToggleContent label={t('Subscriptions')} isOpen={true} >
              <Text as="span">
                <Button
                  size="big"
                  label={t('EditSubscriptionsBtn')}
                  primary={true}
                  onClick={this.onEditSubscriptionsClick}
                />
              </Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {profile.notes && (
          <ToggleWrapper>
            <ToggleContent label={t('Comments')} isOpen={true} >
              <Text as="span">{profile.notes}</Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {profile.contacts && (
          <ToggleWrapper isContacts={true} >
            <ToggleContent label={t('ContactInformation')} isOpen={true} >
              <Text as="span">{infoContacts}</Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {socialContacts && (
          <ToggleWrapper isContacts={true} >
            <ToggleContent label={t('SocialProfiles')} isOpen={true} >
              <Text as="span">{socialContacts}</Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
      </ProfileWrapper>
    );
  }
}

const mapStateToProps = state => {
  return {
    settings: state.auth.settings,
    profile: state.profile.targetUser,
    isAdmin: isAdmin(state.auth.user),
    viewer: state.auth.user
  };
}

export default connect(mapStateToProps, { updateProfileCulture })(withRouter(withTranslation()(SectionBodyContent)));
