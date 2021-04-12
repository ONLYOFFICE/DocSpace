import Avatar from "@appserver/components/avatar";
import Button from "@appserver/components/button";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import ToggleContent from "@appserver/components/toggle-content";
import Link from "@appserver/components/link";
import ProfileInfo from "./ProfileInfo/ProfileInfo";
import React from "react";
import { combineUrl, isMe } from "@appserver/common/utils";
import styled from "styled-components";

import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import {
  getUserContacts,
  getUserRole,
} from "../../../../helpers/people-helpers";
import config from "../../../../../package.json";
import { AppServerConfig } from "@appserver/common/constants";
import { Trans, useTranslation } from "react-i18next";
import { ResetApplicationDialog } from "../../../../components/dialogs";

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

const ToggleWrapper = styled.div`
  width: 100%;
  min-width: 100%;
  ${(props) => props.isSelf && `margin-bottom: 24px;`}
  ${(props) => props.isContacts && `margin-top: 24px;`}
  max-width: 1024px;
`;

const ContactWrapper = styled.div`
  display: inline-flex;
  width: 300px;
  margin-bottom: 12px;

  .icon-button {
    min-width: 16px;
  }

  .contact-link {
    padding: 0 8px;
    line-height: 16px;
  }
`;

const LinkActionWrapper = styled.div`
  margin-top: 17px;
  .link-action {
    margin-right: 5px;
  }
`;

const createContacts = (contacts) => {
  const styledContacts = contacts.map((contact, index) => {
    let url = null;
    if (contact.link && contact.link.length > 0) {
      url = stringFormat(contact.link, [contact.value]);
    }
    return (
      <ContactWrapper key={index}>
        <IconButton
          className="icon-button"
          color="#333333"
          size={16}
          iconName={contact.icon}
          isFill={true}
        />
        <Link className="contact-link" isTextOverflow href={url}>
          {contact.value}
        </Link>
      </ContactWrapper>
    );
  });

  return styledContacts;
};

const stringFormat = (string, data) =>
  string.replace(/\{(\d+)\}/g, (m, n) => data[n] || m);

class SectionBodyContent extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = { dialogVisible: false };
  }
  componentDidMount() {
    const { cultures, getPortalCultures, profile, viewer, isSelf } = this.props;
    //const isSelf = isMe(viewer, profile.userName);
    if (isSelf && !cultures.length) {
      getPortalCultures();
    }
  }

  onEditSubscriptionsClick = () => console.log("Edit subscriptions onClick()");

  onEditProfileClick = () => {
    this.props.avatarMax && this.props.setAvatarMax(null);

    this.props.history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/edit/${this.props.profile.userName}`
      )
    );
  };

  toggleDialogVisible = () => {
    this.setState({ dialogVisible: !this.state.dialogVisible });
  };

  render() {
    const { dialogVisible } = this.state;
    const {
      profile,
      cultures,
      culture,
      isAdmin,
      viewer,
      t,
      isSelf,
    } = this.props;

    const contacts = profile.contacts && getUserContacts(profile.contacts);
    const role = getUserRole(profile);
    const socialContacts =
      (contacts &&
        contacts.social &&
        contacts.social.length > 0 &&
        createContacts(contacts.social)) ||
      null;
    const infoContacts = contacts && createContacts(contacts.contact);
    //const isSelf = isMe(viewer, profile.userName);

    return (
      <ProfileWrapper>
        <AvatarWrapper>
          <Avatar
            size="max"
            role={role}
            source={profile.avatarMax}
            userName={profile.displayName}
          />
          {profile.status !== 2 && (isAdmin || isSelf) && (
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
        <ProfileInfo
          profile={profile}
          isSelf={isSelf}
          isAdmin={isAdmin}
          t={t}
          cultures={cultures}
          culture={culture}
        />
        {isSelf && false && (
          <ToggleWrapper isSelf={true}>
            <ToggleContent label={t("Subscriptions")} isOpen={true}>
              <Text as="span">
                <Button
                  size="big"
                  label={t("EditSubscriptionsBtn")}
                  primary={true}
                  onClick={this.onEditSubscriptionsClick}
                />
              </Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {isSelf && (
          <ToggleWrapper>
            <ToggleContent label={t("LoginSettings")} isOpen={true}>
              <Trans t={t} i18nKey="TwoFactorDescription" ns="Profile">
                <Text>
                  <strong>Two-factor authentication</strong> via code generating
                  application was enabled for all users by cloud service
                  administrator.
                </Text>
              </Trans>
              <LinkActionWrapper>
                <Link
                  type="action"
                  isHovered={true}
                  className="link-action"
                  onClick={this.toggleDialogVisible}
                >
                  {t("ResetApplication")}
                </Link>
                <Link type="action" isHovered={true} className="link-action">
                  {t("ShowBackupCodes")}
                </Link>
              </LinkActionWrapper>
            </ToggleContent>
          </ToggleWrapper>
        )}

        {profile.notes && (
          <ToggleWrapper>
            <ToggleContent label={t("Comments")} isOpen={true}>
              <Text as="span">{profile.notes}</Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {profile.contacts && (
          <ToggleWrapper isContacts={true}>
            <ToggleContent label={t("ContactInformation")} isOpen={true}>
              <Text as="span">{infoContacts}</Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {socialContacts && (
          <ToggleWrapper isContacts={true}>
            <ToggleContent label={t("SocialProfiles")} isOpen={true}>
              <Text as="span">{socialContacts}</Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {dialogVisible && (
          <ResetApplicationDialog
            visible={dialogVisible}
            onClose={this.toggleDialogVisible}
          />
        )}
      </ProfileWrapper>
    );
  }
}

export default withRouter(
  inject(({ auth, peopleStore }) => ({
    cultures: auth.settingsStore.cultures,
    culture: auth.settingsStore.culture,
    getPortalCultures: auth.settingsStore.getPortalCultures,
    isAdmin: auth.isAdmin,
    profile: peopleStore.targetUserStore.targetUser,
    viewer: auth.userStore.user,
    isTabletView: auth.settingsStore.isTabletView,
    isSelf: peopleStore.targetUserStore.isMe,
    avatarMax: peopleStore.avatarEditorStore.avatarMax,
    setAvatarMax: peopleStore.avatarEditorStore.setAvatarMax,
  }))(observer(withTranslation("Profile")(SectionBodyContent)))
);
