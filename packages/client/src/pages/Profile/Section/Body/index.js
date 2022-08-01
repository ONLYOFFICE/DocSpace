import Avatar from "@docspace/components/avatar";
import Button from "@docspace/components/button";
import IconButton from "@docspace/components/icon-button";
import Text from "@docspace/components/text";
import SocialButton from "@docspace/components/social-button";
import FacebookButton from "@docspace/components/facebook-button";
import ToggleContent from "@docspace/components/toggle-content";
import ToggleButton from "@docspace/components/toggle-button";
import Link from "@docspace/components/link";
import ProfileInfo from "./ProfileInfo/ProfileInfo";
import toastr from "client/toastr";
import React from "react";
import { combineUrl, getProviderTranslation } from "@docspace/common/utils";
import styled from "styled-components";

import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import {
  getUserContacts,
  getUserRole,
} from "../../../../helpers/people-helpers";
import config from "PACKAGE_FILE";
import {
  AppServerConfig,
  providersData,
  ThemeKeys,
} from "@docspace/common/constants";
import { unlinkOAuth, linkOAuth } from "@docspace/common/api/people";
import { getAuthProviders } from "@docspace/common/api/settings";
import {
  ResetApplicationDialog,
  BackupCodesDialog,
} from "../../../../components/dialogs";

import Loaders from "@docspace/common/components/Loaders";
import withPeopleLoader from "../../../../HOCs/withPeopleLoader";
import RadioButtonGroup from "@docspace/components/radio-button-group";

const ProfileWrapper = styled.div`
  display: flex;
  align-items: flex-start;
  flex-direction: row;
  flex-wrap: wrap;

  .toggle-btn {
    position: relative;

    .toggle-button-text {
      margin-top: -1px;
    }
  }
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

  .link-action-reset {
    margin-right: 18px;
  }

  .link-action-backup {
    margin-right: 5px;
  }
`;
const ProviderButtonsWrapper = styled.div`
  align-items: center;
  display: grid;
  grid-template-columns: auto 1fr;
  grid-gap: 16px 22px;

  .link-action {
    margin-right: 5px;
  }
`;

const createContacts = (contacts, theme) => {
  const styledContacts = contacts.map((contact, index) => {
    let url = null;
    if (contact.link && contact.link.length > 0) {
      url = stringFormat(contact.link, [contact.value]);
    }
    return (
      <ContactWrapper key={index}>
        <IconButton
          className="icon-button"
          color={theme ? theme.profileInfo.iconButtonColor : "#333333"}
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
    this.state = {
      resetAppDialogVisible: false,
      backupCodesDialogVisible: false,
      tfa: null,
    };
  }

  fetchData = async () => {
    const {
      setProviders,
      getTfaType,
      getBackupCodes,
      setBackupCodes,
    } = this.props;

    try {
      await getAuthProviders().then((providers) => {
        setProviders(providers);
      });
    } catch (e) {
      console.error(e);
    }

    const type = await getTfaType();
    this._isMounted && this.setState({ tfa: type });
    if (type && type !== "none") {
      const codes = await getBackupCodes();
      setBackupCodes(codes);
    }
  };

  componentDidMount() {
    const {
      //cultures,
      //getPortalCultures,
      //profile,
      //viewer,
      isSelf,
    } = this.props;
    this._isMounted = true;
    //const isSelf = isMe(viewer, profile.userName);
    //if (isSelf && !cultures.length) {
    //getPortalCultures();
    //}

    if (!isSelf) return;

    this.fetchData();
    window.loginCallback = this.loginCallback;
  }

  componentWillUnmount() {
    window.loginCallback = null;
    this._isMounted = false;
  }

  onEditSubscriptionsClick = () => console.log("Edit subscriptions onClick()");

  onEditProfileClick = () => {
    const {
      isMy,
      avatarMax,
      setAvatarMax,
      history,
      profile,
      setIsEditTargetUser,
    } = this.props;

    avatarMax && setAvatarMax(null);
    setIsEditTargetUser(true);
    const editUrl = isMy
      ? combineUrl(AppServerConfig.proxyURL, `/my?action=edit`)
      : combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/accounts/edit/${profile.userName}`
        );

    history.push(editUrl);
  };

  toggleResetAppDialogVisible = () => {
    this.setState({ resetAppDialogVisible: !this.state.resetAppDialogVisible });
  };

  toggleBackupCodesDialogVisible = () => {
    this.setState({
      backupCodesDialogVisible: !this.state.backupCodesDialogVisible,
    });
  };
  loginCallback = (profile) => {
    const { setProviders, t } = this.props;
    linkOAuth(profile).then((resp) => {
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
    const { t, providers, theme } = this.props;

    const providerButtons =
      providers &&
      providers.map((item) => {
        if (!providersData[item.provider]) return;
        const { icon, label, iconOptions } = providersData[item.provider];

        if (!icon || !label) return <React.Fragment></React.Fragment>;
        return (
          <React.Fragment key={`${item.provider}ProviderItem`}>
            <div>
              {item.provider === "facebook" ? (
                <FacebookButton
                  noHover={true}
                  iconName={icon}
                  label={getProviderTranslation(label, t)}
                  className="socialButton"
                  $iconOptions={iconOptions}
                />
              ) : (
                <SocialButton
                  noHover={true}
                  iconName={icon}
                  label={getProviderTranslation(label, t)}
                  className="socialButton"
                  $iconOptions={iconOptions}
                />
              )}
            </div>
            {item.linked ? (
              <div>
                <Link
                  type="action"
                  color={theme.profileInfo.linkColor}
                  onClick={(e) => this.unlinkAccount(item.provider, e)}
                  isHovered={true}
                >
                  {t("Common:Disconnect")}
                </Link>
              </div>
            ) : (
              <div>
                <Link
                  type="action"
                  color={theme.profileInfo.linkColor}
                  onClick={(e) => this.linkAccount(item.provider, item.url, e)}
                  isHovered={true}
                >
                  {t("Common:Connect")}
                </Link>
              </div>
            )}
          </React.Fragment>
        );
      });

    return providerButtons;
  };

  oauthDataExists = () => {
    const { providers } = this.props;

    let existProviders = 0;
    providers && providers.length > 0;
    providers.map((item) => {
      if (!providersData[item.provider]) return;
      existProviders++;
    });

    return !!existProviders;
  };

  onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;

    this.props.changeEmailSubscription(checked);
  };

  onChangeTheme = async (e) => {
    const { setIsLoading, changeTheme, setTheme } = this.props;

    const value = e.currentTarget.value;

    setIsLoading(true);

    await changeTheme(value);

    setIsLoading(false);
  };

  render() {
    const { resetAppDialogVisible, backupCodesDialogVisible, tfa } = this.state;
    const {
      profile,
      cultures,
      culture,
      isAdmin,
      viewer,
      t,
      isSelf,
      providers,
      backupCodes,
      personal,
      tipsSubscription,
      theme,
      setTheme,
      selectedTheme,
    } = this.props;
    const contacts = profile.contacts && getUserContacts(profile.contacts);
    const role = getUserRole(profile);
    const socialContacts =
      (contacts &&
        contacts.social &&
        contacts.social.length > 0 &&
        createContacts(contacts.social, theme)) ||
      null;
    const infoContacts = contacts && createContacts(contacts.contact, theme);
    //const isSelf = isMe(viewer, profile.userName);

    let backupCodesCount = 0;

    if (backupCodes && backupCodes.length > 0) {
      backupCodes.map((item) => {
        if (!item.isUsed) {
          backupCodesCount++;
        }
      });
    }

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
                size="normal"
                scale={true}
                label={t("EditUser")}
                title={t("EditUser")}
                onClick={this.onEditProfileClick}
                className="edit-profile-button"
              />
            </EditButtonWrapper>
          )}
        </AvatarWrapper>
        <ProfileInfo
          profile={profile}
          isSelf={isSelf}
          isAdmin={isAdmin}
          t={t}
          //cultures={cultures}
          //culture={culture}
        />

        {!personal && isSelf && this.oauthDataExists() && (
          <ToggleWrapper>
            <ToggleContent label={t("LoginSettings")} isOpen={true}>
              <ProviderButtonsWrapper>
                {this.providerButtons()}
              </ProviderButtonsWrapper>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {personal && isSelf && (
          <ToggleWrapper isSelf={true}>
            <ToggleContent
              label={t("Subscriptions")}
              isOpen={true}
              enableToggle={false}
            >
              <ToggleButton
                className="toggle-btn"
                label={t("SubscriptionEmailTipsToggleLbl")}
                onChange={this.onChangeEmailSubscription}
                isChecked={tipsSubscription}
              />
            </ToggleContent>
          </ToggleWrapper>
        )}
        {!personal && isSelf && false && (
          <ToggleWrapper isSelf={true}>
            <ToggleContent label={t("Subscriptions")} isOpen={true}>
              <Text as="span">
                <Button
                  size="normal"
                  label={t("EditSubscriptionsBtn")}
                  primary={true}
                  onClick={this.onEditSubscriptionsClick}
                />
              </Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {isSelf && tfa && tfa !== "none" && (
          <ToggleWrapper>
            <ToggleContent label={t("TfaLoginSettings")} isOpen={true}>
              <Text as="span">{t("TwoFactorDescription")}</Text>
              <LinkActionWrapper>
                <Link
                  type="action"
                  isHovered={true}
                  className="link-action-reset"
                  isBold={true}
                  onClick={this.toggleResetAppDialogVisible}
                >
                  {t("Common:ResetApplication")}
                </Link>
                <Link
                  type="action"
                  isHovered={true}
                  className="link-action-backup"
                  isBold={true}
                  onClick={this.toggleBackupCodesDialogVisible}
                >
                  {t("ShowBackupCodes")}
                </Link>

                <Link color={theme.profileInfo.linkColor} noHover={true}>
                  ({backupCodesCount} {t("CountCodesRemaining")})
                </Link>
              </LinkActionWrapper>
            </ToggleContent>
          </ToggleWrapper>
        )}

        {isSelf && (
          <ToggleWrapper>
            <ToggleContent
              label={t("InterfaceTheme")}
              isOpen={true}
              enableToggle={false}
            >
              <RadioButtonGroup
                orientation={"vertical"}
                name={"interface-theme"}
                options={[
                  { value: ThemeKeys.SystemStr, label: t("SystemTheme") },
                  { value: ThemeKeys.BaseStr, label: t("LightTheme") },
                  { value: ThemeKeys.DarkStr, label: t("DarkTheme") },
                ]}
                onClick={this.onChangeTheme}
                selected={selectedTheme}
                spacing={"10px"}
              />
            </ToggleContent>
          </ToggleWrapper>
        )}

        {!personal && profile.notes && (
          <ToggleWrapper isContacts={true}>
            <ToggleContent label={t("Common:Comments")} isOpen={true}>
              <Text className="profile-comments" as="span">
                {profile.notes}
              </Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {!personal && profile.contacts && (
          <ToggleWrapper isContacts={true}>
            <ToggleContent label={t("ContactInformation")} isOpen={true}>
              <Text as="span">{infoContacts}</Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {!personal && socialContacts && (
          <ToggleWrapper isContacts={true}>
            <ToggleContent
              label={t("PeopleTranslations:SocialProfiles")}
              isOpen={true}
            >
              <Text as="span">{socialContacts}</Text>
            </ToggleContent>
          </ToggleWrapper>
        )}
        {resetAppDialogVisible && (
          <ResetApplicationDialog
            visible={resetAppDialogVisible}
            onClose={this.toggleResetAppDialogVisible}
            resetTfaApp={this.props.resetTfaApp}
            id={profile.id}
          />
        )}
        {backupCodesDialogVisible && (
          <BackupCodesDialog
            visible={backupCodesDialogVisible}
            onClose={this.toggleBackupCodesDialogVisible}
            getNewBackupCodes={this.props.getNewBackupCodes}
            backupCodes={backupCodes}
            backupCodesCount={backupCodesCount}
            setBackupCodes={this.props.setBackupCodes}
          />
        )}
      </ProfileWrapper>
    );
  }
}

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { isAdmin, userStore, settingsStore, tfaStore, logout } = auth;
    const { user: viewer, changeTheme } = userStore;

    const {
      isTabletView,
      getOAuthToken,
      getLoginLink,
      theme,
      setTheme,
    } = settingsStore;
    const {
      targetUserStore,
      avatarEditorStore,
      usersStore,
      loadingStore,
    } = peopleStore;
    const {
      targetUser: profile,
      isMe: isSelf,
      setIsEditTargetUser,
      changeEmailSubscription,
      tipsSubscription,
    } = targetUserStore;
    const { avatarMax, setAvatarMax } = avatarEditorStore;
    const { providers, setProviders } = usersStore;
    const {
      getBackupCodes,
      getNewBackupCodes,
      unlinkApp: resetTfaApp,
      getTfaType,
      backupCodes,
      setBackupCodes,
    } = tfaStore;

    return {
      isAdmin,
      profile,
      viewer,
      isTabletView,
      isSelf,
      avatarMax,
      setAvatarMax,
      providers,
      setProviders,
      getOAuthToken,
      getLoginLink,
      getBackupCodes,
      getNewBackupCodes,
      resetTfaApp,
      getTfaType,
      backupCodes,
      setBackupCodes,
      setIsEditTargetUser,
      personal: auth.settingsStore.personal,
      changeEmailSubscription,
      tipsSubscription,
      theme,
      setTheme,
      changeTheme,
      selectedTheme: viewer.theme,
      setIsLoading: loadingStore.setIsLoading,
      logout,
    };
  })(
    observer(
      withTranslation(["Profile", "Common", "PeopleTranslations"])(
        withPeopleLoader(SectionBodyContent)(<Loaders.ProfileView />)
      )
    )
  )
);
