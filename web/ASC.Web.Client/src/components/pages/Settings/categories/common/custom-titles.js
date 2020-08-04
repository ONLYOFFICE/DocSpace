import React from "react";
import { connect } from "react-redux";
import { withTranslation } from 'react-i18next';
import { FieldContainer, Loader, Button, toastr, TextInput, Link } from "asc-web-components";
import styled from 'styled-components';
import { setGreetingTitle, restoreGreetingTitle } from '../../../../../store/settings/actions';
import SaveSettingsButtons from '../../../../save-settings-buttons';

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
   .link-wrapper{
      margin-top: 8px;
   }
`;

const saveToSessionStorage = (key, value) => {
   sessionStorage.setItem(key, JSON.stringify(value))
}

const getFromSessionStorage = (key) => {
   return JSON.parse(sessionStorage.getItem(key));
}

let greetingTitleFromStorage = "";

const settingsOptions = [
   "greetingTitle"
];

class CustomTitles extends React.Component {

   constructor(props) {
      super(props);

      const { t, greetingSettings } = props;

      greetingTitleFromStorage = getFromSessionStorage("greetingTitle");

      document.title = `${t("Customization")} – ${t("OrganizationName")}`;

      this.state = {
         isLoadedData: false,
         isLoading: false,
         greetingTitle: greetingTitleFromStorage || greetingSettings,
         greetingTitleDefault: greetingSettings,
         isLoadingGreetingSave: false,
         isLoadingGreetingRestore: false,
         hasChanged: false,
         showReminder: false
      }
   }

   componentDidMount(){
      const { showReminder } = this.state

      if((greetingTitleFromStorage) && !showReminder){
         this.setState({
            showReminder: true
         })
      }
      this.setState({
         isLoadedData: true
      })
   }

   componentDidUpdate(prevProps, prevState){
      if (prevState.isLoadedData !== true){
         this.setState({
            isLoadedData: true
         })
      }

      console.log(this.state.greetingTitleDefault)

      if(this.state.greetingTitleDefault){

         this.checkChanges()
      }
   }

   onChangeGreetingTitle = (e) => {
      this.setState({ greetingTitle: e.target.value })

      if(this.isEqualDefault("greetingTitle", e.target.value)){
         saveToSessionStorage("greetingTitle", "")
      } else {
         saveToSessionStorage("greetingTitle", e.target.value)
      }

      this.checkChanges()
   };

   onSaveGreetingSettings = () => {
      const { setGreetingTitle, t } = this.props;
      const { greetingTitle } = this.state
      this.setState({ isLoadingGreetingSave: true }, function () {
         setGreetingTitle(greetingTitle)
            .then(() => toastr.success(t('SuccessfullySaveGreetingSettingsMessage')))
            .catch((error) => toastr.error(error))
            .finally(() => this.setState({ isLoadingGreetingSave: false }));
      })

      this.setState({
         showReminder: false,
         greetingTitleDefault:greetingTitle, 
      })
   }

   onRestoreGreetingSettings = () => {
      const { restoreGreetingTitle, t } = this.props;
      this.setState({ isLoadingGreetingRestore: true }, function () {
         restoreGreetingTitle()
            .then(() => {
               this.setState({
                  greetingTitle: this.props.greetingSettings,
                  greetingTitleDefault: this.props.greetingSettings,
                  showReminder: false,
               })
               toastr.success(t('SuccessfullySaveGreetingSettingsMessage'));
            })
            .catch((error) => toastr.error(error))
            .finally(() => this.setState({ isLoadingGreetingRestore: false }));
      })
   }

   onClickLink = (e) => {
      e.preventDefault();
      const { history } = this.props;
      history.push("/settings/common/customization");
   };

   onCancelClick = () => {

      for (let i = 0; i < settingsOptions.length; i++) {
         const value = getFromSessionStorage(settingsOptions[i]);

         if(value && !this.isEqualDefault(settingsOptions[i], value)) {
            const defaultValue = this.state[settingsOptions[i] + "Default"];

            this.setState({ [settingsOptions[i]]: defaultValue })
            saveToSessionStorage(settingsOptions[i], "")
         }
      }
      this.setState({
         showReminder: false
      })
      this.checkChanges()
   }

   isEqualDefault = (stateName, value) => {
      const defaultValue = JSON.stringify(this.state[stateName + "Default"])
      const currentValue = JSON.stringify(value)
      return defaultValue === currentValue
   }

   checkChanges = () => {
      let hasChanged = false;

      for (let i = 0; i < settingsOptions.length; i++) {
         const value = getFromSessionStorage(settingsOptions[i]);

         if(value && !this.isEqualDefault(settingsOptions[i], value)) hasChanged = true
      }

      if(hasChanged !== this.state.hasChanged){
         this.setState({
            hasChanged: hasChanged,
         })
      }
   }
   

   render() {
      const { t } = this.props;
      const { isLoadedData, greetingTitle, isLoadingGreetingSave, isLoadingGreetingRestore, hasChanged, showReminder } = this.state;

      return (
         !isLoadedData ?
            <Loader className="pageLoader" type="rombs" size='40px' />
            : <>
               <StyledComponent>
                  <div className='settings-block'>
                     <FieldContainer
                        id='fieldContainerWelcomePage'
                        className='field-container-width'
                        labelText={`${t("Welcome page title")}:`}
                        isVertical={true}>
                        <TextInput
                           scale={true}
                           value={greetingTitle}
                           onChange={this.onChangeGreetingTitle}
                           isDisabled={isLoadingGreetingSave || isLoadingGreetingRestore}
                        />
                        <div className="link-wrapper">
                           <Link 
                              onClick={this.onRestoreGreetingSettings}
                              type="action"
                              color="#A3A9AE"
                           > 
                              Set default title 
                           </Link>
                        </div>

                     </FieldContainer>
                  </div>
                  {hasChanged && 
                     <SaveSettingsButtons
                        onSaveClick={this.onSaveGreetingSettings}
                        onCancelClick={this.onCancelClick}
                        showReminder={showReminder}
                     />
                  }
               </StyledComponent>
            </>
      );
   }
};

function mapStateToProps(state) {
   return {
      greetingSettings: state.auth.settings.greetingSettings,
   };
}

export default connect(mapStateToProps, {setGreetingTitle, restoreGreetingTitle})(withTranslation()(CustomTitles));
