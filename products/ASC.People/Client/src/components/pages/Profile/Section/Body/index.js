import React, { useCallback } from "react";
import { withRouter } from "react-router";
import { useTranslation } from 'react-i18next';
import {
  Text,
  Avatar,
  Button,
  ToggleContent,
  IconButton,
  Link
} from "asc-web-components";
import { connect } from "react-redux";
import styled from 'styled-components';
import {
  getUserRole,
  getUserContacts
} from "../../../../../store/people/selectors";
import { isAdmin, isMe } from "../../../../../store/auth/selectors";

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

const InfoContainer = styled.div`
  margin-bottom: 24px;
`;

const InfoItem = styled.div`
  font-family: Open Sans;
  font-style: normal;
  font-weight: normal;
  font-size: 13px;
  line-height: 24px;
  display: flex;
  width: 400px;
`;

const InfoItemLabel = styled.div`
  width: 120px;
  white-space: nowrap;
  color: #A3A9AE;
`;

const InfoItemValue = styled.div`
  width: 220px;
`;

const IconButtonWrapper = styled.div`
  ${props => props.isBefore 
    ? `margin-right: 8px;` 
    : `margin-left: 8px;`
  }

  display: inline-flex;

  :hover {
    & > div > svg > path {
      fill: #3B72A7;
    }
  }
`;

const getFormattedDepartments = departments => {
  const splittedDepartments = departments.split(",");
  const departmentsLength = splittedDepartments.length - 1;
  const formattedDepartments = splittedDepartments.map((department, index) => {
    return (
      <span key={index}>
        <Link type="page" fontSize={13} isHovered={true}>
          {department.trim()}
        </Link>
        {departmentsLength !== index ? ", " : ""}
      </span>
    );
  });

  return formattedDepartments;
};

const capitalizeFirstLetter = string => {
  return string && string.charAt(0).toUpperCase() + string.slice(1);
};

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

const ProfileInfo = (props) => {
  const { isVisitor, email, activationStatus, department, title, mobilePhone, sex, workFrom, birthday, location, cultureName, currentCulture } = props.profile;
  const isAdmin = props.isAdmin;
  const isSelf = props.isSelf;
  const t = props.t;

  const type = isVisitor ? "Guest" : "Employee";
  const language = cultureName || currentCulture;
  const workFromDate = new Date(workFrom).toLocaleDateString(language);
  const birthDayDate = new Date(birthday).toLocaleDateString(language);
  const formatedSex = capitalizeFirstLetter(sex);
  const formatedDepartments = getFormattedDepartments(department);

  const onEmailClick = useCallback(
    () => window.open("mailto:" + email),
    [email]
  );

  return (
    <InfoContainer>
      <InfoItem>
        <InfoItemLabel>
          {t('UserType')}:
        </InfoItemLabel>
        <InfoItemValue>
          {type}
        </InfoItemValue>
      </InfoItem>
      {email &&
        <InfoItem>
          <InfoItemLabel>
            {t('Email')}:
          </InfoItemLabel>
          <InfoItemValue>
            <Link
              type="page"
              fontSize={13}
              isHovered={true}
              title={email}
              onClick={onEmailClick}
            >
              {activationStatus === 2 && (isAdmin || isSelf) &&
                <IconButtonWrapper isBefore={true} title={t('PendingTitle')}>
                  <IconButton color='#C96C27' size={16} iconName='DangerIcon' isFill={true} />
                </IconButtonWrapper>
              }
              {email}
              {(isAdmin || isSelf) &&
                <IconButtonWrapper title={t('EmailChangeButton')} >
                  <IconButton color="#A3A9AE" size={16} iconName='AccessEditIcon' isFill={true} />
                </IconButtonWrapper>
              }
              {activationStatus === 2 && (isAdmin || isSelf) &&
                <IconButtonWrapper title={t('SendInviteAgain')}>
                  <IconButton color="#A3A9AE" size={16} iconName='FileActionsConvertIcon' isFill={true} />
                </IconButtonWrapper>
              }
            </Link>
          </InfoItemValue>
        </InfoItem>
      }
      {department &&
        <InfoItem>
          <InfoItemLabel>
            Department:
          </InfoItemLabel>
          <InfoItemValue>
            {formatedDepartments}
          </InfoItemValue>
        </InfoItem>
      }
      {title &&
        <InfoItem>
          <InfoItemLabel>
            Position:
          </InfoItemLabel>
          <InfoItemValue>
            {title}
          </InfoItemValue>
        </InfoItem>
      }
      {(mobilePhone || isSelf) &&
        <InfoItem>
          <InfoItemLabel>
          {t('PhoneLbl')}:
          </InfoItemLabel>
          <InfoItemValue>
            {mobilePhone}
            {(isAdmin || isSelf) &&
            <IconButtonWrapper title={t('PhoneChange')} >
              <IconButton color="#A3A9AE" size={16} iconName='AccessEditIcon' isFill={true} />
            </IconButtonWrapper>
            }
          </InfoItemValue>
        </InfoItem>
      }
      {sex &&
        <InfoItem>
          <InfoItemLabel>
            {t('Sex')}:
          </InfoItemLabel>
          <InfoItemValue>
            {formatedSex}
          </InfoItemValue>
        </InfoItem>
      }
      {workFrom &&
        <InfoItem>
          <InfoItemLabel>
            Employed since:
          </InfoItemLabel>
          <InfoItemValue>
            {workFromDate}
          </InfoItemValue>
        </InfoItem>
      }
      {birthday &&
        <InfoItem>
          <InfoItemLabel>
            {t('Birthdate')}:
          </InfoItemLabel>
          <InfoItemValue>
            {birthDayDate}
          </InfoItemValue>
        </InfoItem>
      }
      {location &&
        <InfoItem>
          <InfoItemLabel>
            {t('Location')}:
          </InfoItemLabel>
          <InfoItemValue>
            {location}
          </InfoItemValue>
        </InfoItem>
      }
      {isSelf &&
        <InfoItem>
          <InfoItemLabel>
            {t('Language')}:
          </InfoItemLabel>
          <InfoItemValue>
            {language}
          </InfoItemValue>
        </InfoItem>
      }
    </InfoContainer>
  );
};

const SectionBodyContent = props => {
  const { t } = useTranslation();
  const { profile, history, settings, isAdmin, viewer } = props;

  const contacts = profile.contacts && getUserContacts(profile.contacts);
  const role = getUserRole(profile);
  const socialContacts = contacts && createContacts(contacts.social);
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
              label={t("EditUserDialogTitle")}
              onClick={onEditProfileClick}
            />
          </EditButtonWrapper>
        )}
      </AvatarWrapper>
      <ProfileInfo profile={profile} isSelf={isSelf} isAdmin={isAdmin} t={t}/>
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
      {profile.contacts && (
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

export default connect(mapStateToProps)(withRouter(SectionBodyContent));
