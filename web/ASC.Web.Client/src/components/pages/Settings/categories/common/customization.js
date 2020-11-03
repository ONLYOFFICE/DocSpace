import React from "react";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import { Text, Loader, toastr, Link, Icons } from "asc-web-components";
import styled from "styled-components";
import { store, utils } from "asc-web-common";
import {
  setLanguageAndTime,
  getPortalTimezones,
} from "../../../../../store/settings/actions";
import { default as clientStore } from "../../../../../store/store";
import { setDocumentTitle } from "../../../../../helpers/utils";
const { getLanguage } = store.auth.selectors;
const { changeLanguage } = utils;
const {
  getPortalCultures,
  getModules,
  getCurrentCustomSchema,
} = store.auth.actions;

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
      organizationName,
      t,
    } = props;
    const languages = mapCulturesToArray(rawCultures, t);
    const timezones = mapTimezonesToArray(rawTimezones);

    setDocumentTitle(t("Customization"));

    this.state = {
      isLoadedData: false,
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
    const { timezones, languages, isLoadedData } = this.state;

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

    if (timezones.length && languages.length && !isLoadedData) {
      this.setState({ isLoadedData: true });
    }
  }

  componentDidUpdate(prevProps, prevState) {
    const { timezones, languages } = this.state;
    const { i18n, language, nameSchemaId } = this.props;

    if (timezones.length && languages.length && !prevState.isLoadedData) {
      this.setState({ isLoadedData: true });
    }
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
        .then(() => getModules(clientStore.dispatch))
        .then(() => getCurrentCustomSchema(clientStore.dispatch, nameSchemaId));
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
    const { t } = this.props;
    const { isLoadedData, language, timezone } = this.state;
    return !isLoadedData ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <>
        <StyledComponent>
          <div className="category-item-wrapper">
            <div className="category-item-heading">
              <Link
                className="inherit-title-link header"
                onClick={this.onClickLink}
                truncate={true}
                href="/settings/common/customization/language-and-time-zone"
              >
                {t("StudioTimeLanguageSettings")}
              </Link>
              <Icons.ArrowRightIcon
                size="small"
                isfill={true}
                color="#333333"
              />
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
                href="/settings/common/customization/custom-titles"
              >
                {t("CustomTitles")}
              </Link>
              <Icons.ArrowRightIcon
                size="small"
                isfill={true}
                color="#333333"
              />
            </div>
            <Text className="category-item-description">
              {t("CustomTitlesSettingsDescription")}
            </Text>
          </div>
        </StyledComponent>
      </>
    );
  }
}

function mapStateToProps(state) {
  const {
    culture,
    timezone,
    timezones,
    cultures,
    nameSchemaId,
    organizationName,
  } = state.auth.settings;
  return {
    portalLanguage: culture,
    portalTimeZoneId: timezone,
    language: getLanguage(state),
    rawTimezones: timezones,
    rawCultures: cultures,
    nameSchemaId: nameSchemaId,
    organizationName,
  };
}

export default connect(mapStateToProps, {
  getPortalCultures,
  setLanguageAndTime,
  getPortalTimezones,
})(withTranslation()(Customization));
