import React, { useCallback } from "react";
import { withRouter } from "react-router";
import { useTranslation } from 'react-i18next';
import {
  Text,
  Avatar,
  Button,
  ToggleContent,
  IconButton
} from "asc-web-components";
import { connect } from "react-redux";
import styled from 'styled-components';
import { getUserRole, getUserContacts } from "../../../../../store/people/selectors";
import { isAdmin, isMe } from "../../../../../store/auth/selectors";
import { updateProfileCulture } from "../../../../../store/profile/actions";
import ProfileInfo from "./ProfileInfo/ProfileInfo"

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

const ContactTextTruncate = styled.div`
  padding: 0 8px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const ToggleWrapper = styled.div`
  width: 100%;
  ${props => props.isSelf && `margin-bottom: 24px;`}
  ${props => props.isContacts && `margin-top: 24px;`}
`;

const ContactWrapper = styled.div`
  display: inline-flex;
  width: 300px;
`;

const createContacts = contacts => {
  const styledContacts = contacts.map((contact, index) => {
    return (
      <ContactWrapper key={index}>
        <IconButton color="#333333" size={16} iconName={contact.icon} isFill={true} />
        <ContactTextTruncate>{contact.value}</ContactTextTruncate>
      </ContactWrapper>
    );
  });

  return styledContacts;
};

const SectionBodyContent = props => {
  const { t } = useTranslation();
  const { profile, updateProfileCulture, history, settings, isAdmin, viewer } = props;

  const contacts = profile.contacts && getUserContacts(profile.contacts);
  const role = getUserRole(profile);
  const socialContacts = (contacts && contacts.social && contacts.social.length > 0 && createContacts(contacts.social)) || null;
  const infoContacts = contacts && createContacts(contacts.contact);
  const isSelf = isMe(viewer, profile.userName);

  const onEditSubscriptionsClick = useCallback(
    () => console.log("Edit subscriptions onClick()"),
    []
  );

  const onEditProfileClick = useCallback(
    () => history.push(`${settings.homepage}/edit/${profile.userName}`),
    [history, settings.homepage, profile.userName]
  );

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
              onClick={onEditProfileClick}
            />
          </EditButtonWrapper>
        )}
      </AvatarWrapper>
      <ProfileInfo profile={profile} updateProfileCulture={updateProfileCulture} isSelf={isSelf} isAdmin={isAdmin} t={t} cultures={settings.cultures} culture={settings.culture} />
      {isSelf && (
        <ToggleWrapper isSelf={true} >
          <ToggleContent label={t('Subscriptions')} isOpen={true} >
            <Text.Body as="span">
              <Button
                size="big"
                label={t('EditSubscriptionsBtn')}
                primary={true}
                onClick={onEditSubscriptionsClick}
              />
            </Text.Body>
          </ToggleContent>
        </ToggleWrapper>
      )}
      {profile.notes && (
        <ToggleWrapper>
          <ToggleContent label={t('Comments')} isOpen={true} >
            <Text.Body as="span">{profile.notes}</Text.Body>
          </ToggleContent>
        </ToggleWrapper>
      )}
      {profile.contacts && (
        <ToggleWrapper isContacts={true} >
          <ToggleContent label={t('ContactInformation')} isOpen={true} >
            <Text.Body as="span">{infoContacts}</Text.Body>
          </ToggleContent>
        </ToggleWrapper>
      )}
      {socialContacts && (
        <ToggleWrapper isContacts={true} >
          <ToggleContent label={t('SocialProfiles')} isOpen={true} >
            <Text.Body as="span">{socialContacts}</Text.Body>
          </ToggleContent>
        </ToggleWrapper>
      )}
    </ProfileWrapper>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    isAdmin: isAdmin(state.auth.user),
    viewer: state.auth.user
  };
}

export default connect(mapStateToProps, { updateProfileCulture })(withRouter(SectionBodyContent));
