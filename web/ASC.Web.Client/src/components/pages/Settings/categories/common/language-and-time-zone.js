import React from "react";
import { connect } from "react-redux";
import { withTranslation } from 'react-i18next';
import { FieldContainer, Text, ComboBox, Loader, Button, toastr, Link, TextInput } from "asc-web-components";
import styled from 'styled-components';
import { Trans } from 'react-i18next';
import { store, utils } from 'asc-web-common';
import { setLanguageAndTime, getPortalTimezones, setGreetingTitle, restoreGreetingTitle } from '../../../../../store/settings/actions';
import { default as clientStore } from '../../../../../store/store';

const { changeLanguage } = utils;
const { getPortalCultures, getModules, getCurrentCustomSchema } = store.auth.actions;

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
   return items.find(item => item.key === selectedItemKey);
}

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
`;
class LanguageAndTimeZone extends React.Component {

   constructor(props) {
      super(props);

      const { portalLanguage, portalTimeZoneId, rawCultures, rawTimezones, t } = props;
      const languages = mapCulturesToArray(rawCultures, t);
      const timezones = mapTimezonesToArray(rawTimezones);

      document.title = `${t("Customization")} – ${t("OrganizationName")}`;

      this.state = {
         isLoadedData: false,
         isLoading: false,
         timezones,
         timezone: findSelectedItemByKey(timezones, portalTimeZoneId || timezones[0]),
         languages,
         language: findSelectedItemByKey(languages, portalLanguage || languages[0]),
         isLoadingGreetingSave: false,
         isLoadingGreetingRestore: false,
      }
   }


   componentDidMount() {
      const { getPortalCultures, portalLanguage, portalTimeZoneId, t, getPortalTimezones } = this.props;
      const { timezones, languages } = this.state;

      if (!timezones.length && !languages.length) {
         let languages;
         getPortalCultures()
            .then(() => {
               languages = mapCulturesToArray(this.props.rawCultures, t);
            })
            .then(() => getPortalTimezones())
            .then(() => {
               const timezones = mapTimezonesToArray(this.props.rawTimezones);
               const timezone = findSelectedItemByKey(timezones, portalTimeZoneId) || timezones[0];
               const language = findSelectedItemByKey(languages, portalLanguage) || languages[0];

               this.setState({ languages, language, timezones, timezone });
            });
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
               const newLocaleLanguages = mapCulturesToArray(this.props.rawCultures, t);
               const newLocaleSelectedLanguage = findSelectedItemByKey(newLocaleLanguages, this.state.language.key) || newLocaleLanguages[0];

               this.setState({
                  languages: newLocaleLanguages,
                  language: newLocaleSelectedLanguage
               });
            })
            .then(() => getModules(clientStore.dispatch))
            .then(() => getCurrentCustomSchema(clientStore.dispatch, nameSchemaId));
      }
   }

   onLanguageSelect = (language) => {
      this.setState({ language })
   };

   onTimezoneSelect = (timezone) => {
      this.setState({ timezone })
   };

   onSaveLngTZSettings = () => {
      const { setLanguageAndTime, i18n } = this.props;
      this.setState({ isLoading: true }, function () {
         setLanguageAndTime(this.state.language.key, this.state.timezone.key)
            .then(() => changeLanguage(i18n))
            .then((t) => toastr.success(t("SuccessfullySaveSettingsMessage")))
            .catch((error) => toastr.error(error))
            .finally(() => this.setState({ isLoading: false }));
      })
   }

   render() {
      const { t, i18n } = this.props;
      const { isLoadedData, languages, language, isLoading, timezones, timezone } = this.state;
      const supportEmail = "documentation@onlyoffice.com";
      const tooltipLanguage =
         <Text fontSize='13px'>
            <Trans i18nKey="NotFoundLanguage" i18n={i18n}>
               "In case you cannot find your language in the list of the
               available ones, feel free to write to us at
               <Link href={`mailto:${supportEmail}`} isHovered={true}>
                  {{ supportEmail }}
               </Link> to take part in the translation and get up to 1 year free of
               charge."
            </Trans>
            {" "}
            <Link isHovered={true} href="https://helpcenter.onlyoffice.com/ru/guides/become-translator.aspx">{t("LearnMore")}</Link>
         </Text>

      return (
         !isLoadedData ?
            <Loader className="pageLoader" type="rombs" size='40px' />
            : <>
               <StyledComponent>
                  <div className='settings-block'>
                     <FieldContainer
                        id='fieldContainerLanguage'
                        className='margin-top field-container-width'
                        labelText={`${t("Language")}:`}
                        tooltipContent={tooltipLanguage}
                        helpButtonHeaderContent={t("Language")}
                        isVertical={true}>
                        <ComboBox
                           id='comboBoxLanguage'
                           options={languages}
                           selectedOption={language}
                           onSelect={this.onLanguageSelect}
                           isDisabled={isLoading}
                           noBorder={false}
                           scaled={true}
                           scaledOptions={true}
                           dropDownMaxHeight={300}
                           className='dropdown-item-width'
                        />
                     </FieldContainer>

                     <FieldContainer
                        id='fieldContainerTimezone'
                        className='field-container-width'
                        labelText={`${t("TimeZone")}:`}
                        isVertical={true}>
                        <ComboBox
                           id='comboBoxTimezone'
                           options={timezones}
                           selectedOption={timezone}
                           onSelect={this.onTimezoneSelect}
                           isDisabled={isLoading}
                           noBorder={false}
                           scaled={true}
                           scaledOptions={true}
                           dropDownMaxHeight={300}
                           className='dropdown-item-width'
                        />
                     </FieldContainer>
                     <Button
                        id='btnSaveLngTZ'
                        className='margin-top'
                        primary={true}
                        size='medium'
                        label={t('SaveButton')}
                        isLoading={isLoading}
                        onClick={this.onSaveLngTZSettings}
                     />
                  </div>

               </StyledComponent>

            </>
      );
   }
};

function mapStateToProps(state) {
   return {
      portalLanguage: state.auth.settings.culture,
      portalTimeZoneId: state.auth.settings.timezone,
      language: state.auth.user.cultureName || state.auth.settings.culture,
      rawTimezones: state.auth.settings.timezones,
      rawCultures: state.auth.settings.cultures,
      greetingSettings: state.auth.settings.greetingSettings,
      nameSchemaId: state.auth.settings.nameSchemaId
   };
}

export default connect(mapStateToProps, {
   getPortalCultures, setLanguageAndTime, getPortalTimezones,
   setGreetingTitle, restoreGreetingTitle
})(withTranslation()(LanguageAndTimeZone));
