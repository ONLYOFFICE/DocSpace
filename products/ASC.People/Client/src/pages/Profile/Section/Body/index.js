import Avatar from "@appserver/components/avatar";
import Button from "@appserver/components/button";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import SocialButton from "@appserver/components/social-button";
import FacebookButton from "@appserver/components/facebook-button";
import ToggleContent from "@appserver/components/toggle-content";
import Link from "@appserver/components/link";
import ProfileInfo from "./ProfileInfo/ProfileInfo";
import toastr from "studio/toastr";
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
import { AppServerConfig, providersData } from "@appserver/common/constants";
import { unlinkOAuth, linkOAuth } from "@appserver/common/api/people";
import { getAuthProviders } from "@appserver/common/api/settings";

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

const ProviderButtonsWrapper = styled.div`
  align-items: center;
  display: grid;
  grid-template-columns: auto 1fr;
  grid-gap: 16px 22px;
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
  async componentDidMount() {
    const {
      cultures,
      getPortalCultures,
      profile,
      viewer,
      isSelf,
      setProviders,
    } = this.props;
    //const isSelf = isMe(viewer, profile.userName);
    if (isSelf && !cultures.length) {
      getPortalCultures();
    }

    if (!isSelf) return;
    try {
      await getAuthProviders().then((providers) => {
        setProviders(providers);
      });
    } catch (e) {
      console.error(e);
    }

    window.loginCallback = this.loginCallback;
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

  loginCallback = (profile) => {
    const { setProviders, t } = this.props;
    linkOAuth(profile.Serialized).then((resp) => {
      getAuthProviders().then((providers) => {
        setProviders(providers);
        toastr.success(t("ProviderSuccessfullyConnected"));
      });
    });
  };

  unlinkAccount = (providerName) => {
    const { setProviders, t } = this.props;
    unlinkOAuth(providerName).then(() => {
      getAuthProviders().then((providers) => {
        setProviders(providers);
        toastr.success(t("ProviderSuccessfullyDisconnected"));
      });
    });
  };

  linkAccount = (providerName, link, e) => {
    const { getOAuthToken, getLoginLink } = this.props;
    e.preventDefault();

    try {
      const tokenGetterWin = window.open(
        link,
        "login",
        "width=800,height=500,status=no,toolbar=no,menubar=no,resizable=yes,scrollbars=no"
      );

      getOAuthToken(tokenGetterWin).then((code) => {
        const token = window.btoa(
          JSON.stringify({
            auth: providerName,
            mode: "popup",
            callback: "loginCallback",
          })
        );

        tokenGetterWin.location.href = getLoginLink(token, code);
      });
    } catch (err) {
      console.log(err);
    }
  };

  providerButtons = () => {
    const { t, providers } = this.props;

    const providerButtons =
      providers &&
      providers.map((item) => {
        const { icon, label, iconOptions } = providersData[item.provider];

        if (!icon || !label) return <React.Fragment></React.Fragment>;
        return (
          <React.Fragment key={`${item.provider}ProviderItem`}>
            <div>
              {item.provider === "Facebook" ? (
                <FacebookButton
                  noHover={true}
                  iconName={icon}
                  label={t(label)}
                  className="socialButton"
                  $iconOptions={iconOptions}
                />
              ) : (
                <SocialButton
                  noHover={true}
                  iconName={icon}
                  label={t(label)}
                  className="socialButton"
                  $iconOptions={iconOptions}
                />
              )}
            </div>
            {item.linked ? (
              <div>
                <Link
                  type="action"
                  color="A3A9AE"
                  onClick={(e) => this.unlinkAccount(item.provider, e)}
                  isHovered={true}
                >
                  {t("Disconnect")}
                </Link>
              </div>
            ) : (
              <div>
                <Link
                  type="action"
                  color="A3A9AE"
                  onClick={(e) => this.linkAccount(item.provider, item.url, e)}
                  isHovered={true}
                >
                  {t("Connect")}
                </Link>
              </div>
            )}
          </React.Fragment>
        );
      });

    return providerButtons;
  };

  render() {
    const {
      profile,
      cultures,
      culture,
      isAdmin,
      viewer,
      t,
      isSelf,
      providers,
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

        {isSelf && providers && providers.length > 0 && (
          <ToggleWrapper>
            <ToggleContent label={t("LoginSettings")} isOpen={true}>
              <ProviderButtonsWrapper>
                {this.providerButtons()}
              </ProviderButtonsWrapper>
            </ToggleContent>
          </ToggleWrapper>
        )}
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
    providers: peopleStore.usersStore.providers,
    setProviders: peopleStore.usersStore.setProviders,
    getOAuthToken: auth.settingsStore.getOAuthToken,
    getLoginLink: auth.settingsStore.getLoginLink,
  }))(observer(withTranslation("Profile")(SectionBodyContent)))
);
