import React from "react";
import { Trans } from "react-i18next";
import axios from "axios";
import {
  Text,
  IconButton,
  Link,
  ComboBox,
  HelpButton,
} from "asc-web-components";
import styled from "styled-components";
import { history, api, store, toastr, Loaders } from "asc-web-common";
import { connect } from "react-redux";
import { updateProfileCulture } from "../../../../../../store/profile/actions";

const { resendUserInvites } = api.people;

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
  width: 360px;
`;

const InfoItemLabel = styled.div`
  width: 200px;

  @media (max-width: 620px) {
    width: 130px;
  }

  white-space: nowrap;
  color: #83888d;
`;

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
      padding: 6px 24px 8px 8px;
      margin-left: -8px;
    }
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

const onGroupClick = (e) => {
  const id = e.currentTarget.dataset.id;
  history.push(`/products/people/filter?group=${id}`);
};

const getFormattedDepartments = (departments) => {
  const formattedDepartments = departments.map((department, index) => {
    return (
      <span key={index}>
        <Link
          type="page"
          fontSize="13px"
          isHovered={true}
          data-id={department.id}
          onClick={onGroupClick}
        >
          {department.name}
        </Link>
        {departments.length - 1 !== index ? ", " : ""}
      </span>
    );
  });

  return formattedDepartments;
};

const capitalizeFirstLetter = (string) => {
  return string && string.charAt(0).toUpperCase() + string.slice(1);
};

class ProfileInfo extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = this.mapPropsToState(props);
  }

  mapPropsToState = (props) => {
    const newState = {
      profile: props.profile,
    };

    return newState;
  };

  onSentInviteAgain = (id) => {
    resendUserInvites(new Array(id))
      .then(() => toastr.success("The invitation was successfully sent"))
      .catch((error) => toastr.error(error));
  };

  onEmailClick = (e) => {
    const email = e.currentTarget.dataset.email;
    if (e.target.title) window.open("mailto:" + email);
  };

  onLanguageSelect = (language) => {
    console.log("onLanguageSelect", language);
    const { profile, updateProfileCulture } = this.props;

    if (profile.cultureName === language.key) return;

    updateProfileCulture(profile.id, language.key).catch((err) =>
      console.log(err)
    );
  };

  getLanguages = () => {
    const { cultures, t } = this.props;

    return cultures.map((culture) => {
      return { key: culture, label: t(`Culture_${culture}`) };
    });
  };

  render() {
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
    } = this.props.profile;
    const isAdmin = this.props.isAdmin;
    const isSelf = this.props.isSelf;
    const {
      t,
      i18n,
      userPostCaption,
      regDateCaption,
      groupCaption,
      userCaption,
      guestCaption,
    } = this.props;
    const type = isVisitor ? guestCaption : userCaption;
    const language = cultureName || currentCulture || this.props.culture;
    const languages = this.getLanguages();
    const selectedLanguage = languages.find((item) => item.key === language);
    const workFromDate = new Date(workFrom).toLocaleDateString(language);
    const birthDayDate = new Date(birthday).toLocaleDateString(language);
    const formatedSex =
      (sex === "male" && t("MaleSexStatus")) || t("FemaleSexStatus");
    const formatedDepartments = department && getFormattedDepartments(groups);
    const supportEmail = "documentation@onlyoffice.com";
    const tooltipLanguage = (
      <Text fontSize="13px">
        <Trans i18nKey="NotFoundLanguage" i18n={i18n}>
          "In case you cannot find your language in the list of the available
          ones, feel free to write to us at
          <Link href={`mailto:${supportEmail}`} isHovered={true}>
            {{ supportEmail }}
          </Link>{" "}
          to take part in the translation and get up to 1 year free of charge."
        </Trans>{" "}
        <Link
          isHovered={true}
          href="https://helpcenter.onlyoffice.com/ru/guides/become-translator.aspx"
        >
          {t("LearnMore")}
        </Link>
      </Text>
    );
    return (
      <InfoContainer>
        <InfoItem>
          <InfoItemLabel>{t("UserType")}:</InfoItemLabel>
          <InfoItemValue>{type}</InfoItemValue>
        </InfoItem>
        {email && (
          <InfoItem>
            <InfoItemLabel>{t("Email")}:</InfoItemLabel>
            <InfoItemValue>
              <>
                {activationStatus === 2 && (isAdmin || isSelf) && (
                  <IconButtonWrapper isBefore={true} title={t("PendingTitle")}>
                    <IconButton
                      color="#C96C27"
                      size={16}
                      iconName="DangerIcon"
                      isFill={true}
                    />
                  </IconButtonWrapper>
                )}
                <Link
                  type="page"
                  fontSize="13px"
                  isHovered={true}
                  title={email}
                  data-email={email}
                  onClick={this.onEmailClick}
                >
                  {email}
                </Link>
              </>
            </InfoItemValue>
          </InfoItem>
        )}
        {mobilePhone && (
          <InfoItem>
            <InfoItemLabel>{t("PhoneLbl")}:</InfoItemLabel>
            <InfoItemValue>{mobilePhone}</InfoItemValue>
          </InfoItem>
        )}
        {sex && (
          <InfoItem>
            <InfoItemLabel>{t("Sex")}:</InfoItemLabel>
            <InfoItemValue>{formatedSex}</InfoItemValue>
          </InfoItem>
        )}
        {birthday && (
          <InfoItem>
            <InfoItemLabel>{t("Birthdate")}:</InfoItemLabel>
            <InfoItemValue>{birthDayDate}</InfoItemValue>
          </InfoItem>
        )}
        {title && (
          <InfoItem>
            <InfoItemLabel>{userPostCaption}:</InfoItemLabel>
            <InfoItemValue>{title}</InfoItemValue>
          </InfoItem>
        )}
        {department && (
          <InfoItem>
            <InfoItemLabel>{groupCaption}:</InfoItemLabel>
            <InfoItemValue>{formatedDepartments}</InfoItemValue>
          </InfoItem>
        )}
        {location && (
          <InfoItem>
            <InfoItemLabel>{t("Location")}:</InfoItemLabel>
            <InfoItemValue>{location}</InfoItemValue>
          </InfoItem>
        )}
        {workFrom && (
          <InfoItem>
            <InfoItemLabel>{regDateCaption}:</InfoItemLabel>
            <InfoItemValue>{workFromDate}</InfoItemValue>
          </InfoItem>
        )}
        {isSelf && (
          <InfoItem>
            <InfoItemLabel>{t("Language")}:</InfoItemLabel>
            <InfoItemValue>
              {languages && selectedLanguage ? (
                <>
                  <ComboBox
                    options={languages}
                    selectedOption={selectedLanguage}
                    onSelect={this.onLanguageSelect}
                    isDisabled={false}
                    noBorder={true}
                    scaled={false}
                    scaledOptions={false}
                    size="content"
                    className="language-combo"
                  />
                  <HelpButton
                    place="bottom"
                    offsetLeft={50}
                    offsetRight={0}
                    tooltipContent={tooltipLanguage}
                    helpButtonHeaderContent={t("Language")}
                    className="help-icon"
                  />
                </>
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

function mapStateToProps(state) {
  const { customNames } = state.auth.settings;
  const {
    groupCaption,
    regDateCaption,
    userPostCaption,
    userCaption,
    guestCaption,
  } = customNames;

  return {
    groupCaption,
    regDateCaption,
    userPostCaption,
    userCaption,
    guestCaption,
  };
}
const mapDispatchToProps = (dispatch) => {
  return {
    updateProfileCulture: (id, culture) =>
      dispatch(updateProfileCulture(id, culture)),
  };
};
export default connect(mapStateToProps, mapDispatchToProps)(ProfileInfo);
