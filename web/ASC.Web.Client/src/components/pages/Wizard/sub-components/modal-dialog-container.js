import React from 'react';
import PropTypes from "prop-types";
import styled from 'styled-components';

import { 
  ModalDialog, 
  EmailInput, 
  Button,
  Box,
  utils 
} from 'asc-web-components';

const { tablet } = utils.device;

const Modal = ({ 
  t, 
  errorLoading, 
  visibleModal, 
  errorMessage, 
  emailOwner,
  settings,
  onEmailHandler,
  onSaveEmailHandler,
  onCloseModal
}) => {

  let header, content, footer;

  const visible = errorLoading ? errorLoading : visibleModal;

  if(errorLoading) {
    header = t('errorLicenseTitle');
    content = <span 
      className="modal-error-content">
        {errorMessage ? errorMessage: t('errorLicenseBody')}
    </span>;

  } else if( visibleModal ) {
    header = t('changeEmailTitle');

    content = <EmailInput
      tabIndex={1}
      scale={true}
      size='base'
      id="change-email"
      name="email-wizard"
      placeholder={t('placeholderEmail')}
      emailSettings={settings}
      value={emailOwner}
      onValidateInput={onEmailHandler}
    />;

    footer = <Box className="modal-button-save">
      <Button
        key="saveBtn"
        label={t('changeEmailBtn')}
        primary={true}
        scale={true}
        size="big"
        onClick={onSaveEmailHandler}
      />
    </Box>;
  }

  return <ModalDialog
      visible={visible}
      scale={false}
      displayType="auto"
      zIndex={310}
      headerContent={header}
      bodyContent={content}
      footerContent={footer}
      onClose={onCloseModal}
    />
}

const ModalContainer = styled(Modal)`
  .modal-button-save {
    width: 100px;
    
    @media ${tablet} {
      
    }
  }
  
  .modal-error-content {
    font-size: 13px;
    line-height: 20px;
  }
`;

ModalContainer.propTypes = {
  t: PropTypes.func.isRequired,
  errorLoading: PropTypes.bool.isRequired,
  visibleModal: PropTypes.bool.isRequired,
  emailOwner: PropTypes.string,
  settings: PropTypes.object.isRequired,
  onEmailHandler: PropTypes.func.isRequired,
  onSaveEmailHandler: PropTypes.func.isRequired,
  onCloseModal: PropTypes.func.isRequired
};

export default ModalContainer;