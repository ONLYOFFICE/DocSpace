import React from "react";
import { connect } from "react-redux";
import { withTranslation } from 'react-i18next';
import { FieldContainer, Loader, Button, toastr, TextInput } from "asc-web-components";
import styled from 'styled-components';
import { setGreetingTitle, restoreGreetingTitle } from '../../../../../store/settings/actions';

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
class CustomTitles extends React.Component {

   constructor(props) {
      super(props);

      const { t, greetingSettings } = props;

      document.title = `${t("Customization")} – ${t("OrganizationName")}`;

      this.state = {
         isLoadedData: false,
         isLoading: false,
         greetingTitle: greetingSettings,
         isLoadingGreetingSave: false,
         isLoadingGreetingRestore: false,
      }
   }

   componentDidMount(){
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
   }

   onChangeGreetingTitle = (e) => {
      this.setState({ greetingTitle: e.target.value })
   };

   onSaveGreetingSettings = () => {
      const { setGreetingTitle, t } = this.props;
      this.setState({ isLoadingGreetingSave: true }, function () {
         setGreetingTitle(this.state.greetingTitle)
            .then(() => toastr.success(t('SuccessfullySaveGreetingSettingsMessage')))
            .catch((error) => toastr.error(error))
            .finally(() => this.setState({ isLoadingGreetingSave: false }));
      })
   }

   onRestoreGreetingSettings = () => {
      const { restoreGreetingTitle, t } = this.props;
      this.setState({ isLoadingGreetingRestore: true }, function () {
         restoreGreetingTitle()
            .then(() => {
               this.setState({
                  greetingTitle: this.props.greetingSettings
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

   render() {
      const { t} = this.props;
      const { isLoadedData, greetingTitle, isLoadingGreetingSave, isLoadingGreetingRestore } = this.state;

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

                     </FieldContainer>

                     <Button
                        id='btnSaveGreetingSetting'
                        className='margin-top'
                        primary={true}
                        size='medium'
                        label={t('SaveButton')}
                        isLoading={isLoadingGreetingSave}
                        isDisabled={isLoadingGreetingRestore}
                        onClick={this.onSaveGreetingSettings}
                     />

                     <Button
                        id='btnRestoreToDefault'
                        className='margin-top margin-left'
                        size='medium'
                        label={t('RestoreDefaultButton')}
                        isLoading={isLoadingGreetingRestore}
                        isDisabled={isLoadingGreetingSave}
                        onClick={this.onRestoreGreetingSettings}
                     />
                  </div>
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
