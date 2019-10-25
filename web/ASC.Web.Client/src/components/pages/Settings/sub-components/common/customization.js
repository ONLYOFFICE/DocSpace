import React from "react";
import { connect } from "react-redux";
import { withTranslation } from 'react-i18next';
import { FieldContainer, Text, ComboBox, Loader, Button, toastr } from "asc-web-components";
import { getCultures, setLanguageAndTime } from '../../../../../store/auth/actions';
import { getPortalTimezones } from '../../../../../store/services/api';
import styled from 'styled-components'


const StyledComponent = styled.div`
   .margin-top {
      margin-top: 20px;
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
      const { getCultures, portalLanguage, portalTimeZoneId } = this.props;
      let languages, timezones;
      getCultures()
         .then((cultures) => {
            languages = cultures.map((culture) => {
               return { key: culture, label: culture };
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
      const { setLanguageAndTime } = this.props;
      this.setState({ isLoading: true }, function () {
         setLanguageAndTime(this.state.language.key, this.state.timezone.key)
            .then(() => {
               this.setState({ isLoading: false })
               toastr.success('Settings have been successfully updated');
            });
      })
   }

   render() {
      const { isLoadedData } = this.state;
      const tooltipContent = 'Tooltip content 123456';

      console.log("CustomizationSettings render");
      return (
         !isLoadedData ?
            <Loader className="pageLoader" type="rombs" size={40} />
            : <>
               <StyledComponent>
                  <Text.Body fontSize={16}>Language and Time Zone Settings</Text.Body>
                  <FieldContainer
                     className='margin-top'
                     labelText="Language:"
                     tooltipContent={tooltipContent}
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
                     labelText="Time Zone:"
                     tooltipContent={tooltipContent}
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
                     />
                  </FieldContainer>
                  <Button
                     className='margin-top'
                     primary={true}
                     size='big'
                     label='Save'
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
      portalTimeZoneId: state.auth.settings.timezone
   };
}

export default connect(mapStateToProps, { getCultures, setLanguageAndTime })(withTranslation()(Customization));
