import React from "react";
import { Trans, withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import Link from "@docspace/components/link";
import ComboBox from "@docspace/components/combobox";
import HelpButton from "@docspace/components/help-button";
import styled from "styled-components";
import { resendUserInvites } from "@docspace/common/api/people";
import toastr from "client/toastr";
import Loaders from "@docspace/common/components/Loaders";
import { inject, observer } from "mobx-react";
import { showLoader, hideLoader } from "@docspace/common/utils";
import { withRouter } from "react-router";
import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl, convertLanguage } from "@docspace/common/utils";
import withCultureNames from "@docspace/common/hoc/withCultureNames";
import config from "PACKAGE_FILE";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import { Base } from "@docspace/components/themes";
import { isMobileOnly } from "react-device-detect";

const InfoContainer = styled.div`
  margin-bottom: 24px;
  ${NoUserSelect}
`;

const InfoItem = styled.div`
  font-family: Open Sans;
  font-style: normal;
  font-weight: normal;
  font-size: 13px;
  line-height: 24px;
  display: flex;
  width: 344px;
`;

const InfoItemLabel = styled.div`
  width: 200px;
  min-width: 150px;

  @media (max-width: 620px) {
    width: 130px;
    min-width: 120px;
  }

  white-space: nowrap;
  color: ${(props) => props.theme.profileInfo.color};
`;

InfoItemLabel.defaultProps = { theme: Base };

const InfoItemValue = styled.div`
  width: 260px;
  word-break: break-word;

  .language-combo {
    padding-top: 4px;
    float: left;
    margin-right: 8px;

    & > div:first-child > div:last-child {
      margin-right: 0;
    }

    & > div {
      padding-left: 0px;
    }
  }
  .help-icon {
    display: inline-flex;

    @media (min-width: 1025px) {
      margin-top: 6px;
    }
    @media (max-width: 1024px) {
      padding: 6px 8px 8px 8px;
      margin-left: -8px;
    }
  }

  .email-link {
    vertical-align: 4px;
  }
`;

const IconButtonWrapper = styled.div`
  ${(props) => (props.isBefore ? `margin-right: 8px;` : `margin-left: 8px;`)}

  display: inline-flex;

  :hover {
    & > div > svg > path {
      fill: #3b72a7;
    }
  }
`;

const LangSelectorContainer = styled.div`
  display: flex;
`;

class ProfileInfo extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      profile: props.profile,
    };
  }

  componentDidUpdate() {
    const { isLoading } = this.props;

    isLoading ? showLoader() : hideLoader();
  }

  onGroupClick = (e) => {
    const group = e.currentTarget.dataset.id;
    const { filter, setIsLoading, fetchPeople, history } = this.props;

    const newFilter = filter.clone();
    newFilter.group = group;

    setIsLoading(true);

    const urlFilter = newFilter.toUrlParams();

    const url = combineUrl(
      AppServerConfig.proxyURL,
      config.homepage,
      `/accounts/filter?${urlFilter}`
    );
    history.push(url);

    fetchPeople(newFilter).finally(() => setIsLoading(false));
  };

  getFormattedDepartments = (departments) => {
    const formattedDepartments = departments.map((department, index) => {
      return (
        <span key={index}>
          <Link
            type="page"
            fontSize="13px"
            isHovered={true}
            data-id={department.id}
            onClick={this.onGroupClick}
          >
            {department.name}
          </Link>
          {departments.length - 1 !== index ? ", " : ""}
        </span>
      );
    });

    return formattedDepartments;
  };

  onSentInviteAgain = (id) => {
    const { t } = this.props;
    resendUserInvites(new Array(id))
      .then(() => toastr.success(t("PeopleTranslations:SuccessSentInvitation")))
      .catch((error) =>
        toastr.error(error && error.message ? error.message : error)
      );
  };

  onEmailClick = (e) => {
    const email = e.currentTarget.dataset.email;
    if (e.target.title) window.open("mailto:" + email);
  };

  onLanguageSelect = (language) => {
    console.log("onLanguageSelect", language);
    const {
      profile,
      updateProfileCulture,
      //i18n,
      setIsLoading,
    } = this.props;

    if (profile.cultureName === language.key) return;

    setIsLoading(true);
    updateProfileCulture(profile.id, language.key)
      // .then(() => {
      //   console.log("changeLanguage to", language.key);
      //   i18n && i18n.changeLanguage(language.key);
      // })
      .then(() => setIsLoading(false))
      .then(() => location.reload())
      .catch((error) => {
        toastr.error(error && error.message ? error.message : error);
        setIsLoading(false);
      });
  };

  getLanguages = () => {
    const { cultureNames } = this.props;

    return cultureNames;
  };

  render() {
    const {
      t,
      userPostCaption,
      regDateCaption,
      groupCaption,
      userCaption,
      guestCaption,
      cultureNames,
      profile,
      isAdmin,
      isSelf,
      culture,
      personal,
      theme,
      helpLink,
    } = this.props;

    const {
      isVisitor,
      email,
      activationStatus,
      department,
      groups,
      title,
      mobilePhone,
      sex,
      workFrom,
      birthday,
      location,
      cultureName,
      currentCulture,
    } = profile;

    const type = isVisitor ? guestCaption : userCaption;
    const language = convertLanguage(cultureName || currentCulture || culture);

    //const languages = this.getLanguages();
    const selectedLanguage = cultureNames.find(
      (item) => item.key === language
    ) ||
      cultureNames.find((item) => item.key === culture) || {
        key: language,
        label: "",
      };

    const workFromDate = new Date(workFrom).toLocaleDateString(language);
    const birthDayDate = new Date(birthday).toLocaleDateString(language);

    const formatedSex =
      (sex === "male" && t("PeopleTranslations:MaleSexStatus")) ||
      t("PeopleTranslations:FemaleSexStatus");

    const formatedDepartments =
      department && this.getFormattedDepartments(groups);

    const supportEmail = "documentation@onlyoffice.com";

    const tooltipLanguage = (
      <Text fontSize="13px">
        <Trans t={t} i18nKey="NotFoundLanguage" ns="Common">
          "In case you cannot find your language in the list of the available
          ones, feel free to write to us at
          <Link
            href={`mailto:${supportEmail}`}
            isHovered={true}
            color={theme.profileInfo.tooltipLinkColor}
          >
            {{ supportEmail }}
          </Link>
          to take part in the translation and get up to 1 year free of charge."
        </Trans>{" "}
        <Link
          color={theme.profileInfo.tooltipLinkColor}
          isHovered={true}
          href={`${helpLink}/guides/become-translator.aspx`}
          target="_blank"
        >
          {t("Common:LearnMore")}
        </Link>
      </Text>
    );

    return (
      <InfoContainer>
        {!personal && (
          <InfoItem>
            <InfoItemLabel>{t("Common:Type")}:</InfoItemLabel>
            <InfoItemValue className="profile-info_type">{type}</InfoItemValue>
          </InfoItem>
        )}
        {email && (
          <InfoItem>
            <InfoItemLabel>{t("Common:Email")}:</InfoItemLabel>
            <InfoItemValue>
              <>
                {activationStatus === 2 && (isAdmin || isSelf) && (
                  <IconButtonWrapper
                    isBefore={true}
                    title={t("PeopleTranslations:PendingTitle")}
                  >
                    <IconButton
                      color={theme.profileInfo.iconColor}
                      size={16}
                      iconName="images/danger.react.svg"
                      isFill={true}
                    />
                  </IconButtonWrapper>
                )}
                {isSelf ? (
                  <Text>{email}</Text>
                ) : (
                  <Link
                    className="email-link"
                    type="page"
                    fontSize="13px"
                    isHovered={true}
                    title={email}
                    data-email={email}
                    onClick={this.onEmailClick}
                  >
                    {email}
                  </Link>
                )}
              </>
            </InfoItemValue>
          </InfoItem>
        )}
        {mobilePhone && (
          <InfoItem>
            <InfoItemLabel>{t("Profile:PhoneLbl")}:</InfoItemLabel>
            <InfoItemValue>{mobilePhone}</InfoItemValue>
          </InfoItem>
        )}
        {!personal && sex && (
          <InfoItem>
            <InfoItemLabel>{t("PeopleTranslations:Sex")}:</InfoItemLabel>
            <InfoItemValue className="profile-info_sex">
              {formatedSex}
            </InfoItemValue>
          </InfoItem>
        )}
        {!personal && birthday && (
          <InfoItem>
            <InfoItemLabel>{t("PeopleTranslations:Birthdate")}:</InfoItemLabel>
            <InfoItemValue className="profile-info_birthdate">
              {birthDayDate}
            </InfoItemValue>
          </InfoItem>
        )}
        {!personal && title && (
          <InfoItem>
            <InfoItemLabel>{userPostCaption}:</InfoItemLabel>
            <InfoItemValue className="profile-info_title">
              {title}
            </InfoItemValue>
          </InfoItem>
        )}
        {!personal && department && (
          <InfoItem>
            <InfoItemLabel>{groupCaption}:</InfoItemLabel>
            <InfoItemValue className="profile-info_group">
              {formatedDepartments}
            </InfoItemValue>
          </InfoItem>
        )}
        {!personal && location && (
          <InfoItem>
            <InfoItemLabel>{t("Common:Location")}:</InfoItemLabel>
            <InfoItemValue className="profile-info_location">
              {location}
            </InfoItemValue>
          </InfoItem>
        )}
        {!personal && workFrom && (
          <InfoItem>
            <InfoItemLabel>{regDateCaption}:</InfoItemLabel>
            <InfoItemValue className="profile-info_reg-date">
              {workFromDate}
            </InfoItemValue>
          </InfoItem>
        )}
        {isSelf && (
          <InfoItem>
            <InfoItemLabel>{t("Common:Language")}:</InfoItemLabel>
            <InfoItemValue>
              {cultureNames ? (
                <LangSelectorContainer>
                  <ComboBox
                    directionY="both"
                    options={cultureNames}
                    selectedOption={selectedLanguage}
                    onSelect={this.onLanguageSelect}
                    isDisabled={false}
                    noBorder={true}
                    scaled={false}
                    scaledOptions={false}
                    size="content"
                    className="language-combo"
                    showDisabledItems={true}
                    dropDownMaxHeight={364}
                    manualWidth="320px"
                    isDefaultMode={!isMobileOnly}
                    withBlur={isMobileOnly}
                    fillIcon={false}
                    offsetLeft={-16}
                  />
                  <HelpButton
                    place="bottom"
                    offsetLeft={50}
                    offsetRight={0}
                    tooltipContent={tooltipLanguage}
                    helpButtonHeaderContent={t("Common:Language")}
                    className="help-icon"
                  />
                </LangSelectorContainer>
              ) : (
                <Loaders.Text />
              )}
            </InfoItemValue>
          </InfoItem>
        )}
      </InfoContainer>
    );
  }
}

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { settingsStore } = auth;
    const { culture, customNames, theme, helpLink } = settingsStore;
    const {
      groupCaption,
      regDateCaption,
      userPostCaption,
      userCaption,
      guestCaption,
    } = customNames;
    const {
      usersStore,
      filterStore,
      loadingStore,
      targetUserStore,
    } = peopleStore;
    const { getUsersList: fetchPeople } = usersStore;
    const { filter } = filterStore;
    const { setIsLoading, isLoading } = loadingStore;
    const { updateProfileCulture } = targetUserStore;
    return {
      theme,
      culture,
      groupCaption,
      regDateCaption,
      userPostCaption,
      userCaption,
      guestCaption,
      fetchPeople,
      filter,
      setIsLoading,
      isLoading,
      updateProfileCulture,
      personal: auth.settingsStore.personal,
      helpLink,
    };
  })(
    observer(
      withTranslation(["Profile", "Common", "PeopleTranslations"])(
        withCultureNames(ProfileInfo)
      )
    )
  )
);
