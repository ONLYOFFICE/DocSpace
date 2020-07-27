import React from "react";
import { connect } from "react-redux";
import { withTranslation } from 'react-i18next';
import { Text, Loader, toastr, Link, Icons } from "asc-web-components";
import styled from 'styled-components';
import { Trans } from 'react-i18next';
import { store, utils, api } from 'asc-web-common';
import { setLanguageAndTime, getPortalTimezones, setGreetingTitle, restoreGreetingTitle, getDefaultGreetingTitle } from '../../../../../store/settings/actions';
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

const GreetingTitleIsDefault = () => {
   api.settings.getSettings().then((settings) => {
      return true  // TODO Add a check for the standard title
    /*  if(settings.defaultGreetingTitle){
         return DefaultGreetingTitle === greetingSettings
      }*/
   })
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

   .category-item-wrapper{
      margin-bottom:40px;

      .category-item-heading{
         display:flex;
         align-items: center;
         margin-bottom: 5px;
      }

      .category-item-subheader{
         font-size: 13px;
         font-weight: 600;
         margin-bottom: 5px;
      }

      .category-item-description{
         color: #555F65;
         font-size: 12px;
         max-width: 1024px;
      }

      .inherit-title-link{
         margin-right: 7px;
         font-size:19px;
         font-weight: 600;
      }

      .link-text{
         margin:0;
      }
   }
`;
class Customization extends React.Component {

   constructor(props) {
      super(props);

      const { portalLanguage, portalTimeZoneId, rawCultures, rawTimezones, t, greetingSettings } = props;
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
         greetingTitle: greetingSettings,
         isLoadingGreetingSave: false,
         isLoadingGreetingRestore: false,
         GreetingTitleIsDefault: GreetingTitleIsDefault(greetingSettings),
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

   onClickLink = (e) => {
      e.preventDefault();
      const { history } = this.props;
      history.push(e.target.pathname);
   }

   render() {
      const { t, i18n } = this.props;
      const { isLoadedData, language, timezone, greetingTitle, GreetingTitleIsDefault } = this.state;
      return (
         !isLoadedData ?
            <Loader className="pageLoader" type="rombs" size='40px' />
            : <>
               <StyledComponent>
                  <div className="category-item-wrapper">
                     <div className="category-item-heading">
                        <Link 
                           className='inherit-title-link header' 
                           onClick={this.onClickLink} 
                           href="/settings/common/customization/language-and-time-zone">
                              Language and time zone settings
                        </Link>
                        <Icons.ArrowRightIcon size="small" isfill={true} color="#333333" />
                     </div>
                     <Text className="category-item-subheader"> {language.label} / {timezone.label} </Text>
                     <Text className="category-item-description">
                        Language and time zone settings is a way to change the language 
                        of the whole portal for all portal users and to configure the time zone 
                        so that all the events of the portal will be shown with the correct date and time.
                     </Text>
                  </div>
                  <div className="category-item-wrapper">
                     <div className="category-item-heading">
                        <Link 
                           className='inherit-title-link header'
                           onClick={this.onClickLink} 
                           href="/settings/common/customization/custom-titles">
                              Custom titles
                        </Link>
                        <Icons.ArrowRightIcon size="small" isfill={true} color="#333333" />
                     </div>
                     {GreetingTitleIsDefault 
                        ? <Text className="category-item-subheader">{t("Default")}</Text>
                        : <Text className="category-item-subheader">{t("Custom") + ":" + greetingTitle}</Text>
                     }
                     <Text className="category-item-description">
                        Custom welcome page title will be displayed on the welcome page of your portal. The same name is also used for the From field of your portal email notifications. 
                        Custom domain name is a way to set an alternative URL for your portal. Custom portal name will appear next to the onlyoffice.com/onlyoffice.eu portal address.
                     </Text>
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
})(withTranslation()(Customization));
