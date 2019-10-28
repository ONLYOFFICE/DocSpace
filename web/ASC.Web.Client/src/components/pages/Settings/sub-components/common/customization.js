import React from "react";
import { connect } from "react-redux";
import { withTranslation } from 'react-i18next';
import { FieldContainer, Text, ComboBox, Loader, Button, toastr, Link } from "asc-web-components";
import { getCultures, setLanguageAndTime } from '../../../../../store/auth/actions';
import { getPortalTimezones } from '../../../../../store/services/api';
import styled from 'styled-components';
import { Trans } from 'react-i18next';

const StyledComponent = styled.div`
   .margin-top {
      margin-top: 20px;
   }

   .dropdown-item-width {
      div > div {
         max-width: 100%;
      }
   }
`;
class Customization extends React.Component {

   constructor() {
      super();
      this.state = {
         isLoadedData: false,
         isLoading: false,
         timezones: [],
         timezone: {},
         languages: [],
         language: {},
      }
   }


   componentDidMount() {
      const { getCultures, portalLanguage, portalTimeZoneId, t, i18n } = this.props;
      let languages, timezones;
      getCultures()
         .then((cultures) => {
            languages = cultures.map((culture) => {
               return { key: culture, label: t(`Culture_${culture}`) };
            });
            return getPortalTimezones();
         })
         .then((timezonesArr) => {
            timezones = timezonesArr.map((timezone) => {
               return { key: timezone.Id, label: timezone.DisplayName };
            });
            const language = languages.find(item => item.key === portalLanguage);
            const timezone = timezones.find(item => item.key === portalTimeZoneId);

            this.setState({ timezones, timezone, languages, language, isLoadedData: true });
            i18n.changeLanguage(this.props.language);
         }
         );
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
      const { t, i18n} = this.props;
      const { isLoadedData } = this.state;
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
                     className='margin-top'
                     labelText={`${t("Language")}:`}
                     tooltipContent={tooltipLanguage}
                     isVertical={true}>
                     <ComboBox
                        options={this.state.languages}
                        selectedOption={this.state.language}
                        onSelect={this.onLanguageSelect}
                        isDisabled={this.state.isLoading}
                        noBorder={false}
                        scaled={false}
                        scaledOptions={true}
                        size='huge'
                     />
                  </FieldContainer>

                  <FieldContainer
                     labelText={`${t("TimeZone")}:`}
                     isVertical={true}>
                     <ComboBox
                        options={this.state.timezones}
                        selectedOption={this.state.timezone}
                        onSelect={this.onTimezoneSelect}
                        isDisabled={this.state.isLoading}
                        noBorder={false}
                        scaled={false}
                        scaledOptions={true}
                        dropDownMaxHeight={300}
                        size='huge'
                        className='dropdown-item-width'
                     />
                  </FieldContainer>
                  <Button
                     className='margin-top'
                     primary={true}
                     size='big'
                     label={t('SaveButton')}
                     isLoading={this.state.isLoading}
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
   };
}

export default connect(mapStateToProps, { getCultures, setLanguageAndTime })(withTranslation()(Customization));
