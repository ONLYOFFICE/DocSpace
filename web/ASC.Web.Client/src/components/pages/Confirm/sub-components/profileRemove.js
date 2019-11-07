import React from 'react';
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { Button, PageLayout, Text } from 'asc-web-components';
import styled from 'styled-components';
import PropTypes from 'prop-types';
import { withTranslation } from 'react-i18next';
import { deleteSelf } from './../../../../store/services/api';
import { logout } from '../../../../store/auth/actions';

const ProfileRemoveContainer = styled.div`
    display: flex;
    flex-direction: column;
    align-items: center;

    .start-basis {
        align-items: flex-start;
    }
    
    .confirm-row {
        margin: 23px 0 0;
    }

    .break-word {
        word-break: break-word;
    }

`;

class ProfileRemove extends React.PureComponent {

  constructor() {
    super();

    this.state = {
      isProfileDeleted: false,
    };
  }

  onDeleteProfile = () => {
    this.setState({ isLoading: true }, function () {
      const { linkData, logout } = this.props;
      deleteSelf(linkData.confirmHeader)
        .then((res) => {
          this.setState({
            isLoading: false,
            isProfileDeleted: true
          });
          //setAuthorizationToken();
          console.log('success delete', res)
          return logout()
        })
        .catch((e) => {
          this.setState({ isLoading: false });
          console.log('error delete', e)
        })
    });
  };


  render() {
    console.log('profileRemove render');
    const { t, greetingTitle } = this.props;
    const { isProfileDeleted } = this.state;
    return (
      <ProfileRemoveContainer>
        <div className='start-basis'>

          <div className='confirm-row full-width break-word'>
            <a href='/login'>
              <img src="images/dark_general.png" alt="Logo" />
            </a>
            <Text.Body as='p' fontSize={24} color='#116d9d'>{greetingTitle}</Text.Body>
          </div>

          {!isProfileDeleted
            ? <>
              <Text.Body className='confirm-row' as='p' fontSize={18} >{t('DeleteProfileConfirmation')}</Text.Body>
              <Text.Body className='confirm-row' as='p' fontSize={16} >{t('DeleteProfileConfirmationInfo')}</Text.Body>

              <Button
                className='confirm-row'
                primary
                size='big'
                label={t('DeleteProfileBtn')}
                tabIndex={1}
                isLoading={this.state.isLoading}
                onClick={this.onDeleteProfile}
              />
            </>
            : <>
              <Text.Body className='confirm-row' as='p' fontSize={18} >{t('DeleteProfileSuccessMessage')}</Text.Body>
              <Text.Body className='confirm-row' as='p' fontSize={16} >{t('DeleteProfileSuccessMessageInfo')}</Text.Body>
            </>
          }

        </div>
      </ProfileRemoveContainer>
    );
  }
}


ProfileRemove.propTypes = {
  location: PropTypes.object.isRequired,
};
const ProfileRemoveForm = (props) => (<PageLayout sectionBodyContent={<ProfileRemove {...props} />} />);

function mapStateToProps(state) {
  return {
    greetingTitle: state.auth.settings.greetingSettings
  };
}

export default connect(mapStateToProps, { logout })(withRouter(withTranslation()(ProfileRemoveForm)));