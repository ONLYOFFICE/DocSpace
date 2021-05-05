import React from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Text from "@appserver/components/text";
import Loader from "@appserver/components/loader";
import Box from "@appserver/components/box";
import toastr from "@appserver/components/toast/toastr";
import Link from "@appserver/components/link";
import ArrowRightIcon from "../../../../../../public/images/arrow.right.react.svg";
import { setDocumentTitle } from "../../../../../helpers/utils";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { showLoader, hideLoader, combineUrl } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@appserver/common/constants";

const mapCulturesToArray = (cultures, t) => {
  return cultures.map((culture) => {
    return { key: culture, label: t(`Culture_${culture}`) };
  });
};

const mapTimezonesToArray = (timezones) => {
  return timezones.map((timezone) => {
    return { key: timezone.id, label: timezone.displayName };
  });
};

const findSelectedItemByKey = (items, selectedItemKey) => {
  return items.find((item) => item.key === selectedItemKey);
};

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

const StyledComponent = styled.div`
  .margin-top {
    margin-top: 20px;
  }

  .margin-left {
    margin-left: 20px;
  }

  .settings-block {
    margin-bottom: 70px;
  }

  .field-container-width {
    max-width: 500px;
  }

  .combo-button-label {
    max-width: 100%;
  }

  .category-item-wrapper {
    margin-bottom: 40px;

    .category-item-heading {
      display: flex;
      align-items: center;
      margin-bottom: 5px;
    }

    .category-item-subheader {
      font-size: 13px;
      font-weight: 600;
      margin-bottom: 5px;
    }

    .category-item-description {
      color: #555f65;
      font-size: 12px;
      max-width: 1024px;
    }

    .inherit-title-link {
      margin-right: 7px;
      font-size: 19px;
      font-weight: 600;
    }

    .link-text {
      margin: 0;
    }
  }
`;
class Customization extends React.Component {
  constructor(props) {
    super(props);

    const {
      portalLanguage,
      portalTimeZoneId,
      rawCultures,
      rawTimezones,
      /*organizationName,*/
      t,
    } = props;
    const languages = mapCulturesToArray(rawCultures, t);
    const timezones = mapTimezonesToArray(rawTimezones);

    setDocumentTitle(t("Customization"));

    this.state = {
      isLoading: false,
      timezones,
      timezone: findSelectedItemByKey(
        timezones,
        portalTimeZoneId || timezones[0]
      ),
      languages,
      language: findSelectedItemByKey(
        languages,
        portalLanguage || languages[0]
      ),
      isLoadingGreetingSave: false,
      isLoadingGreetingRestore: false,
    };
  }

  componentDidMount() {
    const {
      getPortalCultures,
      portalLanguage,
      portalTimeZoneId,
      t,
      getPortalTimezones,
    } = this.props;
    const { timezones, languages } = this.state;
    showLoader();
    if (!timezones.length && !languages.length) {
      let languages;
      getPortalCultures()
        .then(() => {
          languages = mapCulturesToArray(this.props.rawCultures, t);
        })
        .then(() => getPortalTimezones())
        .then(() => {
          const timezones = mapTimezonesToArray(this.props.rawTimezones);
          const timezone =
            findSelectedItemByKey(timezones, portalTimeZoneId) || timezones[0];
          const language =
            findSelectedItemByKey(languages, portalLanguage) || languages[0];

          this.setState({ languages, language, timezones, timezone });
        });
    }

    hideLoader();
  }

  componentDidUpdate(prevProps) {
    const { i18n, language, nameSchemaId, getCurrentCustomSchema } = this.props;

    if (language !== prevProps.language) {
      changeLanguage(i18n)
        .then((t) => {
          const newLocaleLanguages = mapCulturesToArray(
            this.props.rawCultures,
            t
          );
          const newLocaleSelectedLanguage =
            findSelectedItemByKey(
              newLocaleLanguages,
              this.state.language.key
            ) || newLocaleLanguages[0];

          this.setState({
            languages: newLocaleLanguages,
            language: newLocaleSelectedLanguage,
          });
        })
        //.then(() => getModules(clientStore.dispatch))
        .then(() => getCurrentCustomSchema(nameSchemaId));
    }
  }

  onLanguageSelect = (language) => {
    this.setState({ language });
  };

  onTimezoneSelect = (timezone) => {
    this.setState({ timezone });
  };

  onSaveLngTZSettings = () => {
    const { setLanguageAndTime, i18n } = this.props;
    this.setState({ isLoading: true }, function () {
      setLanguageAndTime(this.state.language.key, this.state.timezone.key)
        .then(() => changeLanguage(i18n))
        .then((t) => toastr.success(t("SuccessfullySaveSettingsMessage")))
        .catch((error) => toastr.error(error))
        .finally(() => this.setState({ isLoading: false }));
    });
  };

  onClickLink = (e) => {
    e.preventDefault();
    const { history } = this.props;
    history.push(e.target.pathname);
  };

  render() {
    const { t, helpUrlCommonSettings, customNames } = this.props;
    const { language, timezone } = this.state;

    return (
      <StyledComponent>
        <div className="category-item-wrapper">
          <div className="category-item-heading">
            <Link
              className="inherit-title-link header"
              onClick={this.onClickLink}
              truncate={true}
              href={combineUrl(
                AppServerConfig.proxyURL,
                "/settings/common/customization/language-and-time-zone"
              )}
            >
              {t("StudioTimeLanguageSettings")}
            </Link>
            <StyledArrowRightIcon size="small" color="#333333" />
          </div>
          {language && language.label && timezone && timezone.label && (
            <Text className="category-item-subheader" truncate={true}>
              {`${language.label} / ${timezone.label}`}
            </Text>
          )}
          <Text className="category-item-description">
            {t("LanguageAndTimeZoneSettingsDescription")}
          </Text>
        </div>
        <div className="category-item-wrapper">
          <div className="category-item-heading">
            <Link
              truncate={true}
              className="inherit-title-link header"
              onClick={this.onClickLink}
              href={combineUrl(
                AppServerConfig.proxyURL,
                "/settings/common/customization/custom-titles"
              )}
            >
              {t("CustomTitles")}
            </Link>
            <StyledArrowRightIcon size="small" color="#333333" />
          </div>
          <Text className="category-item-description">
            {t("CustomTitlesSettingsDescription")}
          </Text>
        </div>
        <div className="category-item-wrapper">
          <div className="category-item-heading">
            <Link
              truncate={true}
              className="inherit-title-link header"
              onClick={this.onClickLink}
              href={combineUrl(
                AppServerConfig.proxyURL,
                "/settings/common/customization/team-template"
              )}
            >
              {t("TeamTemplate")}
            </Link>
            <StyledArrowRightIcon size="small" color="#333333" />
          </div>
          <Box marginProp="4px 0 6px 0">
            <Text fontWeight="600">{`${customNames.name}`}</Text>
          </Box>
          <Text className="category-item-description">
            {t("TeamTemplateSettingsDescription")}
          </Text>
          <Box marginProp="16px 0 0 0">
            <Link
              color="#316DAA"
              target="_blank"
              isHovered={true}
              href={helpUrlCommonSettings}
            >
              {t("LearnMore")}
            </Link>
          </Box>
        </div>
      </StyledComponent>
    );
  }
}

export default inject(({ auth, setup }) => {
  const {
    culture,
    timezone,
    timezones,
    cultures,
    nameSchemaId,
    organizationName,
    getPortalCultures,
    getCurrentCustomSchema,
    getPortalTimezones,
    helpUrlCommonSettings,
    customNames,
  } = auth.settingsStore;

  const { setLanguageAndTime } = setup;

  return {
    portalLanguage: culture,
    language: culture,
    portalTimeZoneId: timezone,
    rawTimezones: timezones,
    rawCultures: cultures,
    nameSchemaId,
    organizationName,
    getPortalCultures,
    setLanguageAndTime,
    getPortalTimezones,
    getCurrentCustomSchema,
    helpUrlCommonSettings,
    customNames,
  };
})(withTranslation("Settings")(observer(Customization)));
