import React from "react";
import { connect } from "react-redux";
import { withTranslation } from 'react-i18next';
import { FieldContainer, Text, ComboBox, Loader, Button, toastr, Link } from "asc-web-components";
import { getCultures, setLanguageAndTime, getPortalTimezones } from '../../../../../store/auth/actions';
import styled from 'styled-components';
import { Trans } from 'react-i18next';

const mapCulturesToArray = (cultures, t) => {
   return cultures.map((culture) => {
      return { key: culture, label: t(`Culture_${culture}`) };
   });
};

const mapTimezonesToArray = (timezones) => {
   return timezones.map((timezone) => {
      return { key: timezone.Id, label: timezone.DisplayName };
   });
};

const findSelectedItemByKey = (items, selectedItemKey) => {
   return items.find(item => item.key === selectedItemKey);
}

const StyledComponent = styled.div`
   .margin-top {
      margin-top: 20px;
   }

   .dropdown-item-width {
      & > div:first-child {
            div:first-child{
               max-width: 100%;
            }
      }
   }
`;
class Customization extends React.Component {

   constructor(props) {
      super(props);

      const { portalLanguage, portalTimeZoneId, rawCultures, rawTimezones, t } = props;
      const languages = mapCulturesToArray(rawCultures, t);
      const timezones = mapTimezonesToArray(rawTimezones);

      this.state = {
         isLoadedData: false,
         isLoading: false,
         timezones,
         timezone: findSelectedItemByKey(timezones, portalTimeZoneId),
         languages,
         language: findSelectedItemByKey(languages, portalLanguage),
      }
   }


   componentDidMount() {
      const { getCultures, portalLanguage, portalTimeZoneId, t, getPortalTimezones } = this.props;
      const { timezones, languages } = this.state;

      if (!timezones.length && !languages.length) {
         let languages;
         getCultures()
            .then(() => {
               languages = mapCulturesToArray(this.props.rawCultures, t);
            })
            .then(() => getPortalTimezones())
            .then(() => {
               const timezones = mapTimezonesToArray(this.props.rawTimezones);
               const timezone = findSelectedItemByKey(timezones, portalTimeZoneId);
               const language = findSelectedItemByKey(languages, portalLanguage);

               this.setState({ languages, language, timezones, timezone, isLoadedData: true });
            });
      }
      else {
         this.setState({ isLoadedData: true });
      }

   }

   onLanguageSelect = (language) => {
      this.setState({ language })
   };

   onTimezoneSelect = (timezone) => {
      this.setState({ timezone })
   };

   onSaveLngTZSettings = () => {
      const { setLanguageAndTime, t } = this.props;
      this.setState({ isLoading: true }, function () {
         setLanguageAndTime(this.state.language.key, this.state.timezone.key)
            .then(() => {
               this.setState({ isLoading: false })
               toastr.success(t('SuccessfullySaveSettingsMessage'));
            });
      })
   }

   render() {
      const { t, i18n } = this.props;
      const { isLoadedData, languages, language, isLoading, timezones, timezone } = this.state;
      const supportEmail = "documentation@onlyoffice.com";
      const tooltipLanguage =
         <Text.Body fontSize={13}>
            <Trans i18nKey="NotFoundLanguage" i18n={i18n}>
               "In case you cannot find your language in the list of the
               available ones, feel free to write to us at
         <Link href="mailto:documentation@onlyoffice.com" isHovered={true}>
                  {{ supportEmail }}
               </Link> to take part in the translation and get up to 1 year free of
               charge."
            </Trans>
            {" "}
            <Link isHovered={true} href="https://helpcenter.onlyoffice.com/ru/guides/become-translator.aspx">{t("LearnMore")}</Link>
         </Text.Body>

      console.log("CustomizationSettings render");
      return (
         !isLoadedData ?
            <Loader className="pageLoader" type="rombs" size={40} />
            : <>
               <StyledComponent>
                  <Text.Body fontSize={16}>{t('StudioTimeLanguageSettings')}</Text.Body>
                  <FieldContainer
                     id='fieldContainerLanguage'
                     className='margin-top'
                     labelText={`${t("Language")}:`}
                     tooltipContent={tooltipLanguage}
                     isVertical={true}>
                     <ComboBox
                        id='comboBoxLanguage'
                        options={languages}
                        selectedOption={language}
                        onSelect={this.onLanguageSelect}
                        isDisabled={isLoading}
                        noBorder={false}
                        scaled={false}
                        scaledOptions={true}
                        size='huge'
                     />
                  </FieldContainer>

                  <FieldContainer
                     id='fieldContainerTimezone'
                     labelText={`${t("TimeZone")}:`}
                     isVertical={true}>
                     <ComboBox
                        id='comboBoxTimezone'
                        options={timezones}
                        selectedOption={timezone}
                        onSelect={this.onTimezoneSelect}
                        isDisabled={isLoading}
                        noBorder={false}
                        scaled={false}
                        scaledOptions={true}
                        dropDownMaxHeight={300}
                        size='huge'
                        className='dropdown-item-width'
                     />
                  </FieldContainer>
                  <Button
                     id='btnSaveLngTZ'
                     className='margin-top'
                     primary={true}
                     size='big'
                     label={t('SaveButton')}
                     isLoading={isLoading}
                     onClick={this.onSaveLngTZSettings}
                  />
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
   };
}

export default connect(mapStateToProps, { getCultures, setLanguageAndTime, getPortalTimezones })(withTranslation()(Customization));
